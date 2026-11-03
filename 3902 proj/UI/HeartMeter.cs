using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Sprites;

namespace TransformersGame.UI
{
    // A fixed number of hearts represents the current fraction of maximum health.
    internal static class HeartMeter
    {
        private const int PixelSize = 3;
        private const int HeartWidth = 9;
        private const int HeartSpacing = 6;
        private static readonly Color EmptyColor = new Color(65, 45, 55);
        private static readonly Color OutlineColor = new Color(10, 12, 20);
        private static readonly string[] Shape =
        {
            "011001100",
            "111111110",
            "111111111",
            "111111111",
            "011111110",
            "001111100",
            "000111000",
            "000010000"
        };

        public static void Draw(SpriteBatch spriteBatch, Vector2 position, int health, int maximumHealth,
            int heartCount, Color fillColor)
        {
            float fraction = maximumHealth > 0 ? MathHelper.Clamp((float)health / maximumHealth, 0f, 1f) : 0f;
            float filledHearts = fraction * heartCount;
            for (int heart = 0; heart < heartCount; heart++)
            {
                int left = (int)position.X + heart * (HeartWidth * PixelSize + HeartSpacing);
                float filledColumns = MathHelper.Clamp(filledHearts - heart, 0f, 1f) * HeartWidth;
                for (int row = 0; row < Shape.Length; row++)
                {
                    for (int column = 0; column < HeartWidth; column++)
                    {
                        if (Shape[row][column] != '1') continue;
                        Rectangle pixel = new Rectangle(left + column * PixelSize, (int)position.Y + row * PixelSize,
                            PixelSize, PixelSize);
                        spriteBatch.Draw(SpriteAtlas.Texture,
                            new Rectangle(pixel.X - 1, pixel.Y - 1, PixelSize + 2, PixelSize + 2),
                            SpriteAtlas.WhitePixel, OutlineColor);
                    }
                }
                for (int row = 0; row < Shape.Length; row++)
                {
                    for (int column = 0; column < HeartWidth; column++)
                    {
                        if (Shape[row][column] != '1') continue;
                        Rectangle pixel = new Rectangle(left + column * PixelSize, (int)position.Y + row * PixelSize,
                            PixelSize, PixelSize);
                        spriteBatch.Draw(SpriteAtlas.Texture, pixel, SpriteAtlas.WhitePixel, EmptyColor);
                        int fillWidth = (int)(MathHelper.Clamp(filledColumns - column, 0f, 1f) * PixelSize);
                        if (fillWidth > 0)
                        {
                            pixel.Width = fillWidth;
                            spriteBatch.Draw(SpriteAtlas.Texture, pixel, SpriteAtlas.WhitePixel, fillColor);
                        }
                    }
                }
            }
        }
    }
}
