﻿public class Coal_Ore : Block
{
    public override string texture { get; set; } = "block_coal_ore_0";
    public override string[] alternative_textures { get; } = {"block_coal_ore_0", "block_coal_ore_1"};

    public override float breakTime { get; } = 6;

    public override Tool_Type propperToolType { get; } = Tool_Type.Pickaxe;
    public override Tool_Level propperToolLevel { get; } = Tool_Level.Wooden;
    public override Block_SoundType blockSoundType { get; } = Block_SoundType.Stone;

    public override ItemStack GetDrop()
    {
        return new ItemStack(Material.Coal, 1);
    }
}