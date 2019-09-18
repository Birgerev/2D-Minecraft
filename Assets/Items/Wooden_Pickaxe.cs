﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wooden_Pickaxe : Tool
{
    public override Tool_Type tool_type { get; } = Tool_Type.Pickaxe;
    public override Tool_Level tool_level { get; } = Tool_Level.Wooden;

    public static string default_texture = "item_wooden_pickaxe";
}