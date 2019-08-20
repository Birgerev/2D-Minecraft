﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Red_Flower : Block
{
    public static string default_texture = "block_red_flower";
    public override bool playerCollide { get; } = false;
    public override float breakTime { get; } = 0.3f;
    public override bool requiresGround { get; } = true;

    public override void Tick()
    {
        base.Tick();
    }
}