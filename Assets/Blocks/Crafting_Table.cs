﻿using System;

public class Crafting_Table : InventoryContainer
{
    public static string default_texture = "block_crafting_table";
    public override bool playerCollide { get; } = false;
    public override float breakTime { get; } = 3;

    public override Tool_Type propperToolType { get; } = Tool_Type.Axe;
    public override Block_SoundType blockSoundType { get; } = Block_SoundType.Wood;

    public override Type inventoryType { get; } = typeof(CraftingInventory);

    public override void Tick()
    {
        base.Tick();

        CheckCraftingRecepies();
    }

    public void CheckCraftingRecepies()
    {
        var curRecepie = CraftingRecepie.FindRecepieByItems(getInventory().getCraftingTable());

        if (curRecepie == null)
        {
            getInventory().setItem(getInventory().getCraftingResultSlot(), new ItemStack());
            return;
        }

        getInventory().setItem(getInventory().getCraftingResultSlot(), curRecepie.result);
    }

    public override void Interact()
    {
        var newInv = new CraftingInventory();
        inventory = newInv;
        newInv.Open(location);
    }

    private CraftingInventory getInventory()
    {
        return (CraftingInventory) inventory;
    }
}