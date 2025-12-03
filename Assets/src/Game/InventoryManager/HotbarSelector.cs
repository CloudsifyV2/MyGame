using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MyGame.Inventory
{
    public class HotbarSelector : MonoBehaviour
    {
        public int activeSlot = 0;
        public Transform hotbarPanel;

        public Sprite normalSlotSprite;
        public Sprite selectedSlotSprite;

        // Singleton instance so other systems can query selected item
        public static HotbarSelector instance;

        void Awake()
        {
            instance = this;
        }

        void OnDestroy()
        {
            if (instance == this) instance = null;
        }

        // Returns the Item currently selected in the hotbar (or null)
        public Item GetSelectedItem()
        {
            if (Inventory.instance == null) return null;
            if (activeSlot < 0 || activeSlot >= Inventory.instance.hotbarItems.Count) return null;
            var stack = Inventory.instance.hotbarItems[activeSlot];
            return stack != null ? stack.item : null;
        }

        void Update()
        {
            HandleScroll();
            HandleNumberKeys();
            HighlightSlot();
        }

        void HandleScroll()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            float scroll = mouse.scroll.ReadValue().y;

            if (scroll > 0) activeSlot--;
            if (scroll < 0) activeSlot++;

            if (activeSlot < 0) activeSlot = 8;
            if (activeSlot > 8) activeSlot = 0;
        }

        void HandleNumberKeys()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.digit1Key.wasPressedThisFrame) activeSlot = 0;
            if (keyboard.digit2Key.wasPressedThisFrame) activeSlot = 1;
            if (keyboard.digit3Key.wasPressedThisFrame) activeSlot = 2;
            if (keyboard.digit4Key.wasPressedThisFrame) activeSlot = 3;
            if (keyboard.digit5Key.wasPressedThisFrame) activeSlot = 4;
            if (keyboard.digit6Key.wasPressedThisFrame) activeSlot = 5;
            if (keyboard.digit7Key.wasPressedThisFrame) activeSlot = 6;
            if (keyboard.digit8Key.wasPressedThisFrame) activeSlot = 7;
            if (keyboard.digit9Key.wasPressedThisFrame) activeSlot = 8;
        }

        void HighlightSlot()
        {
            for (int i = 0; i < hotbarPanel.childCount; i++)
            {
                Image slotImage = hotbarPanel.GetChild(i).GetComponent<Image>();

                if (i == activeSlot)
                    slotImage.sprite = selectedSlotSprite;
                else
                    slotImage.sprite = normalSlotSprite;
            }
        }
    }
}