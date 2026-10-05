using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    public class EnemyProjectile : IProjectile
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
            Vector2 direction = target - center;
            velocity = direction.LengthSquared() == 0 ? Vector2.Zero : Vector2.Normalize(direction) * Speed;
            IsActive = true;
        }

        public Vector2 Position { get; set; }

        public bool IsActive { get; private set; }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += velocity * elapsed;
            lifetime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (lifetime >= MaximumLifetime)
            {
                IsActive = false;
            }
            sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, Position);
        }
    }
}