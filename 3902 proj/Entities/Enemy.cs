using System;
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
        private const float WalkSpeed = 60f;
        private const float HopSpeed = 420f;
        private const double TurnDelay = 2000;
        private const int MinimumHopDelay = 1000;
        private const int MaximumHopDelay = 3000;
        private const int StartingHealth = 3;

        private static Random random = new Random();

        private Physics physics;
        private double turnTimer;
        private double hopTimer;
        private int health;

        public Enemy(Vector2 position, List<IBlock> blocks, Color tint)
        {
            Position = position;
            Tint = tint;
            physics = new Physics(blocks);
            health = StartingHealth;
            turnTimer = 0;
            hopTimer = random.Next(MinimumHopDelay, MaximumHopDelay);
            State = new RightWalkingEnemyState(this);
        }

        public IEnemyState State { get; set; }

        public ISprite Sprite { get; set; }

        public Color Tint { get; private set; }

        public Vector2 Position { get; set; }

        public void TakeDamage(int amount)
        {
            health -= amount;
            if (health <= 0)
            {
                State.BeDestroyed();
            }
        }

        public void Walk(float direction, GameTime gameTime)
        {
            float distance = direction * WalkSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position = new Vector2(Position.X + distance, Position.Y);
        }

        public void Hop()
        {
            physics.Jump(HopSpeed);
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

        private void UpdateTimers(GameTime gameTime)
        {
            double elapsed = gameTime.ElapsedGameTime.TotalMilliseconds;
            turnTimer += elapsed;
            if (turnTimer >= TurnDelay)
            {
                turnTimer = 0;
                State.ChangeDirection();
            }
            hopTimer -= elapsed;
            if (hopTimer <= 0)
            {
                hopTimer = random.Next(MinimumHopDelay, MaximumHopDelay);
                State.Hop();
            }
        }
    }
}
