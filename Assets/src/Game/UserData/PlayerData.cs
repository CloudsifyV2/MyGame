using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public float posX;
    public float posY;
    public float posZ;

    public float rotY;

    public float health;

    public List<Item> hotbarItems;
    public List<Item> inventoryItems;

    public string gamemode;  // per-player gamemode override cause why not
}
