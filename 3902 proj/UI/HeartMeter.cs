
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TransformersGame.Sprites;

// A fixed number of hearts represents the current fraction of maximum health.
internal static class HeartMeter
{
    private const int PixelSize = 2;
    private const int HeartWidth = 9;
    private const int HeartHeight = 8;
    private const int HeartSpacing = 5;

    private static readonly Color EmptyColor = new Color(75, 55, 30);
    private static readonly Color OutlineColor = new Color(105, 69, 39);
    private static readonly Color FillColor = new Color(235, 180, 55);
    private static readonly Color HighlightColor = new Color(255, 220, 115);

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

    public static void Draw(
        SpriteBatch spriteBatch,
        Vector2 position,
        int health,
        int maximumHealth,
        int heartCount,
        Color fillColor)
    {
        float fraction = maximumHealth > 0
            ? MathHelper.Clamp((float)health / maximumHealth, 0f, 1f)
            : 0f;

        float filledHearts = fraction * heartCount;

        for (int heart = 0; heart < heartCount; heart++)
        {
            int left = (int)position.X +
                heart * (HeartWidth * PixelSize + HeartSpacing);

            int top = (int)position.Y;

            float filledColumns =
                MathHelper.Clamp(filledHearts - heart, 0f, 1f) * HeartWidth;

            // Draw a single clean outline silhouette.
            for (int row = 0; row < HeartHeight; row++)
            {
                for (int column = 0; column < HeartWidth; column++)
                {
                    if (Shape[row][column] != '1')
                        continue;

                    Rectangle pixel = new Rectangle(
                        left + column * PixelSize - 1,
                        top + row * PixelSize - 1,
                        PixelSize + 2,
                        PixelSize + 2);

                    spriteBatch.Draw(
                        SpriteAtlas.Texture,
                        pixel,
                        SpriteAtlas.WhitePixel,
                        OutlineColor);
                }
            }

            // Draw the heart interior.
            for (int row = 0; row < HeartHeight; row++)
            {
                for (int column = 0; column < HeartWidth; column++)
                {
                    if (Shape[row][column] != '1')
                        continue;

                    Rectangle pixel = new Rectangle(
                        left + column * PixelSize,
                        top + row * PixelSize,
                        PixelSize,
                        PixelSize);

                    spriteBatch.Draw(
                        SpriteAtlas.Texture,
                        pixel,
                        SpriteAtlas.WhitePixel,
                        EmptyColor);

                    int fillWidth = (int)MathF.Round(
                        MathHelper.Clamp(filledColumns - column, 0f, 1f)
                        * PixelSize);

                    if (fillWidth > 0)
                    {
                        pixel.Width = fillWidth;

                        Color currentFill = row == 1 && column < filledColumns
                            ? HighlightColor
                            : FillColor;

                        spriteBatch.Draw(
                            SpriteAtlas.Texture,
                            pixel,
                            SpriteAtlas.WhitePixel,
                            currentFill);
                    }
                }
            }
        }
    }
}
