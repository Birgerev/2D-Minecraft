﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planks : Block
{
    public static string default_texture = "block_planks";
    public override float breakTime { get; } = 3f;

    public override void Tick()
    {
        base.Tick();
    }
}