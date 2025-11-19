using System.Collections.Generic;
using UnityEngine;

namespace Game.InventoryManager
{
    public class Inventory : MonoBehaviour
    {
        public int inventorySize = 36;
        public List<InventorySlot> slots = new();

        void Awake()
        {
            for (int i = 0; i < inventorySize; i++)
                slots.Add(new InventorySlot());
        }

        public bool AddItem(ItemData item, int amount = 1)
        {
            // Try to stack first
            foreach (var slot in slots)
            {
                if (slot.CanItemBeAdded(item))
                {
                    slot.AddItem(item, amount);
                    return true;
                }
            }

            // Otherwise, find empty slot
            foreach (var slot in slots)
            {
                if (slot.isEmpty)
                {
                    slot.AddItem(item, amount);
                    return true;
                }
            }

            Debug.Log("Inventory full! Cannot add item: " + item.itemName);
            return false;
        }

        public void RemoveItem(ItemData item, int amount = 1)
        {
            foreach (var slot in slots)
            {
                if (slot.item == item)
                {
                    slot.amount -= amount;
                    if (slot.amount <= 0)
                        slot.ClearSlot();
                    return;
                }
            }
        }
    }
}