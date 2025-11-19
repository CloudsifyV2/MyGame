using UnityEngine;

namespace MyGame.WorldManager
{
    public static class BlockTextures
    {   
        public static Vector2[] GrassTop  => BlockUVs.GetUVs(0, 3);
        public static Vector2[] GrassSide => BlockUVs.GetUVs(1, 3);
        public static Vector2[] Dirt      => BlockUVs.GetUVs(2, 3);

        public static Vector2[] Stone     => BlockUVs.GetUVs(0, 2);

        public static Vector2[] GetUVs(BlockType type, int face)
        {
            switch (type)
            {
                case BlockType.Grass:
                    if (face == 2)  return GrassTop;      // top
                    if (face == 3)  return Dirt;          // bottom
                    return GrassSide;                     // sides

                case BlockType.Dirt:
                    return Dirt;

                default:
                    return Dirt;
            }
        }
    }
}