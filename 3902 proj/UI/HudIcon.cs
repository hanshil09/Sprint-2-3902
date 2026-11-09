using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Sprites;

namespace TransformersGame.UI
{
    internal static class HudIcon
    {
        private const int IconPixelSize = 2;
        private const int IconOutlineThickness = 1;
        private static readonly Color IconOutlineColor = new Color(105, 69, 39);

        // Pixel-art inventory icons.
        public static readonly string[] MedkitIcon =
        {
            "...XX...",
            "...XX...",
            "XXXXXXXX",
            "XXXXXXXX",
            "...XX...",
            "...XX...",
            "...XX...",
            "........"
        };

        public static readonly string[] ShieldIcon =
        {
            "..XXXX..",
            ".XXXXXX.",
            "XXXXXXXX",
            "XXXXXXXX",
            ".XXXXXX.",
            "..XXXX..",
            "...XX...",
            "........"
        };

        public static readonly string[] GemIcon =
        {
            "...XX...",
            "..XXXX..",
            ".XXXXXX.",
            "XXXXXXXX",
            ".XXXXXX.",
            "..XXXX..",
            "...XX...",
            "........"
        };

        public static readonly string[] BombIcon =
        {
            "....XX..",
            "...XX...",
            "..XXXX..",
            ".XXXXXX.",
            ".XXXXXX.",
            ".XXXXXX.",
            "..XXXX..",
            "...XX..."
        };

        public static Point Size(string[] shape) =>
            new Point(shape[0].Length * IconPixelSize, shape.Length * IconPixelSize);

        public static void Draw(
            SpriteBatch spriteBatch,
            string[] shape,
            Point position,
            Color color)
        {
            int x = position.X;
            int y = position.Y;
            int rows = shape.Length;
            int columns = shape[0].Length;

            // Draw a thick black outline around the outside silhouette.
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (shape[row][column] != 'X')
                        continue;

                    int pixelX = x + column * IconPixelSize;
                    int pixelY = y + row * IconPixelSize;

                    // Top edge.
                    if (row == 0 || shape[row - 1][column] != 'X')
                    {
                        Fill(
                            spriteBatch,
                            new Rectangle(
                                pixelX,
                                pixelY - IconOutlineThickness,
                                IconPixelSize,
                                IconOutlineThickness),
                            IconOutlineColor);
                    }

                    // Bottom edge.
                    if (row == rows - 1 || shape[row + 1][column] != 'X')
                    {
                        Fill(
                            spriteBatch,
                            new Rectangle(
                                pixelX,
                                pixelY + IconPixelSize,
                                IconPixelSize,
                                IconOutlineThickness),
                            IconOutlineColor);
                    }

                    // Left edge.
                    if (column == 0 || shape[row][column - 1] != 'X')
                    {
                        Fill(
                            spriteBatch,
                            new Rectangle(
                                pixelX - IconOutlineThickness,
                                pixelY,
                                IconOutlineThickness,
                                IconPixelSize),
                            IconOutlineColor);
                    }

                    // Right edge.
                    if (column == columns - 1 || shape[row][column + 1] != 'X')
                    {
                        Fill(
                            spriteBatch,
                            new Rectangle(
                                pixelX + IconPixelSize,
                                pixelY,
                                IconOutlineThickness,
                                IconPixelSize),
                            IconOutlineColor);
                    }
                }
            }

            DrawFill(spriteBatch, shape, position, color);
        }

        private static void DrawFill(SpriteBatch spriteBatch, string[] shape, Point position, Color color)
        {
            int rows = shape.Length;
            int columns = shape[0].Length;
            // Draw the colored icon pixels over the outline.
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (shape[row][column] != 'X')
                        continue;

                    Rectangle pixel = new Rectangle(
                        position.X + column * IconPixelSize,
                        position.Y + row * IconPixelSize,
                        IconPixelSize,
                        IconPixelSize);

                    spriteBatch.Draw(
                        SpriteAtlas.Texture,
                        pixel,
                        SpriteAtlas.WhitePixel,
                        color);
                }
            }
        }

        private static void Fill(SpriteBatch spriteBatch, Rectangle bounds, Color color)
        {
            spriteBatch.Draw(SpriteAtlas.Texture, bounds, SpriteAtlas.WhitePixel, color);
        }
    }
}
