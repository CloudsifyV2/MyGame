using UnityEngine;

public class PlayerSave : MonoBehaviour
{
    public void SavePlayer()
    {
        PlayerData data = new PlayerData();

        // Save position
        data.posX = transform.position.x;
        data.posY = transform.position.y;
        data.posZ = transform.position.z;

        // Save rotation (Y only)
        data.rotY = transform.eulerAngles.y;

        // Save inventory
        data.hotbarItems = Inventory.instance.hotbarItems;
        data.inventoryItems = Inventory.instance.inventoryItems;

        // Save health, gamemode, etc.
        data.health = 20f;
        data.gamemode = "Survival";

        SaveSystem.Save(data, "player");
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.Load<PlayerData>("player");

        if (data == null) return;

        // Restore position
        transform.position = new Vector3(data.posX, data.posY, data.posZ);

        // Restore rotation
        transform.eulerAngles = new Vector3(0, data.rotY, 0);

        // Restore inventory
        Inventory.instance.hotbarItems = data.hotbarItems;
        Inventory.instance.inventoryItems = data.inventoryItems;
    }
}
