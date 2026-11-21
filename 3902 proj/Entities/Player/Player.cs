using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Collisions;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities;

internal class Player : IPlayer
{
    public const float FootInset = 16f;

    private readonly CollisionManager collisionManager;
    private readonly PlayerMovement movement;
    private readonly PlayerInventory inventory = new PlayerInventory();
    private readonly PlayerHealth health = new PlayerHealth();
    private readonly PlayerShield shield = new PlayerShield();
    private readonly WeaponLoadout weapons = new WeaponLoadout();
    private readonly PlayerAim aim = new PlayerAim();

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

    // Unit vector toward the aim point (the mouse) in world space, refreshed each frame.
    public Vector2 AimDirection => aim.Direction;

    // Sets the aim from a world-space target point relative to the player's center,
    // and turns the player to face the aim's horizontal direction.
    public void SetAimTarget(Vector2 worldTarget)
    {
        aim.SetTarget(Center, worldTarget);
        if (!IsInBallForm)
        {
            Direction facing = aim.Direction.X < 0f ? Direction.Left : Direction.Right;
            if (movement.FaceToward(facing))
            {
                form.RefreshSprite();
            }
        }
    }

    public PlayerArsenal Arsenal { get; }

    public int Health => health.Current;

    public int MaxHealth => health.Maximum;


    public PlayerInventoryView Inventory => inventory.View;
    public int KeyCount => inventory.KeyCount;
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

    public void UseItem(int itemNumber, bool angled = false) => form.UseItem(itemNumber, angled);

    public void Transform()
    {
        if (form.TryTransform(Height, Width))
        {
            PlaySound(GameSound.Transform);
        }
    }

    public bool TakeDamage()
    {
        int amount = PlayerHealth.DamageAmount;

        // The shield only helps while rolling: half damage in ball form, no effect otherwise.
        if (IsShieldActive && IsInBallForm)
        {
            amount = (amount + 1) / 2;
        }

        health.TakeDamage(amount);
        movement.ApplyKnockback();

        return true;
    }

    public bool TryAddItem(ItemKind kind) => inventory.TryCollect(kind, health);

    public bool UseMedkit()
    {
        if (!inventory.TryUseMedkit(health))
        {
            return false;
        }

        PlaySound(GameSound.Item);
        return true;
    }

    public bool UseShield()
    {
        if (!inventory.TryUseShield(shield))
        {
            return false;
        }

        PlaySound(GameSound.Item);
        return true;
    }

    public void DropBomb() => Arsenal.DropBomb(movement.Gravity, Inventory.BombLevel);
    public bool TryUseKey() => inventory.TryUseKey();

    public bool IsWeaponUnlocked(WeaponKind kind) => weapons.IsUnlocked(kind);
    public int WeaponAmmo(WeaponKind kind) => weapons.Ammo(kind);
    public bool WeaponHasUnlimitedAmmo(WeaponKind kind) => WeaponLoadout.HasUnlimitedAmmo(kind);
    public void UnlockWeapon(WeaponKind kind) => weapons.Unlock(kind);
    public void AddWeaponAmmo(WeaponKind kind, int amount) => weapons.AddAmmo(kind, amount);

    // Unlocks and stocks every weapon. Used by dev mode for testing.
    public void UnlockAllWeapons()
    {
        foreach (WeaponKind kind in System.Enum.GetValues<WeaponKind>())
        {
            weapons.Unlock(kind);
        }
    }

    // Fires the given weapon when it is unlocked and loaded, spending one shot.
    public bool FireWeapon(WeaponKind kind)
    {
        if (IsInBallForm || !weapons.TryConsume(kind))
        {
            return false;
        }
        switch (kind)
        {
            case WeaponKind.Boomerang:
                Arsenal.ThrowBoomerang();
                break;
            case WeaponKind.Pistol:
                Arsenal.FirePistol();
                break;
            case WeaponKind.Shotgun:
                Arsenal.FireShotgun(false);
                break;
            case WeaponKind.Molotov:
                Arsenal.ThrowMolotov(movement.Gravity);
                break;
        }
        return true;
    }

    public bool UseShotgunCall() => inventory.TryUseShotgunCall();

    public bool FireShotgun(bool aimUp = false)
    {
        if (IsInBallForm || !inventory.TryFireShotgun()) return false;
        Arsenal.FireShotgun(aimUp);
        return true;
    }

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
