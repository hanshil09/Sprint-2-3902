using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TransformersGame.Sprites
{
    internal static class SpriteAtlas
    {
        public const string AssetName = "Sprites/sprint3-objects";
        private const int Cell = 32;

        public static Texture2D Texture { get; private set; }

        public static Rectangle[] SparkCore => Row(0, 0, 4);
        public static Rectangle[] TurboBoost => Row(4 * Cell, 0, 4);
        public static Rectangle[] Energon => Row(0, Cell, 4);
        public static Rectangle[] BombPack => Row(4 * Cell, Cell, 2);
        public static Rectangle[] MaxHealth => Row(6 * Cell, Cell, 2);

        public static Rectangle PlatformTile => new Rectangle(0, 2 * Cell, Cell, 16);
        public static Rectangle ElevatorTile => new Rectangle(0, 2 * Cell + 16, Cell, 16);
        public static Rectangle SpringIdle => new Rectangle(Cell, 2 * Cell, Cell, Cell);
        public static Rectangle SpringCompressed => new Rectangle(2 * Cell, 2 * Cell, Cell, Cell);
        public static Rectangle SpringLaunched => new Rectangle(3 * Cell, 2 * Cell, Cell, Cell);
        public static Rectangle[] SpreadPellet => Row(4 * Cell + 8, 2 * Cell + 8, 2, 16);
        public static Rectangle[] BossFireball => Row(6 * Cell + 4, 2 * Cell + 4, 2, 24);

        public static Rectangle[] Boomerang => Row(0, 3 * Cell, 4);
        public static Rectangle AlloyBlock => new Rectangle(4 * Cell, 3 * Cell, Cell, Cell);
        public static Rectangle[] Aura => Row(5 * Cell, 3 * Cell, 2);
        public static Rectangle WhitePixel => new Rectangle(7 * Cell, 3 * Cell, 4, 4);
        public static Rectangle SpeedTrail => new Rectangle(7 * Cell, 3 * Cell + 16, Cell, 16);

        public static Rectangle GoalLocked => new Rectangle(0, 4 * Cell, Cell, 2 * Cell);
        public static Rectangle[] GoalActive => Row(Cell, 4 * Cell, 3, Cell, 2 * Cell);

        public static Rectangle[] BossIdle => Row(4 * Cell, 4 * Cell, 2, 2 * Cell);
        public static Rectangle BossAttack => new Rectangle(0, 6 * Cell, 2 * Cell, 2 * Cell);
        public static Rectangle[] BossEnraged => Row(2 * Cell, 6 * Cell, 2, 2 * Cell);
        public static Rectangle BossDefeated => new Rectangle(6 * Cell, 6 * Cell, 2 * Cell, 2 * Cell);

        public static void LoadAllTextures(ContentManager content)
        {
            Texture = content.Load<Texture2D>(AssetName);
        }

        // Frames laid out left-to-right; frames are 'size' square unless a height is given. Cells are 'step' apart.
        private static Rectangle[] Row(int x, int y, int count, int size = Cell, int height = 0)
        {
            int step = size <= Cell ? Cell : size;
            Rectangle[] frames = new Rectangle[count];
            for (int i = 0; i < count; i++)
            {
                frames[i] = new Rectangle(x + i * step, y, size, height == 0 ? size : height);
            }
            return frames;
        }
    }
}
