using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Sprites;

namespace TransformersGame.UI
{
    // Draws a framed horizontal bar using the white pixel on the shared sprite sheet.
    internal static class HealthBar
    {
        private static readonly Color FrameColor = new Color(10, 12, 20, 230);
        private static readonly Color EmptyColor = new Color(70, 20, 30, 230);
        private const int Border = 3;

        public static void Draw(SpriteBatch spriteBatch, Rectangle area, float fraction, Color fill)
        {
            fraction = MathHelper.Clamp(fraction, 0f, 1f);
            Rectangle inner = new Rectangle(area.X + Border, area.Y + Border, area.Width - 2 * Border, area.Height - 2 * Border);
            Fill(spriteBatch, area, FrameColor);
            Fill(spriteBatch, inner, EmptyColor);
            Fill(spriteBatch, new Rectangle(inner.X, inner.Y, (int)(inner.Width * fraction), inner.Height), fill);
        }

        private static void Fill(SpriteBatch spriteBatch, Rectangle area, Color color)
        {
            spriteBatch.Draw(SpriteAtlas.Texture, area, SpriteAtlas.WhitePixel, color);
        }
    }
}
