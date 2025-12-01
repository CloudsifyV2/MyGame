using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public int hotbarSize = 9;
    public int inventorySize = 18;

    public List<InventoryStack> hotbarItems = new List<InventoryStack>();
    public List<InventoryStack> inventoryItems = new List<InventoryStack>();

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one Inventory instance found!");
            return;
        }
        instance = this;

        // Initialize slots
        for (int i = 0; i < hotbarSize; i++) hotbarItems.Add(new InventoryStack(null, 0));
        for (int i = 0; i < inventorySize; i++) inventoryItems.Add(new InventoryStack(null, 0));
    }

    // Add item to hotbar first, then inventory if possible
    public bool Add(Item item)
    {
        // Try to merge into existing stack in hotbar
        if (item.stackable)
        {
            for (int i = 0; i < hotbarItems.Count; i++)
            {
                var s = hotbarItems[i];
                if (s.item != null && s.item.id == item.id && s.count < item.maxStackSize)
                {
                    s.count += 1;
                    if (s.count > item.maxStackSize) s.count = item.maxStackSize;
                    Debug.Log(item.itemName + " stacked in hotbar slot " + i + " (" + s.count + ")");
                    return true;
                }
            }
        }

        // Put in empty hotbar slot
        for (int i = 0; i < hotbarItems.Count; i++)
        {
            var s = hotbarItems[i];
            if (s.item == null)
            {
                s.item = item;
                s.count = 1;
                Debug.Log(item.itemName + " added to hotbar slot " + i);
                return true;
            }
        }

        // Try to merge into inventory stacks
        if (item.stackable)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                var s = inventoryItems[i];
                if (s.item != null && s.item.id == item.id && s.count < item.maxStackSize)
                {
                    s.count += 1;
                    if (s.count > item.maxStackSize) s.count = item.maxStackSize;
                    Debug.Log(item.itemName + " stacked in inventory slot " + i + " (" + s.count + ")");
                    return true;
                }
            }
        }

        // Put in empty inventory slot
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            var s = inventoryItems[i];
            if (s.item == null)
            {
                s.item = item;
                s.count = 1;
                Debug.Log(item.itemName + " added to inventory slot " + i);
                return true;
            }
        }

        Debug.Log("Inventory Full! Could not add " + item.itemName);
        return false;
    }

    public void Remove(Item item)
    {
        // Remove one unit from hotbar stacks first
        for (int i = 0; i < hotbarItems.Count; i++)
        {
            var s = hotbarItems[i];
            if (s.item != null && s.item.id == item.id)
            {
                s.count -= 1;
                if (s.count <= 0)
                {
                    s.item = null;
                    s.count = 0;
                }
                Debug.Log(item.itemName + " removed from hotbar slot " + i + " (left=" + s.count + ")");
                return;
            }
        }

        // Then remove from inventory
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            var s = inventoryItems[i];
            if (s.item != null && s.item.id == item.id)
            {
                s.count -= 1;
                if (s.count <= 0)
                {
                    s.item = null;
                    s.count = 0;
                }
                Debug.Log(item.itemName + " removed from inventory slot " + i + " (left=" + s.count + ")");
                return;
            }
        }
    }
}
