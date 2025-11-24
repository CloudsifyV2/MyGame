using UnityEngine;

namespace MyGame.Game
{
    public class GameManager : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] private GameObject prefab;
        [Tooltip("Spawn X,Z coordinates in world space")] 
        [SerializeField] private Vector2 spawnXZ = Vector2.zero;

        void Start()
        {
            StartCoroutine(WaitAndSpawn());
        }

        private System.Collections.IEnumerator WaitAndSpawn()
        {
            // Try to find World instance
            var world = Object.FindFirstObjectByType<MyGame.WorldManager.World>();

            if (world != null)
            {
                // Ask world to start generating around the spawn position (in case player not present yet)
                world.GenerateAroundPosition(spawnXZ.x, spawnXZ.y, Mathf.Max(1, world.renderDistance));
                // Wait until chunks around spawn are generated (use world's renderDistance)
                yield return StartCoroutine(world.WaitForChunksAt(spawnXZ.x, spawnXZ.y, world.renderDistance, 15f));
            }

            // Determine surface height from terrain generator and spawn slightly above it
            int wx = Mathf.FloorToInt(spawnXZ.x);
            int wz = Mathf.FloorToInt(spawnXZ.y);
            int height = MyGame.WorldManager.TerrainGenerator.GetHeight(wx, wz);

            Vector3 spawnPos = new Vector3(spawnXZ.x, height + 2f, spawnXZ.y);
            var go = Instantiate(prefab, spawnPos, Quaternion.identity);
            if (go != null) go.tag = "Player";
        }
    }
}