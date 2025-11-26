using UnityEngine;
using UnityEngine.UI;

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
            Item item = inventory.hotbarItems[i];

            if (item != null)
                hotbarSlots[i].AddItem(item);   // show icon
            else
                hotbarSlots[i].ClearSlot();      // empty slot
        }
    }
}
