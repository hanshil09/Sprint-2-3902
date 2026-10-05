using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    public class EnergyProjectile : IProjectile
    {
        private const double MaximumLifetime = 3000;
        private const double angleValue = 0.6632;       //38 degrees, in radians

        private ISprite sprite;
        private Vector2 velocity;
        private double lifetime;
        private bool isAngled;

        public EnergyProjectile(Vector2 position, Vector2 velocity, bool angled, bool isLeft, ISprite sprite)
        {
            Position = position;
            isAngled = angled;
            this.sprite = sprite;
            lifetime = 0;
            IsActive = true;

            if(isAngled)
            {
                float angledShotX = (float)(velocity.X * Math.Cos(angleValue) + velocity.Y * Math.Sin(angleValue));
                float angledShotY = (float)(velocity.X * -1 * Math.Sin(angleValue) + velocity.Y * Math.Cos(angleValue));
                if(isLeft)
                {
                    angledShotY *= -1;
                }
                this.velocity = new Vector2(angledShotX, angledShotY);
            }
            else
            {
                this.velocity = velocity;
            }
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
