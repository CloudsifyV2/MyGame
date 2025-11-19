namespace Game.InventoryManager
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemData item;
        public int amount;

        public bool isEmpty => item == null;

        public InventorySlot() => ClearSlot();

        public void ClearSlot()
        {
            item = null;
            amount = 0;
        }

        public bool CanItemBeAdded(ItemData newItem)
        {
            return !isEmpty && item == newItem && item.isStackable && amount < item.maxStackSize;
        }

        public void AddItem(ItemData newItem, int count = 1)
        {
            if (isEmpty)
            {
                item = newItem;
                amount = count;
            }
            else if (CanItemBeAdded(newItem))
            {
                amount += count;
            }
        }

    }
}