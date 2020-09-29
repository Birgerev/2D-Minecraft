﻿using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public static bool active;

    private float deltaTime;
    public Text text_biome;
    public Text text_dimension;
    public Text text_entityCount;

    public Text text_fps;
    public Text text_seed;
    public Text text_time;
    public Text text_x;
    public Text text_y;

    // Update is called once per frame
    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        updateState();

        if (active)
            updateText();
    }

    private void updateState()
    {
        if (Input.GetKeyDown(KeyCode.F3))
            active = !active;

        GetComponent<CanvasGroup>().alpha = active ? 1 : 0;
        GetComponent<CanvasGroup>().interactable = active;
        GetComponent<CanvasGroup>().blocksRaycasts = active;
    }

    private void updateText()
    {
        text_fps.text = "fps: " + (int) (1.0f / deltaTime);

        text_entityCount.text = "entity count: " + Entity.entityCount + ",  living: " + Entity.livingEntityCount;

        var player = Player.localInstance;
        text_x.text = "x: " + player.transform.position.x;
        text_y.text = "y: " + player.transform.position.y;
        text_dimension.text = "dimension: " + player.location.dimension;
        var biome = new ChunkPosition(player.location).GetChunk().getBiome();
        if (biome != null)
            text_biome.text = "biome: " + biome.name;

        text_seed.text = "seed: " + WorldManager.world.seed;
        text_time.text = "time: " + (int) WorldManager.world.time + ", (day " +
                         (int) (WorldManager.world.time / WorldManager.dayLength) + ")";
    }
}