﻿public class Wooden_Door_Bottom : Wooden_Door_Block
{
    public static string default_texture = "block_wooden_door_bottom_close";
    public override bool rotate_x { get; } = true;

    public override string open_texture { get; } = "block_wooden_door_bottom_open";
    public override string closed_texture { get; } = "block_wooden_door_bottom_close";
}