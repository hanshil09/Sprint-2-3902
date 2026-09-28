using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Sprites
{
    public class TextureRegionSprite : ISprite
    {
        private Texture2D texture;
        private Rectangle sourceRectangle;
        private Color tint;

        public TextureRegionSprite(Texture2D texture, Rectangle sourceRectangle, int width, int height, Color tint)
        {
            this.texture = texture;
            this.sourceRectangle = sourceRectangle;
            this.tint = tint;
            Width = width;
            Height = height;
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, Width, Height);
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, tint);
        }
    }
}
