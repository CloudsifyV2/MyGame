using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Text countText;

    private Item item;
    private int count = 0;

    public void AddItem(Item newItem, int newCount)
    {
        item = newItem;
        count = newCount;
        icon.sprite = item.icon;
        icon.enabled = true;
        countText.text = count > 1 ? count.ToString() : "";
    }

    public void ClearSlot()
    {
        item = null;
        count = 0;
        icon.sprite = null;
        icon.enabled = false;
        if (countText != null) countText.text = "";
    }

    public bool HasItem()
    {
        return item != null;
    }
}
