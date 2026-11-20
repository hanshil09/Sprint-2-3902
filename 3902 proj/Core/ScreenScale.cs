using Microsoft.Xna.Framework;

namespace ArmadilloGame.Core
{
    // Converts window-pixel coordinates into the game's virtual 960x540 space.
    // The game renders at virtual resolution and is scaled up uniformly to the window,
    // so mouse input must be scaled back down before any gameplay or UI math uses it.
    internal static class ScreenScale
    {
        public static int VirtualWidth { get; private set; } = 960;
        public static int VirtualHeight { get; private set; } = 540;
        public static int WindowWidth { get; private set; } = 960;
        public static int WindowHeight { get; private set; } = 540;

        public static void Configure(int virtualWidth, int virtualHeight, int windowWidth, int windowHeight)
        {
            VirtualWidth = virtualWidth;
            VirtualHeight = virtualHeight;
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
        }

        public static Point ToVirtual(Point windowPosition)
        {
            float x = windowPosition.X * (float)VirtualWidth / WindowWidth;
            float y = windowPosition.Y * (float)VirtualHeight / WindowHeight;
            return new Point((int)x, (int)y);
        }
    }
}
