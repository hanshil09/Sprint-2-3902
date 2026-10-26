using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private const int ScreenWidth = 960;

        // The player art has transparent margins, so only the middle of the box counts as feet.
        public const float FootInset = 16f;

        private readonly Physics physics;
        private readonly PlayerInventory inventory = new PlayerInventory();
        private IReadOnlyList<IBlock> roomBlocks;
        private IPlayerState state;
        private ISprite sprite;
        private bool movedThisFrame;
        private bool wasOnGround;
        private double shieldTimeRemaining;

        public Player(Vector2 position, IReadOnlyList<IBlock> blocks, IProjectileManager projectiles, float gravity = Physics.DefaultGravity)
        {
            Position = position;
            physics = new Physics(blocks, gravity) { FootInset = FootInset };
            roomBlocks = blocks;
            Arsenal = new PlayerArsenal(this, projectiles);
            Facing = Direction.Right;
            Health = StartingHealth;
            MaxHealth = StartingHealth;
            SpeedScale = 1f;
            state = new RobotState(this);
            sprite = state.CreateSprite();
        }

        public Vector2 Position { get; set; }
        public Direction Facing { get; private set; }
        public bool FacingLeft => Facing == Direction.Left;
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
        public bool IsOnGround => physics.IsOnGround;

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
            if (hasChanged)
            {
                RefreshSprite();
            }
        }

        public void Jump() => physics.Jump(JumpSpeed);

        public void Bounce(float speed) => physics.Launch(speed);

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
            physics.Jump(KnockbackSpeed);
            return true;
        }

        public void AddItem(ItemKind kind)
        {
            if (inventory.TryCollect(kind))
            {
                return;
            }
            switch (kind)
            {
                case ItemKind.HealthCell:
                    Health = System.Math.Min(MaxHealth, Health + HealthCellAmount);
                    break;
                case ItemKind.ShieldBattery:
                    shieldTimeRemaining = System.Math.Min(MaximumShieldTime, shieldTimeRemaining + ShieldDuration);
                    break;
                case ItemKind.MaxHealthUp:
                    MaxHealth = System.Math.Min(MaximumHealthCap, MaxHealth + MaxHealthUpgrade);
                    Health = MaxHealth;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(kind), kind, "This item is not added to the player's inventory.");
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
            Arsenal.DropBomb(roomBlocks, physics.Gravity, BombLevel);
        }

        public void EnterRoom(IReadOnlyList<IBlock> blocks, IProjectileManager projectiles, Vector2 position, float gravity = Physics.DefaultGravity)
        {
            physics.SetBlocks(blocks, gravity);
            roomBlocks = blocks;
            Arsenal.EnterRoom(projectiles);
            Position = position;
            wasOnGround = false;
        }

        public void SetState(IPlayerState newState)
        {
            int oldHeight = Height;
            state = newState;
            RefreshSprite();
            Position = new Vector2(Position.X, Position.Y + oldHeight - Height);
        }

        public void Update(GameTime gameTime)
        {
            UpdateShield(gameTime);
            UpdateMovementState();

            GetPushedByMovingBlocks();
            RideMovingBlock();
            MoveHorizontally(gameTime);

            Position = physics.Apply(Position, Width, Height, gameTime);

            UpdateGroundState();

            state.Update(gameTime);
            sprite.Update(gameTime);
        }
        private void UpdateShield(GameTime gameTime)
        {
            shieldTimeRemaining = System.Math.Max(
                0,
                shieldTimeRemaining -
                gameTime.ElapsedGameTime.TotalMilliseconds);
        }
        private void UpdateMovementState()
        {
            if (!movedThisFrame && IsMoving)
            {
                IsMoving = false;
                RefreshSprite();
            }

            movedThisFrame = false;
        }
        private void UpdateGroundState()
        {
            if (physics.IsOnGround == wasOnGround)
            {
                return;
            }

            wasOnGround = physics.IsOnGround;
            RefreshSprite();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, Position);
        }

        // Push the player away when a moving platform slides into them.
        private void GetPushedByMovingBlocks()
        {
            Rectangle body = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            foreach (IBlock block in roomBlocks)
            {
                if (block is IMovingBlock platform && platform != physics.SupportingBlock
                    && platform.Delta.X != 0 && platform.Bounds.Intersects(body))
                {
                    float targetX = platform.Delta.X > 0 ? platform.Bounds.Right : platform.Bounds.Left - Width;
                    Position = physics.MoveHorizontally(Position, Width, Height, targetX - Position.X, 0, ScreenWidth - Width);
                }
            }
        }

        // Carry the player along with the platform's movement.
        private void RideMovingBlock()
        {
            if (physics.SupportingBlock is not IMovingBlock platform || platform.Delta == Vector2.Zero)
            {
                return;
            }
            Position = physics.MoveHorizontally(Position, Width, Height, platform.Delta.X, 0, ScreenWidth - Width);
            Position = new Vector2(Position.X, Position.Y + platform.Delta.Y);
        }

        private void MoveHorizontally(GameTime gameTime)
        {
            if (IsMoving)
            {
                float direction = FacingLeft ? -1 : 1;
                float distance = direction * state.Speed * SpeedScale * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Position = physics.MoveHorizontally(Position, Width, Height, distance, 0, ScreenWidth - Width);
            }
        }

        private void RefreshSprite()
        {
            sprite = state.CreateSprite();
        }
    }
}
