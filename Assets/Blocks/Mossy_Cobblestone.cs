﻿public class Mossy_Cobblestone : Block
{
    public static string default_texture = "block_mossy_cobblestone";
    public override float breakTime { get; } = 6;

    public override Tool_Type propperToolType { get; } = Tool_Type.Pickaxe;
    public override Tool_Level propperToolLevel { get; } = Tool_Level.Wooden;
    public override Block_SoundType blockSoundType { get; } = Block_SoundType.Stone;
}