using UnityEngine;

namespace MyGame.WorldManager
{
    public static class BlockUVs
    {
        public const float tileCount = 4f;

        public static Vector2[] GetUVs(int tileX, int tileY)
        {
            float size = 1f / tileCount;

            Vector2 bottomLeft  = new Vector2(tileX * size, tileY * size);
            Vector2 bottomRight = new Vector2((tileX + 1) * size, tileY * size);
            Vector2 topRight    = new Vector2((tileX + 1) * size, (tileY + 1) * size);
            Vector2 topLeft     = new Vector2(tileX * size, (tileY + 1) * size);

            return new Vector2[] { bottomLeft, bottomRight, topRight, topLeft };
        }
    }
}
