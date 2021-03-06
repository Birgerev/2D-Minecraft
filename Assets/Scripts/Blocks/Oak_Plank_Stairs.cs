﻿public class Oak_Plank_Stairs : Stairs
{
    public override string texture { get; set; } = "block_oak_plank_stairs";
    public override float breakTime { get; } = 3f;
    public override bool isFlammable { get; } = true;

    public override Tool_Type propperToolType { get; } = Tool_Type.Axe;
    public override Block_SoundType blockSoundType { get; } = Block_SoundType.Wood;
}