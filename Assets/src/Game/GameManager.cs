using UnityEngine;

namespace MyGame.Game
{
    public class GameManager : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] private GameObject prefab;

        void Start()
        {
            Instantiate(prefab, new Vector3(0, 1, 0), Quaternion.identity);
        }
    }
}