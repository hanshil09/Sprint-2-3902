using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Factories;
using TransformersGame.Interfaces;
using TransformersGame.States;

namespace TransformersGame.Entities
{
    public class Player : IPlayer
    {
        private const int StartingHealth = 100;
        private const int MedkitHealing = 25;
        private const int DamageAmount = 25;
        private const double ShieldDuration = 5000;
        private const float JumpSpeed = 650f;
        private const float KnockbackSpeed = 300f;
        private const float ShotSpeed = 520f;
        private const float ShotRecoilDistance = 10f;
        private const float OrbSpeed = 300f;
        private const float MuzzleHeight = 0.3f;

        private readonly Physics physics;
        private IProjectileManager projectiles;
        private IPlayerState state;
        private ISprite sprite;
        private bool movedThisFrame;
        private bool wasOnGround;
        private int medkitCount;
        private int shieldCount;
        private double shieldTimeRemaining;

        public Player(Vector2 position, IReadOnlyList<IBlock> blocks, IProjectileManager projectiles)
        {
            Position = position;
            physics = new Physics(blocks);
            this.projectiles = projectiles;
            Facing = Direction.Right;
            FacingLeft = false;
            IsMoving = false;
            Health = StartingHealth;
            state = new RobotState(this);
            sprite = state.CreateSprite();
        }

        public Vector2 Position { get; set; }

        public Direction Facing { get; private set; }

        public bool FacingLeft { get; private set; }

        public bool IsMoving { get; private set; }
        public bool angledShot { get; private set; }

        public int Health { get; private set; }

        public int MaxHealth => StartingHealth;

        public int MedkitCount => medkitCount;

        public int ShieldCount => shieldCount;

        public bool IsShieldActive => shieldTimeRemaining > 0;

        public bool IsOnGround
        {
            get
            {
                return physics.IsOnGround;
            }
        }

        public int Width
        {
            get
            {
                return sprite.Width;
            }
        }

        public int Height
        {
            get
            {
                return sprite.Height;
            }
        }

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

        public void Jump()
        {
            physics.Jump(JumpSpeed);
        }

        public void Shoot(bool angled)
        {
            angledShot = angled;
            state.Shoot(angledShot);
        }

        public void UseItem(int itemNumber)
        {
            if (itemNumber == 5)
            {
                ISprite shot = ProjectileSpriteFactory.Instance.CreateOrbSprite(FacingLeft);
                projectiles.Add(new PiercingProjectile(MuzzlePosition(shot), ShotVelocity(420), shot));
                return;
            }
            if (itemNumber == 6)
            {
                ISprite shot = ProjectileSpriteFactory.Instance.CreateShotSprite(FacingLeft);
                foreach (float vertical in new[] { -140f, 0f, 140f })
                {
                    projectiles.Add(new EnergyProjectile(MuzzlePosition(shot), new Vector2(FacingLeft ? -400 : 400, vertical), false, FacingLeft, shot));
                }
                return;
            }
            state.UseItem(itemNumber);
        }

        public void Transform()
        {
            state.Transform();
        }

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
            switch (kind)
            {
                case ItemKind.Medkit:
                    medkitCount++;
                    break;
                case ItemKind.Shield:
                    shieldCount++;
                    break;
                case ItemKind.HealthCell:
                    Health = System.Math.Min(MaxHealth, Health + 10);
                    break;
                case ItemKind.ShieldBattery:
                    shieldTimeRemaining = System.Math.Min(10000, shieldTimeRemaining + ShieldDuration);
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(kind), kind, "Unknown inventory item.");
            }
        }

        public bool UseMedkit()
        {
            if (medkitCount == 0 || Health == MaxHealth)
            {
                return false;
            }
            medkitCount--;
            Health = System.Math.Min(MaxHealth, Health + MedkitHealing);
            return true;
        }

        public bool UseShield()
        {
            if (shieldCount == 0 || IsShieldActive)
            {
                return false;
            }
            shieldCount--;
            shieldTimeRemaining = ShieldDuration;
            return true;
        }

        public void EnterRoom(IReadOnlyList<IBlock> blocks, IProjectileManager projectiles, Vector2 position)
        {
            physics.SetBlocks(blocks);
            this.projectiles = projectiles;
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

        public void FireShot()
        {
            ISprite shotSprite = ProjectileSpriteFactory.Instance.CreateShotSprite(FacingLeft);
            projectiles.Add(new EnergyProjectile(MuzzlePosition(shotSprite), ShotVelocity(ShotSpeed), angledShot, FacingLeft, shotSprite));

            // Recoil always pushes the player in the direction opposite of the shot.
            float recoil = FacingLeft ? ShotRecoilDistance : -ShotRecoilDistance;
            Position = new Vector2(Position.X + recoil, Position.Y);
        }

        public void FireOrb()
        {
            ISprite orbSprite = ProjectileSpriteFactory.Instance.CreateOrbSprite(FacingLeft);
            bool angled = false;
            projectiles.Add(new EnergyProjectile(MuzzlePosition(orbSprite), ShotVelocity(OrbSpeed), angled, FacingLeft, orbSprite));
        }

        public void DropBomb()
        {
            ISprite bombSprite = ProjectileSpriteFactory.Instance.CreateBombSprite();
            float x = Position.X + (Width - bombSprite.Width) / 2f;
            float y = Position.Y + Height - bombSprite.Height;
            projectiles.Add(new Bomb(new Vector2(x, y), bombSprite));
        }

        public void Update(GameTime gameTime)
        {
            if (shieldTimeRemaining > 0)
            {
                shieldTimeRemaining = System.Math.Max(
                    0,
                    shieldTimeRemaining - gameTime.ElapsedGameTime.TotalMilliseconds);
            }
            if (!movedThisFrame && IsMoving)
            {
                IsMoving = false;
                RefreshSprite();
            }
            movedThisFrame = false;
            MoveHorizontally(gameTime);
            Position = physics.Apply(Position, Width, Height, gameTime);
            if (physics.IsOnGround != wasOnGround)
            {
                wasOnGround = physics.IsOnGround;
                RefreshSprite();
            }
            state.Update(gameTime);
            sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, Position);
        }

        private void MoveHorizontally(GameTime gameTime)
        {
            if (IsMoving)
            {
                float direction = FacingLeft ? -1 : 1;
                float distance = direction * state.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Position = physics.MoveHorizontally(Position, Width, Height, distance, 0, 960 - Width);
            }
        }

        private Vector2 MuzzlePosition(ISprite projectileSprite)
        {
            float x = FacingLeft ? Position.X - projectileSprite.Width : Position.X + Width;
            float y = Position.Y + Height * MuzzleHeight;
            return new Vector2(x, y);
        }

        private Vector2 ShotVelocity(float speed)
        {
            return new Vector2(FacingLeft ? -speed : speed, 0);
        }

        private void RefreshSprite()
        {
            sprite = state.CreateSprite();
        }
    }
}
