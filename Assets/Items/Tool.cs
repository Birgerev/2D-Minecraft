﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : Item
{
    public virtual Tool_Type tool_type { get; } = Tool_Type.None;
    public virtual Tool_Level tool_level { get; } = Tool_Level.None;

    public override void InteractLeft(Location loc, bool firstFrameDown)
    {
        Block block = loc.GetBlock();

        if (block != null)
        {
            block.Hit(1 / Player.blockHitsPerPerSecond, tool_type, tool_level);
        }
    }
    
    public override void InteractRight(Location loc, bool firstFrameDown)
    {
        if (tool_type == Tool_Type.Hoe && (loc.GetMaterial() == Material.Grass || loc.GetMaterial() == Material.Dirt))
        {
            loc.SetMaterial(Material.Farmland_Dry);
        }
        
        base.InteractRight(loc, firstFrameDown);
    }
}
public enum Tool_Type
{
    None,
    Pickaxe,
    Axe,
    Shovel,
    Hoe,
    Sword
}
public enum Tool_Level
{
    None = 1,
    Wooden = 2,
    Stone = 3,
    Gold = 4,
    Iron = 5,
    Diamond = 6
}