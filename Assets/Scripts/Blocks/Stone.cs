﻿public class Stone : Block
{
    public override string texture { get; set; } = "block_stone";
    public override float breakTime { get; } = 6;

    public override Tool_Type propperToolType { get; } = Tool_Type.Pickaxe;
    public override Tool_Level propperToolLevel { get; } = Tool_Level.Wooden;
    public override Block_SoundType blockSoundType { get; } = Block_SoundType.Stone;

    public override ItemStack GetDrop()
    {
        return new ItemStack(Material.Cobblestone, 1);
    }
}