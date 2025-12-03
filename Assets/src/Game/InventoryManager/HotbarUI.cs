using UnityEngine;
using UnityEngine.UI;

namespace MyGame.Inventory
{
    public class HotbarUI : MonoBehaviour
{
    public InventorySlot[] hotbarSlots;   // 9 UI slots
    private Inventory inventory;

    void Start()
    {
        inventory = Inventory.instance;
        UpdateHotbar();
    }

    void Update()
    {
        UpdateHotbar();   // update every frame OR call only on change
    }

    public void UpdateHotbar()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            var stack = inventory.hotbarItems[i];

            if (stack != null && stack.item != null)
                hotbarSlots[i].AddItem(stack.item, stack.count);   // show icon + count
            else
                hotbarSlots[i].ClearSlot();      // empty slot
        }
    }
}
}