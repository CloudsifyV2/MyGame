using UnityEngine;
using System.Collections.Generic;

namespace MyGame.WorldManager
{
    public class World : MonoBehaviour
    {
        public int renderDistance = 6;
        public Material blockMaterial;
        public Transform player;
        [Header("Debug")]
        public bool debugBiomeSampling = false;
        // radius in chunks to sample for debug distribution
        public int debugBiomeRadiusChunks = 6;
        [Header("Generation")]
        [Tooltip("Radius (in chunks) to pre-generate and keep loaded at Start. Increase to make world larger at runtime.")]
        public int initialLoadRadius = 10;

        private Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();
        private Vector2Int currentChunk;
        private Vector2Int lastChunk;
        private BiomeType lastPlayerBiome;
        private bool hasLastPlayerBiome = false;
        // Water plane that follows player to simulate oceans
        public bool enableWater = true;
        private GameObject waterObject;

        // Tree spawning
        public bool spawnTrees = true;
        // Background build queue for chunk generation
        private System.Collections.Generic.Queue<Vector2Int> buildQueue = new System.Collections.Generic.Queue<Vector2Int>();
        private System.Collections.Generic.HashSet<Vector2Int> queuedSet = new System.Collections.Generic.HashSet<Vector2Int>();
        [Tooltip("Maximum number of chunks to build per frame (use 1-4 for smooth streaming)")]
        public int chunksPerFrame = 2;

        // Expose runtime counters for debug GUI
        public int LoadedChunkCount => chunks != null ? chunks.Count : 0;
        public int BuildQueueLength => buildQueue != null ? buildQueue.Count : 0;
        public int QueuedSetCount => queuedSet != null ? queuedSet.Count : 0;

        void Start()
        {
            // Find player if not assigned
            if (player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                    player = playerObj.transform;
                else if (Camera.main != null)
                    player = Camera.main.transform;
            }

            // Load material if not assigned
            if (blockMaterial == null)
            {
                blockMaterial = Resources.Load<Material>("Materials/BlockMaterial");
                if (blockMaterial == null)
                {
                    Debug.LogError("BlockMaterial not found! Create it at Resources/Materials/BlockMaterial.mat");
                    Debug.LogError("Make sure to use URP/Lit or URP/Unlit shader in Unity 6");
                    return;
                }
                
                // Check if material has a texture
                if (blockMaterial.mainTexture == null)
                {
                    Debug.LogError("BlockMaterial has no texture assigned! Assign BlockAtlas to Base Map.");
                }
                else
                {
                    Debug.Log($"Material loaded successfully with texture: {blockMaterial.mainTexture.name}");
                    Debug.Log($"Texture size: {blockMaterial.mainTexture.width}x{blockMaterial.mainTexture.height}");
                    Debug.Log($"Material shader: {blockMaterial.shader.name}");
                }
            }
            else
            {
                // Material was assigned in inspector
                Debug.Log($"Material assigned in inspector: {blockMaterial.name}");
                if (blockMaterial.mainTexture != null)
                {
                    Debug.Log($"Texture: {blockMaterial.mainTexture.name}, Size: {blockMaterial.mainTexture.width}x{blockMaterial.mainTexture.height}");
                }
                else
                {
                    Debug.LogError("Assigned material has no texture!");
                }
            }

            // Generate initial chunks
            if (player != null)
            {
                currentChunk = GetChunkPosition(player.position);
                lastChunk = currentChunk;
                // Generate visible chunks first
                GenerateChunksAroundPlayer();
                // Optionally pre-generate a larger area and keep it loaded
                if (initialLoadRadius > renderDistance)
                {
                    GenerateChunksAtRadius(initialLoadRadius);
                }
            }

            // Start background build coroutine
            StartCoroutine(ProcessBuildQueue());

            // Create a simple water plane that follows the player so oceans look large
            if (enableWater)
            {
                waterObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                waterObject.name = "Water";
                waterObject.transform.parent = transform;
                waterObject.transform.position = new Vector3(0, WorldData.SeaLevel - 0.5f, 0);
                // large scale; tune as needed
                waterObject.transform.localScale = Vector3.one * 200f;
                var rend = waterObject.GetComponent<MeshRenderer>();
                Material waterMat = Resources.Load<Material>("Materials/Water");
                if (waterMat != null)
                {
                    rend.sharedMaterial = waterMat;
                }
                else if (blockMaterial != null)
                {
                    // fallback: tint the block material
                    Material fallback = new Material(blockMaterial);
                    fallback.color = new Color(0.1f, 0.45f, 0.7f, 0.7f);
                    rend.sharedMaterial = fallback;
                }
                // disable collider for the plane
                var col = waterObject.GetComponent<Collider>();
                if (col != null) col.enabled = false;
            }

            // record starting biome (if player exists)
            if (player != null)
            {
                Vector3 p = player.position;
                int wx = Mathf.FloorToInt(p.x);
                int wz = Mathf.FloorToInt(p.z);
                lastPlayerBiome = TerrainGenerator.GetBiome(wx, wz);
                hasLastPlayerBiome = true;
                Debug.Log($"Starting biome: {lastPlayerBiome}");
                if (debugBiomeSampling)
                    LogBiomeDistribution(wx, wz, debugBiomeRadiusChunks);
            }
        }

        void Update()
        {
            if (player == null) return;

            // Check if player moved to a new chunk
            currentChunk = GetChunkPosition(player.position);
            if (currentChunk != lastChunk)
            {
                lastChunk = currentChunk;
                GenerateChunksAroundPlayer();
                UnloadDistantChunks();
            }

            // Check biome at player's world position and log changes
            Vector3 p = player.position;
            int worldX = Mathf.FloorToInt(p.x);
            int worldZ = Mathf.FloorToInt(p.z);
            BiomeType current = TerrainGenerator.GetBiome(worldX, worldZ);
            if (!hasLastPlayerBiome || current != lastPlayerBiome)
            {
                Debug.Log($"Player entered biome: {current}");
                lastPlayerBiome = current;
                hasLastPlayerBiome = true;
            }
        }

        private void LogBiomeDistribution(int centerX, int centerZ, int radiusChunks)
        {
            var counts = new Dictionary<BiomeType, int>();
            foreach (BiomeType b in System.Enum.GetValues(typeof(BiomeType))) counts[b] = 0;

            int step = WorldData.ChunkSize; // sample per chunk
            for (int cx = -radiusChunks; cx <= radiusChunks; cx++)
            {
                for (int cz = -radiusChunks; cz <= radiusChunks; cz++)
                {
                    int sampleX = centerX + cx * step;
                    int sampleZ = centerZ + cz * step;
                    BiomeType b = TerrainGenerator.GetBiome(sampleX, sampleZ);
                    counts[b]++;
                }
            }

            string outStr = "Biome distribution:\n";
            foreach (var kv in counts)
            {
                outStr += $" - {kv.Key}: {kv.Value}\n";
            }
            Debug.Log(outStr);
        }

        Vector2Int GetChunkPosition(Vector3 worldPos)
        {
            int x = Mathf.FloorToInt(worldPos.x / WorldData.ChunkSize);
            int z = Mathf.FloorToInt(worldPos.z / WorldData.ChunkSize);
            return new Vector2Int(x, z);
        }

        /// <summary>
        /// Wait until chunks around a world position are present in the loaded chunk dictionary.
        /// Returns when all chunk positions within radiusChunks are present or when timeout reached.
        /// </summary>
        public System.Collections.IEnumerator WaitForChunksAt(float worldX, float worldZ, int radiusChunks, float timeoutSeconds = 10f)
        {
            Vector2Int center = new Vector2Int(
                Mathf.FloorToInt(worldX / WorldData.ChunkSize),
                Mathf.FloorToInt(worldZ / WorldData.ChunkSize)
            );

            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - start < timeoutSeconds)
            {
                bool allPresent = true;
                for (int dx = -radiusChunks; dx <= radiusChunks && allPresent; dx++)
                {
                    for (int dz = -radiusChunks; dz <= radiusChunks; dz++)
                    {
                        Vector2Int pos = new Vector2Int(center.x + dx, center.y + dz);
                        if (!chunks.ContainsKey(pos))
                        {
                            allPresent = false;
                            break;
                        }
                    }
                }

                if (allPresent) yield break;

                // wait a frame and try again
                yield return null;
            }

            // timeout reached; just exit and allow spawn
            yield break;
        }

        /// <summary>
        /// Start generating chunks centered at an arbitrary world position (useful when the player hasn't spawned yet).
        /// </summary>
        public void GenerateAroundPosition(float worldX, float worldZ, int radius)
        {
            Vector2Int center = new Vector2Int(
                Mathf.FloorToInt(worldX / WorldData.ChunkSize),
                Mathf.FloorToInt(worldZ / WorldData.ChunkSize)
            );

            currentChunk = center;
            // enqueue the requested radius for generation
            GenerateChunksAtRadius(radius);
        }

        void GenerateChunksAroundPlayer()
        {
            for (int x = -renderDistance; x <= renderDistance; x++)
            {
                for (int z = -renderDistance; z <= renderDistance; z++)
                {
                    Vector2Int chunkPos = new Vector2Int(currentChunk.x + x, currentChunk.y + z);

                    // Skip if chunk already exists
                    if (chunks.ContainsKey(chunkPos))
                        continue;

                    // Generate new chunk
                    GenerateChunk(chunkPos);
                }
            }
        }

        /// <summary>
        /// Generate chunks in a square radius around the current chunk and keep them loaded.
        /// Use with care for large radii (memory/perf cost).
        /// </summary>
        /// <param name="radius">Radius in chunks</param>
        void GenerateChunksAtRadius(int radius)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dz = -radius; dz <= radius; dz++)
                {
                    Vector2Int chunkPos = new Vector2Int(currentChunk.x + dx, currentChunk.y + dz);
                    if (chunks.ContainsKey(chunkPos)) continue;
                    GenerateChunk(chunkPos);
                }
            }
        }

        void GenerateChunk(Vector2Int chunkPos)
        {
            // Enqueue chunk for background generation/building
            if (chunks.ContainsKey(chunkPos)) return; // already built
            if (queuedSet.Contains(chunkPos)) return; // already queued

            queuedSet.Add(chunkPos);
            buildQueue.Enqueue(chunkPos);
        }

        private System.Collections.IEnumerator ProcessBuildQueue()
        {
            while (true)
            {
                int built = 0;
                while (buildQueue.Count > 0 && built < Mathf.Max(1, chunksPerFrame))
                {
                    Vector2Int pos = buildQueue.Dequeue();
                    queuedSet.Remove(pos);

                    // Skip if chunk was created while queued
                    if (chunks.ContainsKey(pos)) continue;

                    // Generate chunk data
                    Chunk chunk = new Chunk(pos);
                    TerrainGenerator.GenerateTerrain(chunk);

                    // Create chunk GameObject
                    GameObject chunkObj = new GameObject($"Chunk {pos.x} {pos.y}");
                    chunkObj.transform.position = new Vector3(
                        pos.x * WorldData.ChunkSize,
                        0,
                        pos.y * WorldData.ChunkSize
                    );
                    chunkObj.transform.parent = transform;

                    // Add required components
                    chunkObj.AddComponent<MeshFilter>();
                    var meshRenderer = chunkObj.AddComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = blockMaterial;

                    // Build the mesh
                    var renderer = chunkObj.AddComponent<ChunkRenderer>();
                    renderer.BuildChunkMesh(chunk);

                    // Spawn simple trees for forest chunks
                    if (spawnTrees)
                    {
                        BiomeType chunkBiome = TerrainGenerator.GetBiome(
                            pos.x * WorldData.ChunkSize + WorldData.ChunkSize / 2,
                            pos.y * WorldData.ChunkSize + WorldData.ChunkSize / 2
                        );
                        if (chunkBiome == BiomeType.Forest)
                        {
                            int treeCount = UnityEngine.Random.Range(1, 4); // 1-3 trees
                            for (int t = 0; t < treeCount; t++)
                            {
                                int lx = UnityEngine.Random.Range(0, WorldData.ChunkSize);
                                int lz = UnityEngine.Random.Range(0, WorldData.ChunkSize);
                                // find surface y in this chunk column
                                int topY = -1;
                                for (int y = WorldData.ChunkHeight - 1; y >= 0; y--)
                                {
                                    if (chunk.blocks[lx, y, lz] != BlockType.Air)
                                    {
                                        topY = y;
                                        break;
                                    }
                                }
                                if (topY <= 0) continue;

                                Vector3 treeWorldPos = chunkObj.transform.position + new Vector3(lx + 0.5f, topY + 1f, lz + 0.5f);
                                CreateSimpleTree(treeWorldPos, chunkObj.transform);
                            }
                        }
                    }

                    // Store in dictionary
                    chunks.Add(pos, chunkObj);

                    built++;
                }

                // yield until next frame to avoid freezing
                yield return null;
            }
        }

        void UnloadDistantChunks()
        {
            List<Vector2Int> chunksToRemove = new List<Vector2Int>();

            foreach (var kvp in chunks)
            {
                Vector2Int chunkPos = kvp.Key;
                int distance = Mathf.Max(
                    Mathf.Abs(chunkPos.x - currentChunk.x),
                    Mathf.Abs(chunkPos.y - currentChunk.y)
                );

                // Keep chunks within the initial load radius (if set), otherwise keep within renderDistance
                int keepRadius = Mathf.Max(initialLoadRadius, renderDistance);

                // Unload chunks beyond keepRadius + 1 (buffer)
                if (distance > keepRadius + 1)
                {
                    chunksToRemove.Add(chunkPos);
                }
            }

            // Remove distant chunks
            foreach (Vector2Int chunkPos in chunksToRemove)
            {
                if (chunks.TryGetValue(chunkPos, out GameObject chunkObj))
                {
                    Destroy(chunkObj);
                    chunks.Remove(chunkPos);
                }
            }
        }

        private void CreateSimpleTree(Vector3 worldPosition, Transform parent)
        {
            GameObject tree = new GameObject("Tree");
            tree.transform.parent = parent;
            tree.transform.position = worldPosition;

            // trunk
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.parent = tree.transform;
            trunk.transform.localScale = new Vector3(0.3f, 1f, 0.3f);
            trunk.transform.localPosition = new Vector3(0f, 1f, 0f);
            if (blockMaterial != null)
                trunk.GetComponent<MeshRenderer>().sharedMaterial = blockMaterial;

            // leaves
            GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leaves.transform.parent = tree.transform;
            leaves.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            leaves.transform.localPosition = new Vector3(0f, 2.2f, 0f);
            if (blockMaterial != null)
                leaves.GetComponent<MeshRenderer>().sharedMaterial = blockMaterial;
        }
    }
}