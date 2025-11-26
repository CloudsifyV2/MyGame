using UnityEngine;
using MyGame.Player;

namespace MyGame.GameModeManager.Survival
{    
    public class HeartManagement : MonoBehaviour {

        void Start()
        {
            if (PlayerData.Instance.gameMode.modeName == "Survival")
            {
                // The player is playing in survival so we need to start the heart stuff.


                // TODO: Implement heart management for survival mode make sure hearts can be lost
                // when the player takes damage. Implement health regen when food is 85.5%.
            }
        }
        
    }
}