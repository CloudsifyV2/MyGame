using System.Collections.Generic;
using UnityEngine;
using MyGame.WorldManager;

namespace MyGame.Register
{
    public static class BlockItemRegistry
    {
        private static Dictionary<BlockType, Item> blockToItem = new Dictionary<BlockType, Item>();
        private static Dictionary<Item, BlockType> itemToBlock = new Dictionary<Item, BlockType>();

        public static void Register(BlockType block, Item item)
        {
            if (!blockToItem.ContainsKey(block))
                blockToItem.Add(block, item);
        }

        public static Item GetItemForBlock(BlockType block)
        {
            if (blockToItem.TryGetValue(block, out Item item))
                return item;

            return null; // No drop
        }

        public static BlockType GetBlockForItem(Item item)
        {
            foreach (var pair in blockToItem)
            {
                if (pair.Value == item)
                    return pair.Key;
            }
            return BlockType.Air; // AIRRRRR
        }
    }
}