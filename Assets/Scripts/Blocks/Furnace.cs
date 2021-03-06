﻿using System;

public class Furnace : InventoryContainer
{
    public override string texture { get; set; } = "block_furnace";
    public override bool solid { get; set; } = false;
    public override float breakTime { get; } = 6;

    public override Tool_Type propperToolType { get; } = Tool_Type.Pickaxe;
    public override Tool_Level propperToolLevel { get; } = Tool_Level.Wooden;
    public override Block_SoundType blockSoundType { get; } = Block_SoundType.Stone;

    public override Type inventoryType { get; } = typeof(FurnaceInventory);

    public override void Tick()
    {
        if (inventory == null)
        {
            base.Tick();
            return;
        }

        CheckFuels();
        SmeltTick();

        base.Tick();
    }

    public void CheckFuels()
    {
        if (getInventory().fuelLeft <= 0)
            if (getInventory().getItem(getInventory().getFuelSlot()) != null)
                if (SmeltingRecipe.Fuels.ContainsKey(getInventory().getItem(getInventory().getFuelSlot()).material))
                    if (GetRecepie() != null)
                    {
                        getInventory().fuelLeft =
                            SmeltingRecipe.Fuels[getInventory().getItem(getInventory().getFuelSlot()).material];
                        getInventory().highestFuel = getInventory().fuelLeft;
                        getInventory().getItem(getInventory().getFuelSlot()).amount--;
                    }
    }

    public void SmeltTick()
    {
        var curRecepie = GetRecepie();

        if (getInventory().fuelLeft <= 0)
            getInventory().highestFuel = 0;

        if (curRecepie != null && getInventory().getItem(getInventory().getIngredientSlot()).amount > 0 &&
            (getInventory().getItem(getInventory().getResultSlot()).material == curRecepie.result.material ||
             getInventory().getItem(getInventory().getResultSlot()).material == Material.Air))
        {
            //subtract fuel
            if (getInventory().fuelLeft > 0)
                getInventory().fuelLeft--;
            else return;

            //Add progress to smeltbar
            getInventory().smeltingProgress += 1 / Chunk.TickRate;

            //If smelting is done, give result
            if (getInventory().smeltingProgress >= SmeltingRecipe.smeltTime)
                FillSmeltingResult();
        }
        else
        {
            //Continiue to deplete fuel if we've already begun smelting but ingredient item was removed
            if (getInventory().fuelLeft > 0)
                getInventory().fuelLeft--;

            //Reset smelting progress if item is removed
            getInventory().smeltingProgress = 0;
        }
    }

    public void FillSmeltingResult()
    {
        //Called once smelting is done 
        var curRecepie = GetRecepie();

        getInventory().getItem(getInventory().getResultSlot()).material = curRecepie.result.material;
        getInventory().getItem(getInventory().getResultSlot()).amount += curRecepie.result.amount;
        getInventory().getItem(getInventory().getIngredientSlot()).amount--;

        getInventory().smeltingProgress = 0;
    }

    public SmeltingRecipe GetRecepie()
    {
        if (getInventory().getItem(getInventory().getIngredientSlot()).amount <= 0)
            return null;
        //Get recepie based on contents of ingredient slot
        return SmeltingRecipe.FindRecipeByIngredient(getInventory().getItem(getInventory().getIngredientSlot())
            .material);
    }

    private FurnaceInventory getInventory()
    {
        return (FurnaceInventory) inventory;
    }
}