using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Sprites
{
    internal class TextureRegionSprite : ISprite
    {
        private Texture2D texture;
        private Rectangle sourceRectangle;
        private Color tint;
        private SpriteEffects effects;
        private readonly bool preserveAspect;

        public TextureRegionSprite(Texture2D texture, Rectangle sourceRectangle, int width, int height, Color tint,
            SpriteEffects effects = SpriteEffects.None, bool preserveAspect = false)
        {
            this.texture = texture;
            this.sourceRectangle = sourceRectangle;
            this.tint = tint;
            this.effects = effects;
            this.preserveAspect = preserveAspect;
            Width = width;
            Height = height;
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location) => Draw(spriteBatch, location, 1f);

        public void Draw(SpriteBatch spriteBatch, Vector2 location, float opacity)
        {
            Rectangle destinationRectangle = preserveAspect
                ? SpriteDraw.BottomCenterFit(sourceRectangle, location, Width, Height)
                : new Rectangle((int)location.X, (int)location.Y, Width, Height);
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, tint * opacity, 0f, Vector2.Zero, effects, 0f);
        }
    }
}
