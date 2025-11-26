using UnityEngine;

namespace MyGame.Player
{
    public class PlayerData : MonoBehaviour
    {
        public static PlayerData Instance;

        [Header("Player Stats")]
        public int health;
        public int level;
        public int foodLevel;
        public int thirstLevel;

        [Header("Gamemode")]
        public GameModeProfile gameMode;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            ApplyGamemode();
        }

        public void ApplyGamemode()
        {
            if (gameMode == null) return;

            health = gameMode.maxHealth;

            // Example usage of rules:
            if (!gameMode.hasHunger) foodLevel = 100;
            if (!gameMode.hasThirst) thirstLevel = 100;
        }
    }
}
