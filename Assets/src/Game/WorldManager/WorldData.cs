using UnityEngine;

namespace MyGame.WorldManager
{
    public static class WorldData
    {
        public const int ChunkSize = 16;
        public const int ChunkHeight = 256; // Taller world for mountains/caves
        public const int MaxRenderDistance = 12; // Increased from 4

        // Better noise parameters for larger scale
        public const float NoiseScale = 60f;
        public const float HeightMultiplier = 30f;
        public const int GroundLevel = 32;

        // Biome / water
        public const int SeaLevel = 30;
        // Smaller biome noise scale gives more frequent biome changes across world
        public const float BiomeNoiseScale = 64f;
        // A seed offset can be used to change overall world
        public static int WorldSeed = 0;
    }
}