using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;
using TransformersGame.States;

namespace TransformersGame.Entities
{
    public class Enemy : IEnemy
    {
        private const float HopSpeed = 300f;
        private const int StartingHealth = 1;

        private Physics physics;
        private EnemyStats stats;
        private float leftBoundary;
        private float rightBoundary;
        private double directionTimer;
        private double hopTimer;
        private int health;

        public Enemy(Vector2 position, List<IBlock> blocks, EnemyKind kind, EnemyStats stats)
        {
            Position = position;
            Kind = kind;
            Tint = Color.White;
            this.stats = stats;
            physics = new Physics(blocks);
            leftBoundary = position.X - stats.PatrolDistance;
            rightBoundary = position.X + stats.PatrolDistance;
            directionTimer = 0;
            hopTimer = 0;
            health = StartingHealth;
            State = new LeftWalkingEnemyState(this);
        }

        public ISprite Sprite { get; set; }

        public IEnemyState State { get; set; }

        public EnemyKind Kind { get; private set; }

        public Color Tint { get; private set; }

        public Vector2 Position { get; set; }

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

        public void TakeDamage(int amount)
        {
            health -= amount;
            if (health <= 0)
            {
                State.BeDestroyed();
            }
        }

        public void Update(GameTime gameTime)
        {
            UpdateTimers(gameTime);
            State.Update(gameTime);
            Position = physics.Apply(Position, Sprite.Width, Sprite.Height, gameTime);
            Sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch, Position);
        }

        public void Walk(int direction, GameTime gameTime)
        {
            float distance = direction * stats.MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position = new Vector2(Position.X + distance, Position.Y);
            if (Position.X <= leftBoundary || Position.X >= rightBoundary)
            {
                Position = new Vector2(MathHelper.Clamp(Position.X, leftBoundary, rightBoundary), Position.Y);
                directionTimer = 0;
                State.ChangeDirection();
            }
        }

        public void Hop()
        {
            physics.Jump(HopSpeed);
        }

        private void UpdateTimers(GameTime gameTime)
        {
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
            directionTimer += elapsed;
            hopTimer += elapsed;
            if (directionTimer >= stats.DirectionChangeSeconds)
            {
                directionTimer = 0;
                State.ChangeDirection();
            }
            if (hopTimer >= stats.HopSeconds)
            {
                hopTimer = 0;
                State.Hop();
            }
        }
    }
}
