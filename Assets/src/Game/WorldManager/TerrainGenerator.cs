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

                    // Determine biome at this location
                    BiomeType biome = GetBiome(worldX, worldZ);

                    for (int y = 0; y < WorldData.ChunkHeight; y++)
                    {
                        if (y > height)
                        {
                            chunk.blocks[x, y, z] = BlockType.Air;
                            continue;
                        }

                        // Beach rule: near sea level we make sand
                        if (height <= WorldData.SeaLevel + 1)
                        {
                            // shallow areas -> sand
                            if (y == height)
                                chunk.blocks[x, y, z] = BlockType.Sand;
                            else if (y > height - 3)
                                chunk.blocks[x, y, z] = BlockType.Sand;
                            else
                                chunk.blocks[x, y, z] = BlockType.Stone;
                            continue;
                        }

                        switch (biome)
                        {
                            case BiomeType.Desert:
                                if (y >= height - 3)
                                    chunk.blocks[x, y, z] = BlockType.Sand;
                                else
                                    chunk.blocks[x, y, z] = BlockType.Stone;
                                break;
                            case BiomeType.Mountain:
                                    // Mountain tops get snow when high enough
                                    float mountainTopThreshold = WorldData.GroundLevel + (WorldData.HeightMultiplier * 0.55f);
                                    if (y == height && height >= mountainTopThreshold)
                                        chunk.blocks[x, y, z] = BlockType.Snow;
                                    else if (y == height)
                                        chunk.blocks[x, y, z] = BlockType.Stone;
                                    else if (y > height - 6)
                                        chunk.blocks[x, y, z] = BlockType.Stone;
                                    else
                                        chunk.blocks[x, y, z] = BlockType.Stone;
                                break;
                            case BiomeType.Forest:
                            case BiomeType.Plains:
                            default:
                                if (y == height)
                                    chunk.blocks[x, y, z] = BlockType.Grass;
                                else if (y > height - 4)
                                    chunk.blocks[x, y, z] = BlockType.Dirt;
                                else
                                    chunk.blocks[x, y, z] = BlockType.Stone;
                                break;
                        }
                    }
                }
            }

            return chunk;
        }

        public static BiomeType GetBiome(int x, int z)
        {
            // Use Perlin noise to choose biome; value [0,1]
            float n = Mathf.PerlinNoise(
                (x + WorldData.WorldSeed) / WorldData.BiomeNoiseScale,
                (z + WorldData.WorldSeed) / WorldData.BiomeNoiseScale
            );

            // thresholds can be adjusted for different biome prevalence
            if (n < 0.25f) return BiomeType.Desert;
            if (n < 0.4f) return BiomeType.Beach;
            if (n < 0.7f) return BiomeType.Plains;
            if (n < 0.9f) return BiomeType.Forest;
            return BiomeType.Mountain;
        }

        public static int GetHeight(int x, int z)
        {
            float noise = Mathf.PerlinNoise(
                x / WorldData.NoiseScale,
                z / WorldData.NoiseScale
            );

            // Add a second octave to make mountains more varied
            float noise2 = Mathf.PerlinNoise(
                (x + WorldData.WorldSeed) / (WorldData.NoiseScale / 2f),
                (z + WorldData.WorldSeed) / (WorldData.NoiseScale / 2f)
            );

            float combined = (noise * 0.7f) + (noise2 * 0.3f);

            int height = Mathf.FloorToInt(combined * WorldData.HeightMultiplier) + WorldData.GroundLevel;

            return Mathf.Clamp(height, 0, WorldData.ChunkHeight - 1);
        }
    }
}