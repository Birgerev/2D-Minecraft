using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldGenerator
{
    public virtual Material GenerateTerrainBlock(Location loc)
    {
        return Material.TNT;
    }

    public virtual void GenerateStructures(Location loc)
    {
        
    }
}
