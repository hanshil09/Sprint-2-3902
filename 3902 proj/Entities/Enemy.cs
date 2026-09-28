
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;
using TransformersGame.States;

namespace TransformersGame.Entities
{
    public class Enemy : IEnemy
    {
        private const float Gravity = 420f;
        private readonly float leftBoundary;
        private readonly float rightBoundary;
        private readonly float movementSpeed;
        private readonly double directionChangeSeconds;
        private readonly double hopSeconds;
        private float verticalVelocity;
        private double directionTimer;
        private double hopTimer;
        private int health = 1;

        public Enemy(
            Vector2 position,
            EnemyKind kind,
            float movementSpeed,
            float patrolDistance,
            double directionChangeSeconds,
            double hopSeconds)
        {
            Position = position;
            GroundY = position.Y;
            Kind = kind;
            Tint = Color.White;
            this.movementSpeed = movementSpeed;
            this.directionChangeSeconds = directionChangeSeconds;
            this.hopSeconds = hopSeconds;
            leftBoundary = position.X - patrolDistance;
            rightBoundary = position.X + patrolDistance;
            State = new LeftWalkingEnemyState(this);
        }

        public ISprite Sprite { get; set; }
        public IEnemyState State { get; set; }
        public EnemyKind Kind { get; }
        public Color Tint { get; }
        public Vector2 Position { get; set; }
        private float GroundY { get; }

        public int Width => Sprite.Width;
        public int Height => Sprite.Height;

        public void TakeDamage(int amount)
        {
            health = System.Math.Max(0, health - amount);
            if (health == 0)
            {
                State.BeDestroyed();
            }
        }

        public void Update(GameTime gameTime)
        {
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
            directionTimer += elapsed;
            hopTimer += elapsed;

            if (directionTimer >= directionChangeSeconds)
            {
                directionTimer = 0;
                State.ChangeDirection();
            }

            if (hopTimer >= hopSeconds)
            {
                hopTimer = 0;
                State.Hop();
            }

            State.Update(gameTime);
            Sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch, Position);
        }

        public void Walk(int direction, GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += new Vector2(direction * movementSpeed * elapsed, verticalVelocity * elapsed);
            verticalVelocity += Gravity * elapsed;

            if (Position.Y >= GroundY)
            {
                Position = new Vector2(Position.X, GroundY);
                verticalVelocity = 0;
            }

            if (Position.X <= leftBoundary || Position.X >= rightBoundary)
            {
                Position = new Vector2(MathHelper.Clamp(Position.X, leftBoundary, rightBoundary), Position.Y);
                directionTimer = 0;
                State.ChangeDirection();
            }
        }

        public void Hop()
        {
            if (Position.Y >= GroundY)
            {
                verticalVelocity = -150f;
            }
        }
    }
}
