using UnityEngine;

namespace MyGame.WorldManager
{
    public static class BlockUVs
    {
        // Number of tiles across the atlas (4x4 grid)
        public const float tileCount = 4f;

        /// <summary>
        /// Calculate UV coordinates for a tile in the atlas
        /// </summary>
        /// <param name="tileX">Column (0-3)</param>
        /// <param name="tileY">Row (0-3)</param>
        /// <returns>Array of 4 UV coordinates: [bottomLeft, bottomRight, topRight, topLeft]</returns>
        public static Vector2[] GetUVs(int tileX, int tileY)
        {
            float size = 1f / tileCount; // Each tile is 0.25 of the texture

            Vector2 bottomLeft  = new Vector2(tileX * size, tileY * size);
            Vector2 bottomRight = new Vector2((tileX + 1) * size, tileY * size);
            Vector2 topRight    = new Vector2((tileX + 1) * size, (tileY + 1) * size);
            Vector2 topLeft     = new Vector2(tileX * size, (tileY + 1) * size);

            // Return in the same order as face vertices
            return new Vector2[] { bottomLeft, bottomRight, topRight, topLeft };
        }
    }
}