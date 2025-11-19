using UnityEngine;
using System.Collections.Generic;

namespace MyGame.WorldManager
{
    public class ChunkRenderer : MonoBehaviour
    {
        public Chunk chunk;

        // The player or camera to check distance from
        private Transform player;

        // Distance in blocks beyond which cubes are destroyed
        public float maxDistance = 20f;

        // Keep track of spawned cubes for distance checking
        private List<GameObject> spawnedCubes = new List<GameObject>();

        private void Awake()
        {
            // Try to find the player or main camera automatically
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else if (Camera.main != null)
                player = Camera.main.transform;
        }

        /// <summary>
        /// Build the chunk by creating cubes for each non-air block
        /// </summary>
        public void BuildChunkMesh(Chunk chunkData)
        {
            chunk = chunkData;

            // Clean old blocks
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            spawnedCubes.Clear();

            for (int x = 0; x < WorldData.ChunkSize; x++)
            {
                for (int y = 0; y < WorldData.ChunkHeight; y++)
                {
                    for (int z = 0; z < WorldData.ChunkSize; z++)
                    {
                        BlockType block = chunk.blocks[x, y, z];
                        if (block == BlockType.Air) continue;

                        Vector3 pos = new Vector3(x, y, z);
                        SpawnCube(pos, block);
                    }
                }
            }
        }

        private void Update()
        {
            if (player == null) return;

            // Remove cubes that are too far from the player
            for (int i = spawnedCubes.Count - 1; i >= 0; i--)
            {
                GameObject cube = spawnedCubes[i];
                if (cube == null) continue;

                float distance = Vector3.Distance(player.position, cube.transform.position);
                if (distance > maxDistance)
                {
                    Destroy(cube);
                    spawnedCubes.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Creates a cube primitive at the given position and assigns a material
        /// </summary>
        private void SpawnCube(Vector3 pos, BlockType block)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = transform;
            cube.transform.localPosition = pos;

            // Assign material based on block type
            var renderer = cube.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material mat = block switch
                {
                    BlockType.Grass => Resources.Load<Material>("Materials/grass_top"),
                    BlockType.Dirt => Resources.Load<Material>("Materials/dirt"),
                    BlockType.Stone => Resources.Load<Material>("Materials/stone"),
                    _ => Resources.Load<Material>("Materials/BlockMaterial")
                };
                renderer.material = mat;
            }

            spawnedCubes.Add(cube);
        }
    }
}
