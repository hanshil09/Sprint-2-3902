using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    internal class EnemyProjectile : IProjectile
    {
        private const double MaximumLifetime = 3000;
        private const float Speed = 220f;

        private readonly ISprite sprite;
        private readonly Vector2 velocity;
        private double lifetime;

        public EnemyProjectile(Vector2 center, Vector2 target, ISprite sprite)
        {
            this.sprite = sprite;
            Position = center - new Vector2(sprite.Width / 2f, sprite.Height / 2f);
            PreviousPosition = Position;
            Vector2 direction = target - center;
            velocity = direction.LengthSquared() == 0 ? Vector2.Zero : Vector2.Normalize(direction) * Speed;
            IsActive = true;
        }

        public Vector2 Position { get; set; }

        public Vector2 PreviousPosition { get; private set; }

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);

        public bool IsActive { get; private set; }

        public void Update(GameTime gameTime)
        {
            PreviousPosition = Position;
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += velocity * elapsed;
            lifetime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (lifetime >= MaximumLifetime)
            {
                IsActive = false;
            }
            sprite.Update(gameTime);
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, Position);
        }
    }
}
