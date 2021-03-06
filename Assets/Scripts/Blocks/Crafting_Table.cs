﻿using System;

public class Crafting_Table : InventoryContainer
{
    public override string texture { get; set; } = "block_crafting_table";
    public override bool solid { get; set; } = false;
    public override float breakTime { get; } = 3;

    public override Tool_Type propperToolType { get; } = Tool_Type.Axe;
    public override Block_SoundType blockSoundType { get; } = Block_SoundType.Wood;

    public override Type inventoryType { get; } = typeof(CraftingInventory);

    private void Update()
    {
        if(getInventory() != null)
            if(getInventory().open)
                CheckCraftingRecipes();
    }

    public void CheckCraftingRecipes()
    {
        CraftingRecipe curRecipe = CraftingRecipe.FindRecipeByItems(getInventory().GetCraftingTable());

        if (curRecipe == null)
        {
            getInventory().setItem(getInventory().GetCraftingResultSlot(), new ItemStack());
            return;
        }

        getInventory().setItem(getInventory().GetCraftingResultSlot(), curRecipe.result);
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