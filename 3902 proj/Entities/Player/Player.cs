using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Collisions;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities;

internal class Player : IPlayer
{
    public const float FootInset = 16f;

    private readonly CollisionManager collisionManager;
    private readonly PlayerMovement movement;
    private readonly PlayerInventory inventory = new PlayerInventory();
    private readonly PlayerHealth health = new PlayerHealth();
    private readonly PlayerShield shield = new PlayerShield();

    private readonly PlayerForm form;

    public Player(
        Vector2 position,
        IReadOnlyList<IBlock> blocks,
        IProjectileManager projectiles,
        float gravity = Physics.DefaultGravity,
        CollisionManager collisionManager = null)
    {
        Position = position;

        if (collisionManager == null)
        {
            collisionManager = new CollisionManager(960, 540);
        }

        this.collisionManager = collisionManager;
        movement = new PlayerMovement(this, this.collisionManager, gravity);

        this.collisionManager.SetBlocks(
            this,
            blocks,
            CollisionManager.PlayerFootInset);

        Arsenal = new PlayerArsenal(
            this,
            projectiles,
            this.collisionManager,
            blocks);

        SpeedScale = 1f;

        form = new PlayerForm(this, this.collisionManager);
    }

    public Vector2 Position { get; set; }
    public Direction Facing => movement.Facing;
    public bool FacingLeft => Facing == Direction.Left;
    public bool IsMoving => movement.IsMoving;
    public bool IsInBallForm => form.IsVehicle;

    public PlayerArsenal Arsenal { get; }

    public int Health => health.Current;

    public int MaxHealth => health.Maximum;

    public int MedkitCount => inventory.MedkitCount;

    public int ShieldCount => inventory.ShieldCount;
    public int BombLevel => inventory.BombLevel;
    public int EnergonCount => inventory.EnergonCount;
    public bool IsShieldActive => shield.IsActive;
    public bool IsInvincible => false;

    public float SpeedScale { get; set; }
    public bool IsOnGround => collisionManager.IsOnGround(this);

    public int Width => form.Width;
    public int Height => form.Height;

    public Vector2 Center =>
        new Vector2(
            Position.X + Width / 2f,
            Position.Y + Height / 2f);

    public void Move(Direction direction)
    {
        if (movement.Move(direction))
        {
            form.RefreshSprite();
        }
    }

    public void Jump()
    {
        if (movement.TryJump())
        {
            PlaySound(GameSound.Jump);
        }
    }

    public event System.Action<GameSound> SoundRequested;

    internal void PlaySound(GameSound sound) => SoundRequested?.Invoke(sound);

    public void Bounce(float speed) => movement.Bounce(speed);

    public void Shoot(bool angled) => form.Shoot(angled);

    public void UseItem(int itemNumber) => form.UseItem(itemNumber);

    public void Transform()
    {
        if (form.TryTransform())
        {
            PlaySound(GameSound.Transform);
        }
    }

    public bool TakeDamage()
    {
        if (IsShieldActive)
        {
            return false;
        }

        health.TakeDamage();
        movement.ApplyKnockback();

        return true;
    }

    public bool TryAddItem(ItemKind kind)
    {
        switch (kind)
        {
            case ItemKind.HealthCell:
                return health.TryUseHealthCell();
            case ItemKind.MaxHealthUp:
                return health.TryUpgrade();
            default:
                return inventory.TryCollect(kind);
        }
    }

    public bool UseMedkit()
    {
        if (health.IsFull || !inventory.TryUseMedkit())
        {
            return false;
        }

        health.Restore();
        PlaySound(GameSound.Item);
        return true;
    }

    public bool UseShield()
    {
        if (!shield.TryActivate(inventory))
        {
            return false;
        }

        PlaySound(GameSound.Item);
        return true;
    }

    public void DropBomb() => Arsenal.DropBomb(movement.Gravity, BombLevel);

    public void EnterRoom(
        IReadOnlyList<IBlock> blocks,
        IProjectileManager projectiles,
        Vector2 position,
        float gravity = Physics.DefaultGravity)
    {
        movement.SetGravity(gravity);

        collisionManager.SetBlocks(
            this,
            blocks,
            CollisionManager.PlayerFootInset);

        Arsenal.EnterRoom(projectiles, blocks);

        Position = position;
    }

    internal void RefreshGroundState() => form.RefreshSprite();
    public void SetState(IPlayerState newState) => form.SetState(newState);

    public void Update(GameTime gameTime)
    {
        shield.Update(gameTime.ElapsedGameTime);
        if (movement.BeginFrame())
        {
            form.RefreshSprite();
        }

        movement.Update(gameTime, form.Speed, SpeedScale);

        form.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch) => form.Draw(spriteBatch);
}
