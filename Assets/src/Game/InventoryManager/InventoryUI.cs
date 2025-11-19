using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Game.InventoryManager
{
    public class InventoryUI : MonoBehaviour
    {
        public Inventory inventory;
        public GameObject slotPrefab;
        public Transform slotParent;

        private void Start()
        {
            DrawInventory();
        }

        void DrawInventory()
        {
            foreach (Transform child in slotParent)
                Destroy(child.gameObject);

            foreach (var slot in inventory.slots)
            {
                var go = Instantiate(slotPrefab, slotParent);
                var icon = go.transform.Find("Icon").GetComponent<Image>();
                var amountText = go.transform.Find("Amount").GetComponent<Text>();

                if (!slot.isEmpty)
                {
                    icon.sprite = slot.item.itemIcon;
                    icon.enabled = true;
                    amountText.text = slot.amount > 1 ? slot.amount.ToString() : "";
                }
                else
                {
                    icon.enabled = false;
                    amountText.text = "";
                }
            }
        }
    }
}
