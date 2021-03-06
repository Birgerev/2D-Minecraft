﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftingRecipe
{
    private static CraftingRecipe[] _cachedRecipes;
    public Dictionary<Vector2Int, Material> recipeShape = new Dictionary<Vector2Int, Material>();

    public ItemStack result;
    public bool flipX;
    public bool flipY;

    public static CraftingRecipe[] allRecipes()
    {
        if (_cachedRecipes == null)
        {
            var files = Resources.LoadAll<TextAsset>("Recipes/Crafting");
            var recipes = new List<CraftingRecipe>();

            foreach (var file in files) 
                recipes.Add(FileToRecipe(file));

            _cachedRecipes = recipes.ToArray(); //Cache results

            return recipes.ToArray();
        }

        return _cachedRecipes;
    }

    public bool Compare(ItemStack[] items)
    {
        if (Compare(items, false, false))
            return true;
        
        if(flipX)
            if (Compare(items, true, false))
                return true;
        
        if(flipY)
            if (Compare(items, false, true))
                return true;
        
        if(flipX && flipY)
            if (Compare(items, true, true))
                return true;

        return false;
    }
    
    public bool Compare(ItemStack[] items, bool mirrorX, bool mirrorY)
    {
        var shape = new Dictionary<Vector2Int, Material>();

        //Get shape by item list
        for (var i = 0; i < items.Length; i++)
        {
            var curPos = Vector2Int.zero;
            var maxPos = Vector2Int.zero;

            if (items.Length == 4)
            {
                switch (i)
                {
                    case 0:
                        curPos = new Vector2Int(0, 1);
                        break;
                    case 1:
                        curPos = new Vector2Int(1, 1);
                        break;
                    case 2:
                        curPos = new Vector2Int(0, 0);
                        break;
                    case 3:
                        curPos = new Vector2Int(1, 0);
                        break;
                }

                maxPos = new Vector2Int(1, 1);
            }
            else if (items.Length == 9)
            {
                switch (i)
                {
                    case 0:
                        curPos = new Vector2Int(0, 2);
                        break;
                    case 1:
                        curPos = new Vector2Int(1, 2);
                        break;
                    case 2:
                        curPos = new Vector2Int(2, 2);
                        break;
                    case 3:
                        curPos = new Vector2Int(0, 1);
                        break;
                    case 4:
                        curPos = new Vector2Int(1, 1);
                        break;
                    case 5:
                        curPos = new Vector2Int(2, 1);
                        break;
                    case 6:
                        curPos = new Vector2Int(0, 0);
                        break;
                    case 7:
                        curPos = new Vector2Int(1, 0);
                        break;
                    case 8:
                        curPos = new Vector2Int(2, 0);
                        break;
                }

                maxPos = new Vector2Int(2, 2);
            }
            else Debug.LogError("Invalid Crating Table Size Of " + items.Length);
            
            //Mirroring
            if (mirrorX)
                curPos.x = maxPos.x - curPos.x;
            if (mirrorY)
                curPos.y = maxPos.y - curPos.y;
            
            if (items[i] != null && items[i].material != Material.Air) 
                shape.Add(curPos, items[i].material);
        }

        //Find How much the recepie is Offsetted by
        var lowest_x = 2;
        var lowest_y = 2;

        foreach (var shapeItem in shape)
        {
            if (shapeItem.Key.x < lowest_x) lowest_x = shapeItem.Key.x;
            if (shapeItem.Key.y < lowest_y) lowest_y = shapeItem.Key.y;
        }

        var keys = shape.Keys.ToArray();
        var shapeCopy = new Dictionary<Vector2Int, Material>(shape);
        shape.Clear();

        //Move To Lowest Slots
        foreach (var key in keys)
        {
            var mat = shapeCopy[key];

            shape.Add(key - new Vector2Int(lowest_x, lowest_y), mat);
        }

        //Compare both Dictionaries
        var anyDifferences = false;

        if (recipeShape.Count != shape.Count)
            anyDifferences = true;

        foreach (var recipeItem in recipeShape)
        {
            if (!shape.ContainsKey(recipeItem.Key))
            {
                anyDifferences = true;
                break;
            }

            if (shape[recipeItem.Key] != recipeItem.Value)
            {
                anyDifferences = true;
                break;
            }
        }

        return !anyDifferences;
    }

    public static CraftingRecipe FindRecipeByItems(ItemStack[] items)
    {
        foreach (var recipe in allRecipes())
            if (recipe.Compare(items))
                return recipe;
        return null;
    }

    public static CraftingRecipe FileToRecipe(TextAsset file)
    {
        var recipe = new CraftingRecipe();

        try
        {
            var lines = file.text.Split('\n');
            
            recipe.flipX = lines[0].Split('*')[1].Contains("1");
            recipe.flipY = lines[0].Split('*')[2].Contains("1");
            
            recipe.result = new ItemStack(
                (Material) Enum.Parse(typeof(Material), lines[1].Split('*')[0]),
                int.Parse(lines[1].Split('*')[1]), lines[1].Split('*')[2]);

            for (var i = 2; i < lines.Length; i++)
            {
                var mat = (Material) Enum.Parse(typeof(Material), lines[i].Split('*')[0]);
                var pos = new Vector2Int(
                    int.Parse(lines[i].Split('*')[1].Split(',')[0]),
                    int.Parse(lines[i].Split('*')[1].Split(',')[1]));

                recipe.recipeShape.Add(pos, mat);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("faulty crafting recipe \"" + file.name + "\", error: " + e.StackTrace);
        }

        return recipe;
    }
}