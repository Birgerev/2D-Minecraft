﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;
using Debug = UnityEngine.Debug;
using Random = System.Random;

[BurstCompile]
public class Block : MonoBehaviour
{
    public static Dictionary<Block, int> lightSources = new Dictionary<Block, int>();
    public static HashSet<Block> sunlightSources = new HashSet<Block>();

    public string texture;
    public virtual string[] alternative_textures { get; } = { };
    public virtual float change_texture_time { get; } = 0;
    
    public virtual bool playerCollide { get; } = true;
    public virtual bool triggerCollider { get; } = false;
    public virtual bool requiresGround { get; } = false;
    public virtual bool autosave { get; } = false;
    public virtual bool autoTick { get; } = false;
    public virtual float averageRandomTickDuration { get; } = 0;
    public virtual float breakTime { get; } = 0.75f;
    public virtual bool rotate_x { get; } = false;
    public virtual bool rotate_y { get; } = false;

    public virtual Tool_Type propperToolType { get; } = Tool_Type.None;
    public virtual Tool_Level propperToolLevel { get; } = Tool_Level.None;
    
    public virtual Block_SoundType blockSoundType { get; } = Block_SoundType.Stone;

    public virtual int glowLevel { get; } = 0;

    public BlockData data = new BlockData();

    public float blockHealth = 0;

    public Location location;
    public int age = 0;

    private float time_of_last_hit = 0;
    private float time_of_last_autosave = 0;

    public void ScheduleBlockInitialization()
    {
        StartCoroutine(scheduleBlockInitialization());
    }

    IEnumerator scheduleBlockInitialization()
    {
        yield return new WaitForSeconds(0.02f);
        Initialize();
    }
    
    public void ScheduleBlockBuildTick()
    {
        StartCoroutine(scheduleBlockBuildTick());
    }

    IEnumerator scheduleBlockBuildTick()
    {
        yield return new WaitForSeconds(0.02f);
        BuildTick();
    }
    
    public virtual void Initialize()
    {
        //gameObject.name = "block [" + transform.position.x + "," + transform.position.y + "]";
        blockHealth = breakTime;

        texture = (string)this.GetType().
            GetField("default_texture", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).GetValue(null);


        UpdateColliders();

        RenderNoLight();

        //Cache position for use in multithreading
        location = Location.LocationByPosition(transform.position, location.dimension);
        
        checkGround();
        
        if (autoTick || autosave)
            StartCoroutine(autoTickLoop());
        if (averageRandomTickDuration != 0)
            StartCoroutine(randomTickLoop());
        
        Render();
    }

    public virtual void RandomTick()
    {
    }
    
    public virtual void BuildTick()
    {
        if ((rotate_x || rotate_y) && !(data.HasData("rotated_x") || data.HasData("rotated_y")))
        {
            print(data.GetSaveString() + " rotating " + GetMaterial());
            RotateTowardsPlayer();
        }
    }

    public virtual void GeneratingTick()
    {
    }

    public virtual void Tick()
    {
        if(age == 0 && new ChunkPosition(location).GetChunk().isLoaded)    //Block place sound
        {
            Sound.Play(location, "block/" + blockSoundType.ToString().ToLower() + "/break", SoundType.Blocks, 0.5f, 1.5f);
        }

        checkGround();
        UpdateColliders();
        RenderRotate();


        if (Time.time - time_of_last_autosave > SaveManager.AutosaveDuration && autosave)
        {
            Autosave();
            return;
        }

        age++;
    }

    private void checkGround()
    {
        if (requiresGround)
        {
            if ((location - new Location(0, 1)).GetMaterial() == Material.Air)
            {
                Break();
            }
        }
    }

    public float getRandomChance()
    {
        return (float)new System.Random(SeedGenerator.SeedByLocation(location) + age).NextDouble();
    }

    IEnumerator randomTickLoop()
    {
        Random r = new Random(SeedGenerator.SeedByLocation(location));
        
        while (true)
        {
            float nextTickDuration = 1;
            while (r.NextDouble() > 1 / averageRandomTickDuration)
            {
                nextTickDuration += 1;
            }
            
            yield return new WaitForSeconds(nextTickDuration);
            this.RandomTick();
        }
    }

    IEnumerator autoTickLoop()
    {
        //Wait a random duration, to smooth out ticks across time
        yield return new WaitForSeconds((float)new System.Random(SeedGenerator.SeedByLocation(location)).NextDouble() * (1f / Chunk.TickRate));

        while (true)
        {
            yield return new WaitForSeconds(1 / Chunk.TickRate);
            this.Tick();
        }
    }
    
    public static void UpdateSunlightSourceAt(int x, Dimension dimension)
    {
        Block topBlock = Chunk.getTopmostBlock(x, dimension);
        bool isDay = (WorldManager.world.time % WorldManager.dayLength) < (WorldManager.dayLength / 2);
        if (topBlock == null)
            return;

        //remove all sunlight sources in the same column
        foreach(Block source in sunlightSources.ToList())
        {
            if (source.location.x == x && source.location.dimension == dimension)
            {
                sunlightSources.Remove(source);
                lightSources.Remove(source);
                UpdateLightAround(source.location);
            }
        }

        //Add the new position
        sunlightSources.Add(topBlock);
        lightSources.Add(topBlock, isDay ? 15 : 5);
        UpdateLightAround(topBlock.location);
    }

    public bool IsSunlightSource()
    {
        return sunlightSources.Contains(this);
    }

    public bool CheckBlockLightSource()
    {
        if (glowLevel > 0)
        {
            lightSources[this] = glowLevel;
        }

        return GetLightSourceLevel(this) > 0;
    }
    
    public void RenderNoLight()
    {
        float brightnessColorValue = 0;
        GetComponent<SpriteRenderer>().color = new Color(brightnessColorValue, brightnessColorValue, brightnessColorValue);
    }

    public void RenderBlockLight(int lightLevel)
    {
        float brightnessColorValue = (float)lightLevel / 15f;
        GetComponent<SpriteRenderer>().color = new Color(brightnessColorValue, brightnessColorValue, brightnessColorValue);
    }

    public static void UpdateLightAround(Location loc)
    {
        Block source = (loc).GetBlock();
        if(source != null)
        {
            source.CheckBlockLightSource();    //TODO
        }
        
        Chunk chunk = new ChunkPosition(loc).GetChunk();
        if(chunk != null)
            chunk.lightSourceToUpdate.Add(loc);
    }

    public static int GetLightSourceLevel(Block block)
    {
        if (block == null)
            return 0;

        int blockLevel = block.glowLevel;
        if (blockLevel == 0 && block.IsSunlightSource())
        {
            bool isDay = (WorldManager.world.time % WorldManager.dayLength) < (WorldManager.dayLength / 2);
            blockLevel = (isDay ? 15 : 5);
        }

        return blockLevel;
    }

    public static int GetLightLevel(Location loc)
    {
        //Messy layout due to multithreading
        if (lightSources.Count <= 0 && sunlightSources.Count <= 0)
            return 0;
        
        List<Block> sources;
        lock (lightSources)
        {
            //clone actual list to avoid threading errors
            sources = new Dictionary<Block, int>(lightSources).Keys.ToList();
        }

        Location brightestSourceLoc = new Location(0, 0);
        int brightestValue = 0;
        
        foreach (Block source in sources)
        {
            Location sourceLoc = source.location;
            if (sourceLoc.dimension == loc.dimension)
            {
                int sourceBrightness = GetLightSourceLevel(source);
                int value = sourceBrightness - (int)(math.distance(sourceLoc.GetPosition(), loc.GetPosition()));

                if (value > brightestValue)
                {
                    brightestValue = value;
                    brightestSourceLoc = sourceLoc;
                }
            }
        }
        
        return brightestValue;
    }

    public virtual void UpdateColliders()
    {
        GetComponent<Collider2D>().enabled = (playerCollide || triggerCollider);
        GetComponent<Collider2D>().isTrigger = (triggerCollider);
    }

    public Color GetRandomColourFromTexture()
    {
        Texture2D texture = getTexture().texture;
        Color[] pixels = texture.GetPixels();
        System.Random random = new System.Random((DateTime.Now).GetHashCode());

        return pixels[random.Next(pixels.Length)];
    }

    public void RotateTowardsPlayer()
    {
        bool rotated_x = false;
        bool rotated_y = false;

        if (rotate_y)
        {
            rotated_y = (Player.localInstance.transform.position.y < location.y);
        }
        if (rotate_x)
        {
            rotated_x = (Player.localInstance.transform.position.x < location.x);
        }

        data.SetData("rotated_x", rotated_x ? "true" : "false");
        data.SetData("rotated_y", rotated_y ? "true" : "false");
        
        //Save new rotation
        Autosave();
    }

    public void RenderRotate()
    {
        bool rotated_x = false;
        bool rotated_y = false;

        rotated_x = (data.GetData("rotated_x") == "true");
        rotated_y = (data.GetData("rotated_y") == "true");

        GetComponent<SpriteRenderer>().flipX = rotated_x;
        GetComponent<SpriteRenderer>().flipY = rotated_y;
    }
    
    public virtual void Autosave()
    {
        time_of_last_autosave = Time.time;

        location.SetData(data);
    }

    public virtual void Hit(float time)
    {
        Hit(time, Tool_Type.None, Tool_Level.None);
    }

    public virtual void Hit(float time, Tool_Type tool_type, Tool_Level tool_level)
    {
        time_of_last_hit = Time.time;

        bool properToolStats = false;

        if(tool_level != Tool_Level.None && tool_type == propperToolType && tool_level >= propperToolLevel)
        {
            time *= 2 + ((float)tool_level * 2f);
        }
        if (propperToolLevel == Tool_Level.None ||
            (tool_type == propperToolType && tool_level >= propperToolLevel))
        {
            properToolStats = true;
        }

        blockHealth -= time;

        Sound.Play(location, "block/" + blockSoundType.ToString().ToLower() + "/hit", SoundType.Blocks, 0.8f, 1.2f);

        RenderBlockDamage();

        if (blockHealth <= 0)
        {
            if (properToolStats)
                Break();
            else
                Break(false);

            Player.localInstance.DoToolDurability();

            return;
        }

        RenderBlockDamage();
        StartCoroutine(repairOnceViable());
    }

    IEnumerator repairOnceViable()
    {
        while (Time.time - time_of_last_hit < 1)
        {
            yield return new WaitForSeconds(0.2f);
        }

        blockHealth = breakTime;
    }


    public virtual void Break()
    {
        Break(true);
    }

    public virtual void Break(bool drop)
    {
        if (drop)
            Drop();
        
        Sound.Play(location, "block/" + blockSoundType.ToString().ToLower() + "/break", SoundType.Blocks, 0.5f, 1.5f);

        System.Random r = new System.Random();
        for (int i = 0; i < r.Next(2, 8); i++)    //SpawnParticles
        {
            Particle part = (Particle)Entity.Spawn("Particle");

            part.transform.position = location.GetPosition() + new Vector2((float)r.NextDouble() - 0.5f, (float)r.NextDouble() - 0.5f);
            part.color = GetRandomColourFromTexture();
            part.doGravity = true;
            part.velocity = Vector2.zero;
            part.maxAge = 1f + (float)r.NextDouble();
            part.maxBounces = 10;
        }

        location.SetMaterial(Material.Air);
        location.Tick();
    }

    public virtual void Drop()
    {
        GetDrop().Drop(location);
    }

    public virtual ItemStack GetDrop()
    {
        return new ItemStack(GetMaterial(), 1);
    }

    public virtual void Interact()
    {
    }

    public void ResetBlockDamage()
    {
        blockHealth = breakTime;
        RenderBlockDamage();
    }

    public virtual void RenderBlockDamage()
    {

        Transform damageIndicator = transform.Find("BreakIndicator");

        if (blockHealth != breakTime)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Block_Break");
            int spriteIndex = (int)((blockHealth/breakTime) / (1f / ((float)sprites.Length)));

            BreakIndicator.instance.UpdateState(spriteIndex, location);
        }
    }

    public virtual void Render()
    {
        GetComponent<SpriteRenderer>().sprite = getTexture();
    }

    public virtual Sprite getTexture()
    {
        if (change_texture_time > 0 && alternative_textures.Length > 0)
        {
            float totalTimePerTextureLoop = change_texture_time * alternative_textures.Length;
            int textureIndex = (int)((Time.time % totalTimePerTextureLoop) / change_texture_time);

            return Resources.Load<Sprite>("Sprites/" + alternative_textures[textureIndex]);
        }
        else if(alternative_textures.Length > 0)
        {
            int textureIndex = new System.Random(SeedGenerator.SeedByLocation(location)).Next(0, alternative_textures.Length);

            return Resources.Load<Sprite>("Sprites/" + alternative_textures[textureIndex]);
        }
        else return Resources.Load<Sprite>("Sprites/" + texture);
    }

    public Material GetMaterial()
    {
        return (Material)System.Enum.Parse(typeof(Material), this.GetType().Name);
    }
}

public enum Block_SoundType
{
    Stone,
    Wood,
    Sand,
    Dirt,
    Grass,
    Wool,
    Gravel
}