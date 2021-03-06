﻿using System;
using System.Collections.Generic;

public class Pig : PassiveEntity
{
    //Entity Properties
    public override float maxHealth { get; } = 10;

    public override List<ItemStack> GetDrops()
    {
        //Drop a random amount of a certain item
        var result = new List<ItemStack>();
        var r = new Random(SeedGenerator.SeedByLocation(Location));

        result.Add(new ItemStack(Material.Raw_Porkchop, r.Next(1, 3 + 1)));

        return result;
    }

    public override EntityController GetController()
    {
        return new AnimalController(this);
    }
}