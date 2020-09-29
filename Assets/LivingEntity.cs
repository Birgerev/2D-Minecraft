﻿using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

public class LivingEntity : Entity
{
    //Entity Properties
    public static Color damageColor = new Color(1, 0.5f, 0.5f, 1);

    [Header("Movement Properties")] private readonly float acceleration = 4f;

    private readonly float airDrag = 0.92f;
    private readonly float climbSpeed = 0.35f;
    protected EntityController controller;
    private readonly float groundFriction = 0.92f;

    //Entity Data Tags
    [EntityDataTag(false)] public float health;

    protected float highestYlevelsinceground;
    private readonly float jumpVelocity = 8f;
    private readonly float ladderFriction = 0.95f;
    protected float last_jump_time;
    private readonly float liquidDrag = 0.75f;
    protected bool sneaking;
    private readonly float sneakSpeed = 1.3f;


    //Entity State
    protected bool sprinting;
    private readonly float sprintSpeed = 5.6f;
    private readonly float swimUpSpeed = 2f;
    private readonly float walkSpeed = 4.3f;
    public virtual float maxHealth { get; } = 20;

    public override void Start()
    {
        health = maxHealth;

        base.Start();

        controller = GetController();

        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public virtual void FixedUpdate()
    {
        ProcessMovement();
        fallDamageCheck();
    }

    public override void Update()
    {
        base.Update();

        controller.Tick();
        CalculateFlip();
        UpdateAnimatorValues();

        if (Mathf.Abs(getVelocity().x) >= sneakSpeed && isOnGround)
        {
            float chances;
            if (sprinting)
                chances = 0.2f;
            else
                chances = 0.02f;

            spawnMovementParticles(chances);
        }
    }

    public virtual void UpdateAnimatorValues()
    {
        var anim = GetComponent<Animator>();

        if (anim == null)
            return;

        if (anim.isInitialized)
            anim.SetFloat("velocity-x", Mathf.Abs(getVelocity().x));
    }

    public virtual void ProcessMovement()
    {
        ApplyFriction();
    }

    public virtual void ApplyFriction()
    {
        if (isInLiquid)
            setVelocity(getVelocity() * liquidDrag);
        if (isOnLadder)
            setVelocity(getVelocity() * ladderFriction);
        if (!isInLiquid && !isOnLadder && !isOnGround)
            setVelocity(new Vector3(getVelocity().x * airDrag, getVelocity().y));
        if (!isInLiquid && !isOnLadder && isOnGround)
            setVelocity(getVelocity() * groundFriction);
    }

    public virtual void Walk(int direction)
    {
        float maxSpeed;
        if (sprinting)
            maxSpeed = sprintSpeed;
        else if (sneaking)
            maxSpeed = sneakSpeed;
        else
            maxSpeed = walkSpeed;

        if (getVelocity().x < maxSpeed && getVelocity().x > -maxSpeed)
        {
            float targetXVelocity = 0;

            if (direction == -1)
                targetXVelocity -= maxSpeed;
            else if (direction == 1)
                targetXVelocity += maxSpeed;
            else targetXVelocity = 0;

            GetComponent<Rigidbody2D>().velocity +=
                new Vector2(targetXVelocity * (acceleration * Time.fixedDeltaTime), 0);
        }

        StairCheck(direction);
    }

    public void StairCheck(int direction)
    {
        if ((Vector2) transform.position != lastFramePosition) //Return if player has moved since last frame
            return;
        if (!isOnGround) //Return if player isn't grounded
            return;

        var blockInFront = Location
            .LocationByPosition((Vector2) transform.position + new Vector2(direction * 0.7f, -0.5f), location.dimension)
            .GetBlock(); //Get block in front of player acording to walk direction

        if (blockInFront == null) return;

        if (Type.GetType(blockInFront.GetMaterial().ToString()).IsSubclassOf(typeof(Stairs)))
        {
            var rotated_x = false;
            var rotated_y = false;

            rotated_x = blockInFront.data.GetData("rotated_x") == "true";
            rotated_y = blockInFront.data.GetData("rotated_y") == "true";

            if (rotated_y == false && (direction == -1 && rotated_x == false || direction == 1 && rotated_x)
            ) //if the stairs are rotated correctly
                transform.position += new Vector3(0, 1);
        }
    }

    public virtual EntityController GetController()
    {
        return new EntityController(this);
    }

    public virtual void CalculateFlip()
    {
        if (getVelocity().x != 0) facingLeft = getVelocity().x < 0;
    }

    public virtual void Jump()
    {
        if (isOnGround)
        {
            if (Time.time - last_jump_time < 0.7f)
                return;

            setVelocity(getVelocity() + new Vector2(0, jumpVelocity));
            last_jump_time = Time.time;
        }

        if (isInLiquid && getVelocity().y < swimUpSpeed) setVelocity(getVelocity() + new Vector2(0, swimUpSpeed));

        if (isOnLadder) setVelocity(getVelocity() + new Vector2(0, climbSpeed));
    }


    private void fallDamageCheck()
    {
        if (isOnGround && !isInLiquid)
        {
            var damage = highestYlevelsinceground - transform.position.y - 3;
            if (damage >= 1)
            {
                Sound.Play(location, "entity/land", SoundType.Entities, 0.5f, 1.5f); //Play entity land sound

                spawnFallDamageParticles();

                TakeFallDamage(damage);
            }
        }

        if (isOnGround || isInLiquid || isOnLadder)
            highestYlevelsinceground = transform.position.y;
        else if (transform.position.y > highestYlevelsinceground)
            highestYlevelsinceground = transform.position.y;
    }

    private void spawnFallDamageParticles()
    {
        var r = new Random();
        Block blockBeneath = null;
        for (var y = -1; blockBeneath == null && y > -3; y--)
        {
            var block = (location + new Location(0, y)).GetBlock();
            if (block != null)
                blockBeneath = block;
        }

        var particleAmount = r.Next(4, 8);
        for (var i = 0; i < particleAmount; i++) //Spawn landing partickes
        {
            var part = (Particle) Spawn("Particle");

            part.transform.position = blockBeneath.location.GetPosition() + new Vector2(0, 0.6f);
            part.color = blockBeneath.GetRandomColourFromTexture();
            part.doGravity = true;
            part.velocity = new Vector2(((float) r.NextDouble() - 0.5f) * 2, 1.5f);
            part.maxAge = 1f + (float) r.NextDouble();
            part.maxBounces = 10;
        }
    }

    private void spawnMovementParticles(float chances)
    {
        var r = new Random();

        if (r.NextDouble() < chances)
        {
            Block blockBeneath = null;
            for (var y = 1; blockBeneath == null && y < 3; y++)
            {
                var block = (location - new Location(0, y)).GetBlock();
                if (block != null && block.playerCollide)
                    blockBeneath = block;
            }


            var part = (Particle) Spawn("Particle");

            part.transform.position = blockBeneath.location.GetPosition() + new Vector2(0, 0.6f);
            part.color = blockBeneath.GetRandomColourFromTexture();
            part.doGravity = true;
            part.velocity = -(getVelocity() * 0.2f);
            part.maxAge = (float) r.NextDouble();
            part.maxBounces = 10;
        }
    }

    public override void Hit(float damage)
    {
        base.Hit(damage);

        Knockback(transform.position - Player.localInstance.transform.position);
    }

    public virtual void TakeFallDamage(float damage)
    {
        Damage(damage);
    }

    public override void Damage(float damage)
    {
        Sound.Play(location, "entity/damage", SoundType.Entities, 0.5f, 1.5f);

        health -= damage;

        if (health <= 0)
            Die();

        StartCoroutine(TurnRedByDamage());
    }

    public override void Die()
    {
        Particle.Spawn_SmallSmoke(transform.position, Color.white);

        base.Die();
    }

    public virtual void Knockback(Vector2 direction)
    {
        direction.Normalize();

        GetComponent<Rigidbody2D>().velocity += new Vector2(direction.x * 3f, 4f);
    }

    private IEnumerator TurnRedByDamage()
    {
        var baseColor = getRenderer().color;

        getRenderer().color = damageColor;
        yield return new WaitForSeconds(0.15f);
        getRenderer().color = baseColor;
    }
}