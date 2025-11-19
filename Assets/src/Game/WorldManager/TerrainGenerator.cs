using UnityEngine;

namespace MyGame.WorldManager
{
    public static class TerrainGenerator
    {
        public static Chunk GenerateTerrain(Chunk chunk)
        {
            for (int x = 0; x < WorldData.ChunkSize; x++)
            {
                for (int z = 0; z < WorldData.ChunkSize; z++)
                {
                    int worldX = chunk.position.x * WorldData.ChunkSize + x;
                    int worldZ = chunk.position.y * WorldData.ChunkSize + z;

                    int height = GetHeight(worldX, worldZ);

                    height = Mathf.Clamp(height, 0, WorldData.ChunkHeight - 1);

                    for (int y = 0; y < WorldData.ChunkHeight; y++)
                    {
                        if (y > height)
                        {
                            chunk.blocks[x, y, z] = BlockType.Air;
                        }
                        else if (y == height)
                        {
                            chunk.blocks[x, y, z] = BlockType.Grass;
                        }
                        else if (y >= height - 3)
                        {
                            chunk.blocks[x, y, z] = BlockType.Dirt;
                        }
                        else
                        {
                            // Everything below is stone
                            chunk.blocks[x, y, z] = BlockType.Stone;
                        }
                    }
                }
            }

            return chunk;
        }

        public static int GetHeight(int x, int z)
        {
            float noise = Mathf.PerlinNoise(
                x / WorldData.NoiseScale,
                z / WorldData.NoiseScale
            );

            int height = Mathf.FloorToInt(noise * WorldData.HeightMultiplier) + WorldData.GroundLevel;

            return Mathf.Clamp(height, 0, WorldData.ChunkHeight - 1);
        }
    }
}
