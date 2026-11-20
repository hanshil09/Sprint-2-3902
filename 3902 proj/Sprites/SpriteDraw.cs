using System;
using Microsoft.Xna.Framework;

namespace ArmadilloGame.Sprites
{
    internal static class SpriteDraw
    {
        // Scales a source frame uniformly to fit the logical box's height, then anchors it to the
        // bottom-center of that box. Keeps proportions true and the feet planted while frames of
        // different sizes play, so nothing squishes or jumps around.
        public static Rectangle BottomCenterFit(Rectangle source, Vector2 location, int boxWidth, int boxHeight)
        {
            if (source.Height <= 0)
            {
                return new Rectangle((int)location.X, (int)location.Y, boxWidth, boxHeight);
            }
            float scale = (float)boxHeight / source.Height;
            int drawWidth = (int)MathF.Round(source.Width * scale);
            int drawHeight = boxHeight;
            int x = (int)location.X + (boxWidth - drawWidth) / 2;
            int y = (int)location.Y + (boxHeight - drawHeight);
            return new Rectangle(x, y, drawWidth, drawHeight);
        }
    }
}
