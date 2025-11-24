using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public int hotbarSize = 9;
    public int inventorySize = 18;

    public List<Item> hotbarItems = new List<Item>();
    public List<Item> inventoryItems = new List<Item>();

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one Inventory instance found!");
            return;
        }
        instance = this;

        // Initialize slots
        for (int i = 0; i < hotbarSize; i++) hotbarItems.Add(null);
        for (int i = 0; i < inventorySize; i++) inventoryItems.Add(null);
    }

    // Add item to inventory first, then hotbar if possible
    public bool Add(Item item)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i] == null)
            {
                inventoryItems[i] = item;
                Debug.Log(item.itemName + " added to inventory slot " + i);
                return true;
            }
        }

        for (int i = 0; i < hotbarItems.Count; i++)
        {
            if (hotbarItems[i] == null)
            {
                hotbarItems[i] = item;
                Debug.Log(item.itemName + " added to hotbar slot " + i);
                return true;
            }
        }

        Debug.Log("Inventory Full! Could not add " + item.itemName);
        return false;
    }

    public void Remove(Item item)
    {
        if (hotbarItems.Contains(item))
        {
            int index = hotbarItems.IndexOf(item);
            hotbarItems[index] = null;
            Debug.Log(item.itemName + " removed from hotbar slot " + index);
        }
        else if (inventoryItems.Contains(item))
        {
            int index = inventoryItems.IndexOf(item);
            inventoryItems[index] = null;
            Debug.Log(item.itemName + " removed from inventory slot " + index);
        }
    }
}
