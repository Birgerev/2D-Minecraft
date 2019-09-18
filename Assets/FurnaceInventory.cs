﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FurnaceInventory : Inventory
{
    public static bool open { get { return InventoryMenuManager.instance.furnaceInventoryMenu.active; } }

    public float highestFuel = 0;
    public float fuelLeft = 0;
    public int smeltingProgress = 0;
    
    public FurnaceInventory()
    {
        initialize(3, "Furnace");
    }
    
    public int getFuelSlot()
    {
        return 0;
    }

    public int getIngredientSlot()
    {
        return 1;
    }

    public int getResultSlot()
    {
        return 2;
    }

    public override void ToggleOpen()
    {
        FurnaceInventoryMenu inventory = InventoryMenuManager.instance.furnaceInventoryMenu;
        inventory.active = !inventory.active;
        inventory.inventory = this;
    }
}