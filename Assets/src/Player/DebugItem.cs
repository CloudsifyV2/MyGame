using UnityEngine;

namespace Game.InventoryManager
{
    public class TestItemAdder : MonoBehaviour
    {
        public GameObject inventory;
        public ItemData testItem;

        void Start()
        {
            inventory.GetComponent<Inventory>().AddItem(testItem, 1);
        }
    }
}