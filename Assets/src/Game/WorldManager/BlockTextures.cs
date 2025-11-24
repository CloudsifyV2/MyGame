using UnityEngine;

namespace MyGame.WorldManager
{
    public static class BlockTextures
    {   
        // Define texture positions in the atlas (column, row)
        // NOTE: Row numbers are from BOTTOM of texture (Unity reads textures bottom-to-top)
        public static Vector2[] GrassTop  => BlockUVs.GetUVs(0, 0);  // Bottom-left in image editor
        public static Vector2[] GrassSide => BlockUVs.GetUVs(1, 0);  
        public static Vector2[] Dirt      => BlockUVs.GetUVs(2, 0);  
        public static Vector2[] Stone     => BlockUVs.GetUVs(0, 1);  // One row up from bottom
        public static Vector2[] Sand      => BlockUVs.GetUVs(3, 0);
        public static Vector2[] Snow      => BlockUVs.GetUVs(0, 2);

        /// <summary>
        /// Get UV coordinates for a specific block type and face
        /// </summary>
        /// <param name="type">Block type</param>
        /// <param name="faceIndex">Face index: 0=Front, 1=Back, 2=Top, 3=Bottom, 4=Right, 5=Left</param>
        public static Vector2[] GetUVs(BlockType type, int faceIndex)
        {
            switch (type)
            {
                case BlockType.Air:
                    return Dirt; // Shouldn't be called but just in case

                case BlockType.Grass:
                    if (faceIndex == 2)  // Top face
                        return GrassTop;
                    if (faceIndex == 3)  // Bottom face
                        return Dirt;
                    return GrassSide;    // All side faces

                case BlockType.Dirt:
                    return Dirt;

                case BlockType.Stone:
                    return Stone;

                case BlockType.Sand:
                    return Sand;

                case BlockType.Snow:
                    return Snow;

                default:
                    UnityEngine.Debug.LogWarning($"Unknown block type: {type}");
                    return Dirt; // Fallback texture
            }
        }
    }
}