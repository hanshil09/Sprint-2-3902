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
        private const float JumpSpeed = 650f;
        private const float KnockbackSpeed = 300f;
        private const float ShotSpeed = 520f;
        private const float OrbSpeed = 300f;
        private const float MuzzleHeight = 0.3f;

        private Physics physics;
        private ProjectileManager projectiles;
        private IPlayerState state;
        private ISprite sprite;
        private bool movedThisFrame;
        private bool wasOnGround;

        public Player(Vector2 position, List<IBlock> blocks, ProjectileManager projectiles)
        {
            Position = position;
            physics = new Physics(blocks);
            this.projectiles = projectiles;
            Facing = Direction.Right;
            FacingLeft = false;
            IsMoving = false;
            state = new RobotState(this);
            sprite = state.CreateSprite();
        }

        public Vector2 Position { get; set; }

        public Direction Facing { get; private set; }

        public bool FacingLeft { get; private set; }

        public bool IsMoving { get; private set; }
        public bool angledShot { get; set; }

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

        public void Shoot()
        {
            state.Shoot(angledShot);
        }

        public void UseItem(int itemNumber)
        {
            state.UseItem(itemNumber);
        }

        public void Transform()
        {
            state.Transform();
        }

        public void TakeDamage()
        {
            physics.Jump(KnockbackSpeed);
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
                Position = new Vector2(Position.X + distance, Position.Y);
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
