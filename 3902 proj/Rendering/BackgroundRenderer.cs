using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArmadilloGame.Rendering
{
    internal sealed class BackgroundRenderer
    {
        private const int TileSize = 8;

        private readonly Texture2D backgroundTexture;

        public BackgroundRenderer(Texture2D backgroundTexture)
        {
            ArgumentNullException.ThrowIfNull(backgroundTexture);

            this.backgroundTexture = backgroundTexture;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle viewBounds, int roomIndex)
        {
            ArgumentNullException.ThrowIfNull(spriteBatch);

            Color tint = GetRoomTint(roomIndex);

            int startX = (int)Math.Floor(viewBounds.Left / (double)TileSize) * TileSize;
            int startY = (int)Math.Floor(viewBounds.Top / (double)TileSize) * TileSize;

            for (int y = startY; y < viewBounds.Bottom + TileSize; y += TileSize)
            {
                for (int x = startX; x < viewBounds.Right + TileSize; x += TileSize)
                {
                    spriteBatch.Draw(
                        backgroundTexture,
                        new Rectangle(x, y, TileSize, TileSize),
                        tint);
                }
            }
        }

        private static Color GetRoomTint(int roomIndex)
        {
            return roomIndex switch
            {
                0 => new Color(190, 150, 110), // Frontier Hub - warm brown
                1 => new Color(220, 145, 90),  // Dusty orange
                2 => new Color(130, 165, 110), // Muted green
                3 => new Color(105, 145, 190), // Blue
                4 => new Color(155, 120, 180), // Purple
                5 => new Color(105, 165, 160), // Teal
                6 => new Color(185, 105, 100), // Red
                7 => new Color(95, 90, 85),    // Fortress - charcoal
                _ => Color.White
            };
        }
    }
}