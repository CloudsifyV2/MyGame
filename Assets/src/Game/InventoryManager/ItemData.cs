using UnityEngine;

namespace Game.InventoryManager
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public Sprite itemIcon;

        public bool isStackable;
        public int maxStackSize = 4;
    }
}