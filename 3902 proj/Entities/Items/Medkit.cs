using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    internal class Medkit : IItem
    {
        private ISprite sprite;
        private bool isCollected;

        public Medkit(Vector2 position, ISprite sprite)
        {
            Position = position;
            this.sprite = sprite;
        }

        public void Collect(IPlayer player)
        {
            if (isCollected)
            {
                return;
            }
            player.AddItem(ItemKind.Medkit);
            isCollected = true;
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
 
