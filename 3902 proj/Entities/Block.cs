using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    public class Block : IBlock
    {
        private ISprite sprite;

        public Block(Vector2 position, ISprite sprite, bool isSolid)
        {
            Position = position;
            this.sprite = sprite;
            IsSolid = isSolid;
        }

        public Vector2 Position { get; set; }

        public bool IsSolid { get; private set; }

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
