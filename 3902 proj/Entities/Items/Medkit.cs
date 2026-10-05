using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    public class Medkit : IItem
    {
        private ISprite sprite;

        public Medkit(Vector2 position, ISprite sprite)
        {
            Position = position;
            this.sprite = sprite;
        }

        public void Collect(IPlayer player)
        {
            
        }

        public Vector2 Position { get; set; }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);
            }
        }

        public void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, Position);
        }
    }
}
 
