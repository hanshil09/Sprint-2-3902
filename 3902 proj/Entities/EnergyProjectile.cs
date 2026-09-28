using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    public class EnergyProjectile : IProjectile
    {
        private const double MaximumLifetime = 1500;

        private ISprite sprite;
        private Vector2 velocity;
        private double lifetime;

        public EnergyProjectile(Vector2 position, Vector2 velocity, ISprite sprite)
        {
            Position = position;
            this.velocity = velocity;
            this.sprite = sprite;
            lifetime = 0;
            IsActive = true;
        }

        public Vector2 Position { get; set; }

        public bool IsActive { get; private set; }

        public void Update(GameTime gameTime)
        {
            Position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
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
