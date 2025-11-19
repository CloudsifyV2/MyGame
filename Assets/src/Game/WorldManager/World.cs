using UnityEngine;

namespace MyGame.WorldManager
{
    public class World : MonoBehaviour
    {
        public int renderDistance = 2;

        void Start()
        {
            GenerateWorld();
        }

        void GenerateWorld()
        {
            for (int x = -renderDistance; x <= renderDistance; x++)
            {
                for (int z = -renderDistance; z <= renderDistance; z++)
                {
                    Vector2Int chunkPos = new Vector2Int(x, z);

                    // generate chunk data
                    Chunk chunk = new Chunk(chunkPos);
                    TerrainGenerator.GenerateTerrain(chunk);

                    // create chunk GameObject
                    GameObject chunkObj = new GameObject($"Chunk {x} {z}");
                    chunkObj.transform.position = new Vector3(
                        x * WorldData.ChunkSize,
                        0,
                        z * WorldData.ChunkSize
                    );

                    var renderer = chunkObj.AddComponent<ChunkRenderer>();
                    renderer.BuildChunkMesh(chunk);
                }
            }
        }
    }
}