﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Air : Block
{
    public static string default_texture = "block_air";
    public override float breakTime { get; } = 0f;

    public override void Tick()
    {
        base.Tick();
    }
}