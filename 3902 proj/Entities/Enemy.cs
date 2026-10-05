using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Factories;
using TransformersGame.Interfaces;
using TransformersGame.States;

namespace TransformersGame.Entities
{
    public class Enemy : IEnemy
    {
        private const int StartingHealth = 1;
        private const int ScreenWidth = 960;
        private const int ScreenHeight = 540;
        private const int LedgeInset = 12;

        private readonly IReadOnlyList<IBlock> blocks;
        private readonly Physics physics;
        private readonly IProjectileManager projectiles;
        private int health;

        public Enemy(Vector2 position, IReadOnlyList<IBlock> blocks, EnemyConfiguration configuration, IPlayer target, IProjectileManager projectiles)
        {
            Position = position;
            Home = position;
            Kind = configuration.Kind;
            Tint = Color.White;
            Stats = configuration.Stats;
            Behavior = configuration.Behavior;
            Target = target;
            this.blocks = blocks;
            this.projectiles = projectiles;
            physics = new Physics(blocks);
            health = StartingHealth;
            LeftSprite = EnemySpriteFactory.Instance.CreateEnemySprite(Kind, Tint, true);
            RightSprite = EnemySpriteFactory.Instance.CreateEnemySprite(Kind, Tint, false);
            State = new LeftWalkingEnemyState(this);
        }

        public ISprite Sprite { get; set; }

        public ISprite LeftSprite { get; private set; }

        public ISprite RightSprite { get; private set; }

        public IEnemyState State { get; set; }

        public IEnemyBehavior Behavior { get; private set; }

        public EnemyStats Stats { get; private set; }

        public IPlayer Target { get; private set; }

        public EnemyKind Kind { get; private set; }

        public Color Tint { get; private set; }

        public Vector2 Position { get; set; }

        public Vector2 Home { get; private set; }

        public int Facing
        {
            get
            {
                return State.Facing;
            }
        }

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
                return Sprite.Width;
            }
        }

        public int Height
        {
            get
            {
                return Sprite.Height;
            }
        }

        public Vector2 Center
        {
            get
            {
                return new Vector2(Position.X + Width / 2f, Position.Y + Height / 2f);
            }
        }

        public Vector2 TargetCenter
        {
            get
            {
                return new Vector2(Target.Position.X + Target.Width / 2f, Target.Position.Y + Target.Height / 2f);
            }
        }

        public int DirectionToTarget
        {
            get
            {
                return TargetCenter.X >= Center.X ? 1 : -1;
            }
        }

        public float HorizontalDistanceToTarget
        {
            get
            {
                return System.Math.Abs(TargetCenter.X - Center.X);
            }
        }

        public bool CanSeeTarget(float range, float verticalTolerance)
        {
            Vector2 offset = TargetCenter - Center;
            return System.Math.Abs(offset.X) <= range && System.Math.Abs(offset.Y) <= verticalTolerance;
        }

        public void Face(int direction)
        {
            if (direction != 0 && direction != State.Facing)
            {
                State.ChangeDirection();
            }
        }

        public void MoveBy(float distance)
        {
            float x = MathHelper.Clamp(Position.X + distance, 0, ScreenWidth - Width);
            Position = new Vector2(x, Position.Y);
        }

        public bool FlyHorizontally(float distance)
        {
            // flying enemies do not use gravity so their movement checks solid blocks directly
            if (distance == 0)
            {
                return false;
            }
            float desired = Position.X + distance;
            float x = MathHelper.Clamp(desired, 0, ScreenWidth - Width);
            bool blocked = x != desired;
            Rectangle box = new Rectangle((int)x, (int)Position.Y, Width, Height);
            foreach (IBlock block in blocks)
            {
                if (block.IsSolid && box.Intersects(block.Bounds))
                {
                    x = distance > 0 ? block.Bounds.Left - Width : block.Bounds.Right;
                    box.X = (int)x;
                    blocked = true;
                }
            }
            Position = new Vector2(x, Position.Y);
            return blocked;
        }

        public bool FlyVertically(float distance)
        {
            if (distance == 0)
            {
                return false;
            }
            float desired = Position.Y + distance;
            float y = MathHelper.Clamp(desired, 0, ScreenHeight - Height);
            bool blocked = y != desired;
            Rectangle box = new Rectangle((int)Position.X, (int)y, Width, Height);
            foreach (IBlock block in blocks)
            {
                if (block.IsSolid && box.Intersects(block.Bounds))
                {
                    y = distance > 0 ? block.Bounds.Top - Height : block.Bounds.Bottom;
                    box.Y = (int)y;
                    blocked = true;
                }
            }
            Position = new Vector2(Position.X, y);
            return blocked;
        }

        public bool Fly(Vector2 movement)
        {
            bool blockedHorizontally = FlyHorizontally(movement.X);
            bool blockedVertically = FlyVertically(movement.Y);
            return blockedHorizontally || blockedVertically;
        }

        public bool HasGroundAhead(int direction, float distance)
        {
            // the small probe below the leading foot keeps ground enemies on their platform
            float probeX = direction > 0
                ? Position.X + Width - LedgeInset + distance
                : Position.X + LedgeInset - distance;
            Rectangle probe = new Rectangle((int)probeX, (int)(Position.Y + Height) + 1, 1, 2);
            foreach (IBlock block in blocks)
            {
                if (block.IsSolid && block.Bounds.Intersects(probe))
                {
                    return true;
                }
            }
            return false;
        }

        public void Jump(float speed)
        {
            physics.Jump(speed);
        }

        public void ApplyGravity(GameTime gameTime)
        {
            Position = physics.Apply(Position, Width, Height, gameTime);
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
            if (health <= 0)
            {
                State.BeDestroyed();
            }
        }

        public void FireProjectile()
        {
            Vector2 center = Center;
            ISprite sprite = ProjectileSpriteFactory.Instance.CreateOrbSprite(TargetCenter.X < center.X);
            projectiles.Add(new EnemyProjectile(center, TargetCenter, sprite));
        }

        public void Update(GameTime gameTime)
        {
            State.Update(gameTime);
            Sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch, Position);
        }
    }
}
