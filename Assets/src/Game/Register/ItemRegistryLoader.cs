using UnityEngine;
using MyGame.WorldManager;

namespace MyGame.Register
{
    public class ItemRegistryLoader : MonoBehaviour
    {
        public Item dirtItem;
        public Item grassItem;
        public Item stoneItem;
        public Item sandItem;
        public Item snowItem;

        private void Awake()
        {
            BlockItemRegistry.Register(BlockType.Dirt, dirtItem);
            BlockItemRegistry.Register(BlockType.Grass, grassItem);
            BlockItemRegistry.Register(BlockType.Stone, stoneItem);
            BlockItemRegistry.Register(BlockType.Sand, sandItem);
            BlockItemRegistry.Register(BlockType.Snow, snowItem);
        }
    }
}