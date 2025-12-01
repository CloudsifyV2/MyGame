using System;
using UnityEngine;

[Serializable]
public class InventoryStack
{
    public Item item;
    public int count;

    public InventoryStack(Item item = null, int count = 0)
    {
        this.item = item;
        this.count = count;
    }
}
