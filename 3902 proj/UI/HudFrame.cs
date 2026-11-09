using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Sprites;

namespace TransformersGame.UI
{
    internal static class HudFrame
    {
        // Character portrait.
        private const int PortraitX = 18;
        private const int PortraitY = 12;
        private const int PortraitSize = 44;

        private static readonly Rectangle PortraitSource =
            new Rectangle(878, 78, 78, 78);

        private static readonly Color BackgroundColor = new Color(55, 34, 23, 250);
        private static readonly Color HighlightColor = new Color(255, 225, 190, 65);
        private static readonly Color BorderColor = new Color(190, 139, 73, 210);
        private static readonly Color PortraitOutlineColor = new Color(25, 14, 9);

        public static void Draw(SpriteBatch spriteBatch, Rectangle bounds, Texture2D portrait, Color accent)
        {
            Fill(spriteBatch, bounds, BackgroundColor);
            Fill(spriteBatch, new Rectangle(bounds.X, bounds.Y, bounds.Width, 1), HighlightColor);
            Fill(spriteBatch, new Rectangle(bounds.X, bounds.Bottom - 2, bounds.Width, 2), BorderColor);
            Fill(spriteBatch, new Rectangle(bounds.X, bounds.Y, 3, bounds.Height), accent);
            DrawPortrait(spriteBatch, portrait, accent);
        }

        private static void DrawPortrait(SpriteBatch spriteBatch, Texture2D portrait, Color accent)
        {
            Rectangle destination = new Rectangle(PortraitX, PortraitY, PortraitSize, PortraitSize);
            Rectangle outline = destination;
            outline.Inflate(2, 2);
            Fill(spriteBatch, outline, PortraitOutlineColor);

            Rectangle frame = destination;
            frame.Inflate(1, 1);
            Fill(spriteBatch, frame, accent);
            spriteBatch.Draw(portrait, destination, PortraitSource, Color.White);
        }

        public static void Fill(SpriteBatch spriteBatch, Rectangle bounds, Color color)
        {
            spriteBatch.Draw(SpriteAtlas.Texture, bounds, SpriteAtlas.WhitePixel, color);
        }
    }
}
