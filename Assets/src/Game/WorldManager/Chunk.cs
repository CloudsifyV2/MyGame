using UnityEngine;

namespace MyGame.WorldManager
{
    public class Chunk
    {
        public BlockType[,,] blocks;
        public Vector2Int position;

        public Chunk(Vector2Int pos)
        {
            position = pos;
            blocks = new BlockType[
                WorldData.ChunkSize,
                WorldData.ChunkHeight,
                WorldData.ChunkSize
            ];
        }
    }
}
