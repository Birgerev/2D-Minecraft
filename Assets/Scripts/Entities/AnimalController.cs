﻿using UnityEngine;
using Random = System.Random;

public class AnimalController : EntityController
{
    public static float idleStateChangeChance = 0.2f;
    public static float walkingStateChangeChance = 0.4f;
    public bool isWalking;
    public bool walkingRight;

    public AnimalController(LivingEntity instance) : base(instance)
    {
    }

    public override void Tick()
    {
        base.Tick();

        var block = instance.Location.GetBlock();
        var blockInFront = (instance.Location + new Location(walkingRight ? 1 : -1, 0)).GetBlock();
        if (isWalking)
        {
            instance.Walk(walkingRight ? 1 : -1);

            //Jump when there is a block in front of entity
            if (blockInFront != null && blockInFront.solid) instance.Jump();
        }

        //Swim in water
        if (instance.isInLiquid)
            instance.Jump();


        //Change States
        var r = new Random((instance.Location + " " + instance.age).GetHashCode());

        if (r.NextDouble() < (isWalking ? walkingStateChangeChance : idleStateChangeChance) / (1.0f / Time.deltaTime))
        {
            isWalking = !isWalking;
            walkingRight = r.Next(0, 2) == 0;
        }
    }
}