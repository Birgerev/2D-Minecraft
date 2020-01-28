﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oak_Plank_Stairs : Stairs
{
    public static string default_texture = "block_Oak_Plank_Stairs";
    public override float breakTime { get; } = 3f;

    public override Tool_Type propperToolType { get; } = Tool_Type.Axe;
}