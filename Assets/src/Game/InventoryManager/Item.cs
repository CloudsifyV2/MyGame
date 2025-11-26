using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string id; // Unique identifier for the item e.g "wooden_sword" or "crafting_bench"
    public string itemName;
    public Sprite icon;
    public bool stackable;
    public int maxStackSize;
    public string category;
    // Add more properties as needed (stackable, etc.)
}
