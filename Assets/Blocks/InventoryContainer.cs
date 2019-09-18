﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryContainer : Block
{
    public Inventory inventory;
    public override bool autosave { get; } = true;
    public virtual System.Type inventoryType { get; } = typeof(Inventory);

    public override void FirstTick()
    {
        base.FirstTick();
        
        if (data.ContainsKey("inventory") && data["inventory"] != "")
        {
            inventory = (Inventory)JsonUtility.FromJson(data["inventory"], inventoryType);
        }
        else inventory = (Inventory)System.Activator.CreateInstance(inventoryType);

    }

    public override void Tick()
    {
        base.Tick();
    }

    public override void Autosave()
    {
        data["inventory"] = JsonUtility.ToJson(System.Convert.ChangeType(inventory, inventoryType));

        base.Autosave();
    }

    public override void Interact()
    {
        base.Interact();

        inventory.ToggleOpen();
    }

    private static T CastAs<T>(object obj) where T : class, new()
    {
        return obj as T;
    }

}