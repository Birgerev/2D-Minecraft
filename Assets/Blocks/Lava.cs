﻿public class Lava : Liquid
{
    public static string default_texture = "block_lava";

    public override int max_liquid_level { get; } = 4;
    public override int glowLevel { get; } = 15;
}