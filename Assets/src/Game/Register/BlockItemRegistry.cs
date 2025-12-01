using System.Collections.Generic;
using UnityEngine;
using MyGame.WorldManager;

namespace MyGame.Register
{
    public static class BlockItemRegistry
    {
        private static Dictionary<BlockType, Item> blockToItem = new Dictionary<BlockType, Item>();

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
    }
}