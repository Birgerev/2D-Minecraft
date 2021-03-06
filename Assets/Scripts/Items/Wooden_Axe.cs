﻿public class Wooden_Axe : Tool
{
    public override string texture { get; set; } = "item_wooden_axe";
    public override Tool_Type tool_type { get; } = Tool_Type.Axe;
    public override Tool_Level tool_level { get; } = Tool_Level.Wooden;
    public override int maxDurabulity { get; } = 59;
}