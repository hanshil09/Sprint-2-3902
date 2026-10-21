using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Factories;
using TransformersGame.Interfaces;
using TransformersGame.Core;
using System;
using System.Collections.Generic;

namespace TransformersGame.Entities
{
    internal class Bomb : IProjectile
    {
        private const double FuseTime = 1200;
        private const double ExplosionTime = 320;

        private ISprite sprite;
        private double timer;
        private bool hasExploded;
        private readonly Physics physics;
        private readonly Func<ISprite> createExplosion;
        private readonly System.Collections.Generic.HashSet<IEnemy> damagedEnemies = new();

        public Bomb(Vector2 position, ISprite sprite, IReadOnlyList<IBlock> blocks = null,
            float gravity = Physics.DefaultGravity, Func<ISprite> createExplosion = null)
        {
            Position = position;
            PreviousPosition = position;
            this.sprite = sprite;
            physics = new Physics(blocks ?? Array.Empty<IBlock>(), gravity);
            this.createExplosion = createExplosion ?? (() => ProjectileSpriteFactory.Instance.CreateExplosionSprite());
            timer = FuseTime;
            hasExploded = false;
            IsActive = true;
        }

        public Vector2 Position { get; set; }

        public Vector2 PreviousPosition { get; private set; }

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);

        public bool IsActive { get; private set; }

        public bool IsExploding => IsActive && hasExploded;

        public void DamageEnemy(IEnemy enemy)
        {
            if (IsExploding && !enemy.IsDestroyed && damagedEnemies.Add(enemy))
            {
                enemy.TakeDamage(1);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!IsActive) return;
            PreviousPosition = Position;
            if (!hasExploded)
            {
                Position = physics.Apply(Position, sprite.Width, sprite.Height, gameTime);
            }
            timer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer <= 0 && !hasExploded)
            {
                Explode();
            }
            else if (timer <= 0)
            {
                IsActive = false;
            }
            sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, Position);
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        private void Explode()
        {
            ISprite explosion = createExplosion();
            float x = Position.X + (sprite.Width - explosion.Width) / 2f;
            float y = Position.Y + sprite.Height - explosion.Height;
            Position = new Vector2(x, y);
            sprite = explosion;
            timer = ExplosionTime;
            hasExploded = true;
        }
    }
}
