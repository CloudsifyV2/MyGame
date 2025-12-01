using UnityEngine;
using UnityEngine.UI;
using MyGame.Player;
using System.Collections;

namespace MyGame.GameModeManager.Survival
{
    public class HeartManagement : MonoBehaviour
    {
        [Header("Heart Sprites")]
        public Sprite fullHeart;
        public Sprite halfHeart;
        public Sprite emptyHeart;

        [Header("Heart UI Images (10)")]
        public Image[] heartImages; // Heart0–Heart9

        [Header("Regen Settings")]
        public float regenInterval = 1.0f;
        private float regenTimer = 0f;

        private bool initialized = false;
        private int lastHealth = -1; // Track last health to update only on change

        private IEnumerator Start()
        {
            // Wait until PlayerData exists and is initialized
            while (PlayerData.Instance == null || !PlayerData.Instance.initialized)
            {
                Debug.Log("HeartManagement waiting for PlayerData initialization...");
                yield return null;
            }

            Debug.Log("PlayerData initialized. HeartManagement proceeding.");

            // Now gameMode is guaranteed to be applied
            if (PlayerData.Instance.gameMode == null || PlayerData.Instance.gameMode.modeName != "Survival")
            {
                Debug.Log("HeartManagement disabled: Not in Survival mode. Current mode: " + 
                           (PlayerData.Instance.gameMode != null ? PlayerData.Instance.gameMode.modeName : "None"));
                enabled = false;
                yield break;
            }

            // Safety check: heart images and sprites
            if (heartImages == null || heartImages.Length == 0)
            {
                Debug.LogWarning("Heart images not assigned.");
                enabled = false;
                yield break;
            }

            if (fullHeart == null || halfHeart == null || emptyHeart == null)
            {
                Debug.LogWarning("Heart sprites not assigned.");
                enabled = false;
                yield break;
            }

            // Initialize UI
            UpdateHearts(PlayerData.Instance.health);
            lastHealth = PlayerData.Instance.health;
            initialized = true;
        }


        private void Update()
        {
            if (!initialized || PlayerData.Instance == null || PlayerData.Instance.gameMode == null)
                return;

            if (PlayerData.Instance.gameMode.modeName != "Survival")
                return;

            // Only update hearts if health changed
            if (PlayerData.Instance.health != lastHealth)
            {
                UpdateHearts(PlayerData.Instance.health);
                lastHealth = PlayerData.Instance.health;
            }

            // Optional: Uncomment to handle health regeneration
            // HandleRegen();
        }

        // -------------------------------
        // DAMAGE
        // -------------------------------
        public void ApplyDamage(int amount)
        {
            if (PlayerData.Instance == null)
                return;

            PlayerData.Instance.health = Mathf.Clamp(PlayerData.Instance.health - amount, 0, 100);
            UpdateHearts(PlayerData.Instance.health);
            lastHealth = PlayerData.Instance.health;
        }

        // -------------------------------
        // REGEN AT 85.5% FOOD
        // -------------------------------
        // private void HandleRegen()
        // {
        //     if (PlayerData.Instance == null)
        //         return;

        //     // Example hunger check
        //     if (PlayerData.Instance.foodLevel < 85.5f)
        //     {
        //         regenTimer = 0f;
        //         return;
        //     }

        //     regenTimer += Time.deltaTime;

        //     if (regenTimer >= regenInterval)
        //     {
        //         regenTimer = 0f;

        //         PlayerData.Instance.health = Mathf.Clamp(PlayerData.Instance.health + 1, 0, 100);

        //         // Only update if health changed
        //         if (PlayerData.Instance.health != lastHealth)
        //         {
        //             UpdateHearts(PlayerData.Instance.health);
        //             lastHealth = PlayerData.Instance.health;
        //         }
        //     }
        // }

        // -------------------------------
        // UPDATE HEART DISPLAY
        // -------------------------------
        private void UpdateHearts(int currentHealth)
        {
            if (heartImages == null)
                return;

            for (int i = 0; i < heartImages.Length; i++)
            {
                if (heartImages[i] == null)
                {
                    Debug.LogWarning($"Heart slot {i} is not assigned!");
                    continue;
                }

                int heartHP = (i + 1) * 10;

                if (currentHealth >= heartHP)
                    heartImages[i].sprite = fullHeart;
                else if (currentHealth >= heartHP - 5)
                    heartImages[i].sprite = halfHeart;
                else
                    heartImages[i].sprite = emptyHeart;
            }
        }
    }
}
