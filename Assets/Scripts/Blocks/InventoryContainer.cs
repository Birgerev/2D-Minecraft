﻿using System;
using UnityEngine;

public class InventoryContainer : Block
{
    public Inventory inventory;
    public override bool autosave { get; } = true;
    public virtual Type inventoryType { get; } = typeof(Inventory);
    public override bool autoTick { get; } = true;

    public override void Initialize()
    {
        base.Initialize();

        if (GetData().GetTag("inventory") != "")
            inventory = (Inventory) JsonUtility.FromJson(GetData().GetTag("inventory"), inventoryType);
        else inventory = (Inventory) Activator.CreateInstance(inventoryType);
    }

    public override void Tick()
    {
        SetData(GetData().SetTag("inventory", JsonUtility.ToJson(Convert.ChangeType(inventory, inventoryType))));

        base.Tick();
    }

    public override void Break(bool drop)
    {
        inventory.DropAll(location);

        base.Break(drop);
    }

    public override void Autosave()
    {
        SetData(GetData().SetTag("inventory", JsonUtility.ToJson(Convert.ChangeType(inventory, inventoryType))));

        if (inventory == null || inventory.open) return;

        base.Autosave();
    }

    public override void Interact()
    {
        base.Interact();

        inventory.Open(location);
    }
}