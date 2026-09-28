using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TransformersGame.Core;
using TransformersGame.Factories;
using TransformersGame.Interfaces;
namespace TransformersGame.Entities;
public sealed class Player : IPlayer
{
    private const float RobotSpeed = 180f;
    private const float VehicleSpeed = 280f;
    private readonly PlaceholderSpriteFactory spriteFactory;
    private ISprite sprite;
    private Vector2 movement;
    private bool isVehicle;
    private double damageTimeRemaining;
    private double shootingTimeRemaining;
    private double jumpTimeRemaining;
    private readonly List<EnergyProjectile> projectiles = new();
    public Player(Vector2 position, PlaceholderSpriteFactory spriteFactory)
    {
        Position = position;
        this.spriteFactory = spriteFactory;
        Facing = Direction.Down;
        sprite = spriteFactory.CreateRobotSprite(Facing);
    }
    public Vector2 Position { get; set; }
    public Direction Facing { get; private set; }
    public int Width => sprite.Width;
    public int Height => sprite.Height;
    public IReadOnlyList<EnergyProjectile> Projectiles => projectiles;
    public void Move(Direction direction)
    {
        bool needsNewSprite = movement == Vector2.Zero || Facing != direction;
        Facing = direction;
        movement = direction switch
        {
            Direction.Up => -Vector2.UnitY,
            Direction.Down => Vector2.UnitY,
            Direction.Left => -Vector2.UnitX,
            Direction.Right => Vector2.UnitX,
            _ => Vector2.Zero
        };
        if (needsNewSprite) RefreshSprite();
    }
    public void StopMoving()
    {
        if (movement == Vector2.Zero) return;
        movement = Vector2.Zero;
        RefreshSprite();
    }
    public void Shoot()
    {
        if (isVehicle || shootingTimeRemaining > 0) return;

        Vector2 direction = Facing switch
        {
            Direction.Up => -Vector2.UnitY,
            Direction.Down => Vector2.UnitY,
            Direction.Left => -Vector2.UnitX,
            _ => Vector2.UnitX
        };
        Vector2 origin = Position + new Vector2(Width / 2f - 12f, Height / 2f - 12f) + direction * 34f;
        projectiles.Add(new EnergyProjectile(origin, direction, spriteFactory.CreateProjectileSprite()));
        shootingTimeRemaining = 0.28;
        RefreshSprite();
    }
    public void Jump()
    {
        if (jumpTimeRemaining <= 0) jumpTimeRemaining = 0.55;
    }
    public void Transform() { isVehicle = !isVehicle; RefreshSprite(); }
    public void TakeDamage() { damageTimeRemaining = 0.5; RefreshSprite(); }
    public void Reset(Vector2 position)
    {
        Position = position;
        Facing = Direction.Down;
        movement = Vector2.Zero;
        isVehicle = false;
        damageTimeRemaining = 0;
        shootingTimeRemaining = 0;
        jumpTimeRemaining = 0;
        projectiles.Clear();
        RefreshSprite();
    }
    public void Update(GameTime gameTime)
    {
        Position += movement * (isVehicle ? VehicleSpeed : RobotSpeed) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (damageTimeRemaining > 0)
        {
            damageTimeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
            if (damageTimeRemaining <= 0) RefreshSprite();
        }
        if (shootingTimeRemaining > 0)
        {
            shootingTimeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
            if (shootingTimeRemaining <= 0) RefreshSprite();
        }
        if (jumpTimeRemaining > 0)
            jumpTimeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
        for (int index = projectiles.Count - 1; index >= 0; index--)
        {
            projectiles[index].Update(gameTime);
            if (!projectiles[index].IsActive) projectiles.RemoveAt(index);
        }
        sprite.Update(gameTime);
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        float jumpOffset = jumpTimeRemaining > 0
            ? (float)System.Math.Sin((0.55 - jumpTimeRemaining) / 0.55 * System.Math.PI) * 28f
            : 0f;
        sprite.Draw(spriteBatch, Position - new Vector2(0, jumpOffset));
        foreach (EnergyProjectile projectile in projectiles) projectile.Draw(spriteBatch);
    }
    private void RefreshSprite()
    {
        bool damaged = damageTimeRemaining > 0;
        if (shootingTimeRemaining > 0 && !isVehicle)
        {
            sprite = spriteFactory.CreateShootingRobotSprite(Facing, damaged);
        }
        else if (movement != Vector2.Zero)
        {
            sprite = isVehicle ? spriteFactory.CreateMovingVehicleSprite(Facing, damaged) : spriteFactory.CreateMovingRobotSprite(Facing, damaged);
        }
        else
        {
            sprite = isVehicle ? spriteFactory.CreateVehicleSprite(Facing, damaged) : spriteFactory.CreateRobotSprite(Facing, damaged);
        }
    }
}
