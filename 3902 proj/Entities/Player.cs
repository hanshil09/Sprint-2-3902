using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Collisions;
using TransformersGame.Core;
using TransformersGame.Factories;
using TransformersGame.Interfaces;
using TransformersGame.States;

namespace TransformersGame.Entities
{
    internal class Player : IPlayer
    {
        private const int StartingHealth = 100;
        private const int MaximumHealthCap = 200;
        private const int MaxHealthUpgrade = 20;
        private const int HealthCellAmount = 10;
        private const int DamageAmount = 3;
        private const double ShieldDuration = 5000;
        private const double MaximumShieldTime = 10000;
        private const float JumpSpeed = 650f;
        private const float KnockbackSpeed = 300f;
        private readonly Physics physics;
        private readonly CollisionManager collisionManager;
        private readonly PlayerInventory inventory = new PlayerInventory();
        private IPlayerState state;
        private ISprite sprite;
        private bool movedThisFrame;
        private double shieldTimeRemaining;

        public Player(Vector2 position, IReadOnlyList<IBlock> blocks, IProjectileManager projectiles,
            float gravity = Physics.DefaultGravity, CollisionManager collisionManager = null)
        {
            Position = position;
            physics = new Physics(gravity);
            this.collisionManager = collisionManager ?? new CollisionManager(960, 540);
            this.collisionManager.SetBlocks(this, blocks, CollisionManager.PlayerFootInset);
            Arsenal = new PlayerArsenal(this, projectiles, this.collisionManager, blocks);
            Facing = Direction.Right;
            Health = StartingHealth;
            MaxHealth = StartingHealth;
            SpeedScale = 1f;
            state = new RobotState(this);
            sprite = state.CreateSprite();
        }

        public Vector2 Position { get; set; }

        public Direction Facing { get; private set; }

        public bool FacingLeft { get; private set; }

        public bool IsMoving { get; private set; }

        public PlayerArsenal Arsenal { get; }

        public int Health { get; private set; }

        public int MaxHealth { get; private set; }

        public int MedkitCount => inventory.MedkitCount;

        public int ShieldCount => inventory.ShieldCount;

        public int BombLevel => inventory.BombLevel;

        public int EnergonCount => inventory.EnergonCount;

        public bool IsShieldActive => shieldTimeRemaining > 0;

        public bool IsInvincible => false;

        public float SpeedScale { get; set; }

        public bool IsOnGround => collisionManager.IsOnGround(this);

        public int Width => sprite.Width;

        public int Height => sprite.Height;

        public Vector2 Center => new Vector2(Position.X + Width / 2f, Position.Y + Height / 2f);

        public void Move(Direction direction)
        {
            bool isHorizontal = direction == Direction.Left || direction == Direction.Right;
            bool hasChanged = Facing != direction || IsMoving != isHorizontal;
            movedThisFrame = true;
            Facing = direction;
            IsMoving = isHorizontal;
            if (isHorizontal)
            {
                FacingLeft = direction == Direction.Left;
            }
            if (hasChanged)
            {
                RefreshSprite();
            }
        }

        public void Jump() => collisionManager.TryJump(this, physics, JumpSpeed);

        public void Bounce(float speed) => collisionManager.Launch(this, physics, speed);

        public void Shoot(bool angled) => state.Shoot(angled);

        public void UseItem(int itemNumber) => state.UseItem(itemNumber);

        public void Transform() => state.Transform();

        public bool TakeDamage()
        {
            if (IsShieldActive)
            {
                return false;
            }
            Health = System.Math.Max(0, Health - DamageAmount);
            collisionManager.TryJump(this, physics, KnockbackSpeed);
            return true;
        }

        public bool TryAddItem(ItemKind kind)
        {
            switch (kind)
            {
                case ItemKind.HealthCell:
                    if (Health == MaxHealth) return false;
                    Health = System.Math.Min(MaxHealth, Health + HealthCellAmount);
                    return true;
                case ItemKind.ShieldBattery:
                    if (shieldTimeRemaining == MaximumShieldTime) return false;
                    shieldTimeRemaining = System.Math.Min(MaximumShieldTime, shieldTimeRemaining + ShieldDuration);
                    return true;
                case ItemKind.MaxHealthUp:
                    if (MaxHealth == MaximumHealthCap && Health == MaxHealth) return false;
                    MaxHealth = System.Math.Min(MaximumHealthCap, MaxHealth + MaxHealthUpgrade);
                    Health = MaxHealth;
                    return true;
                default:
                    return inventory.TryCollect(kind);
            }
        }

        public bool UseMedkit()
        {
            if (Health == MaxHealth || !inventory.TryUseMedkit())
            {
                return false;
            }
            Health = MaxHealth;
            return true;
        }

        public bool UseShield()
        {
            if (IsShieldActive || !inventory.TryUseShield())
            {
                return false;
            }
            shieldTimeRemaining = ShieldDuration;
            return true;
        }

        public void DropBomb()
        {
            Arsenal.DropBomb(physics.Gravity, BombLevel);
        }

        public void EnterRoom(IReadOnlyList<IBlock> blocks, IProjectileManager projectiles, Vector2 position, float gravity = Physics.DefaultGravity)
        {
            physics.SetGravity(gravity);
            collisionManager.SetBlocks(this, blocks, CollisionManager.PlayerFootInset);
            Arsenal.EnterRoom(projectiles, blocks);
            Position = position;
        }

        internal void RefreshGroundState() => RefreshSprite();

        public void SetState(IPlayerState newState)
        {
            int oldHeight = Height;
            state = newState;
            RefreshSprite();
            Position = new Vector2(Position.X, Position.Y + oldHeight - Height);
        }

        public void Update(GameTime gameTime)
        {
            shieldTimeRemaining = System.Math.Max(0, shieldTimeRemaining - gameTime.ElapsedGameTime.TotalMilliseconds);
            if (!movedThisFrame && IsMoving)
            {
                IsMoving = false;
                RefreshSprite();
            }
            movedThisFrame = false;
            float horizontalDistance = IsMoving
                ? (FacingLeft ? -1 : 1) * state.Speed * SpeedScale * (float)gameTime.ElapsedGameTime.TotalSeconds
                : 0;
            collisionManager.UpdatePlayerMovement(this, physics, gameTime, horizontalDistance);
            state.Update(gameTime);
            sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, Position);
        }

        private void RefreshSprite()
        {
            sprite = state.CreateSprite();
        }
    }
}
