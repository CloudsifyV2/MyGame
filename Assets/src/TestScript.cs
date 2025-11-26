using UnityEngine;
using MyGame.Inventory;

namespace MyGame
{
    public class TestScript : MonoBehaviour
    {
        public Item testItem;

        void Start()
        {
            Inventory.instance.Add(testItem);
        }
    }
    }