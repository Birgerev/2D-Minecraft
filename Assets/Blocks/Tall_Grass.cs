﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tall_Grass : Block
{
    public static string default_texture = "block_tall_grass";
    public override bool playerCollide { get; } = false;
    public override float breakTime { get; } = 0.3f;
    public override bool requiresGround { get; } = true;

    public override void Tick()
    {
        base.Tick();
    }
}