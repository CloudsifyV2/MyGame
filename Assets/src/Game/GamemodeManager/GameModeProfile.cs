using UnityEngine;

[CreateAssetMenu(menuName = "MyGame/Gamemode")]
public class GameModeProfile : ScriptableObject
{
    public string modeName;

    public bool takesDamage = true;
    public bool hasHunger = true;
    public bool hasThirst = true;

    public bool canBreakBlocks = true;
    public bool canFly = false;

    public int maxHealth = 20;
}
