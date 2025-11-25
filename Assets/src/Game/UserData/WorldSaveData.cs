using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    public WorldData worldData;

    private void Awake()
    {
        instance = this;
    }

    public void SaveWorld()
    {
        SaveSystem.Save(worldData, "world");
    }

    public void LoadWorld()
    {
        WorldData data = SaveSystem.Load<WorldData>("world");

        if (data != null)
        {
            worldData = data;
        }
        else
        {
            // New world defaults
            worldData = new WorldData();
            worldData.worldName = "MyWorld";
            worldData.difficulty = "Normal";
            worldData.gamemode = "Survival";
        }
    }
}
