﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed_Top : Bed
{
    public static string default_texture = "block_bed_top";
    
    public override ItemStack GetDrop()
    {
        return new ItemStack(Material.Bed_Bottom, 1);
    }
}