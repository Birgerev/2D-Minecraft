﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : Block
{
    public override bool playerCollide { get; } = false;
    
    public override float breakTime { get; } = 3f;
    
    public override Tool_Type propperToolType { get; } = Tool_Type.Axe;
    public override Block_SoundType blockSoundType { get; } = Block_SoundType.Wood;
    
    public override void Initialize()
    {
        base.Initialize();
        
        if (data.ContainsKey("leaf_texture"))
        {
            bool leaf_texture = (data["leaf_texture"] == "false") ? false : true;
            if (leaf_texture)
            {
                texture = "block_logged_leaves";
                Render();
            }
        }
    }
}