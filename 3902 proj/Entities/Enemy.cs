
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    public class Enemy : IEnemy
    {
        public Enemy(Vector2 position, ISprite sprite)
        {
            Position = position;
            Sprite = sprite;
        }

        public ISprite Sprite { get; set; }
        public Vector2 Position { get; set; }

        private const float VerticalOffset = 40f;

        public int Width => Sprite.Width;
        public int Height => Sprite.Height;

        private const float MovementAmplitude = 12f;
        private const float MovementSpeed = 2f;

        private float movementTime = 0f;

        public void TakeDamage(int amount)
        {
            // Damage is not used in the idle-animation demonstration.
        }

        public void Update(GameTime gameTime)
        {
            movementTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float horizontalOffset =
                (float)System.Math.Sin(movementTime * MovementSpeed)
                * MovementAmplitude;

            Vector2 offset = new Vector2(
                horizontalOffset,
                VerticalOffset
            );

            Sprite.Draw(spriteBatch, Position + offset);
        }
    }
}