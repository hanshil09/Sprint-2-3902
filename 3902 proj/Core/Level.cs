using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    public class Level
    {
        private const int FloorTop = 508;
        private const int FloorLength = 30;
        private const int LowPlatformTop = 412;
        private const int MiddlePlatformTop = 316;
        private const int HighPlatformTop = 220;

        private static readonly Vector2 ShowcasePosition = new Vector2(912, 16);
        private static readonly Vector2 FirstEnemyStart = new Vector2(420, 200);
        private static readonly Vector2 SecondEnemyStart = new Vector2(620, 400);

        public Level()
        {
            Blocks = new List<IBlock>();
            AddRow(BlockKind.Ground, 0, FloorTop, FloorLength);
            AddRow(BlockKind.Brick, 5, LowPlatformTop, 5);
            AddRow(BlockKind.Stone, 12, MiddlePlatformTop, 6);
            AddRow(BlockKind.Metal, 19, HighPlatformTop, 2);
            AddRow(BlockKind.Metal, 21, LowPlatformTop, 5);
        }

        public List<IBlock> Blocks { get; private set; }

        public static void FillBlockCycler(GameObjectCycler cycler)
        {
            cycler.Add(CreateShowcaseBlock(BlockKind.Ground));
            cycler.Add(CreateShowcaseBlock(BlockKind.Brick));
            cycler.Add(CreateShowcaseBlock(BlockKind.Stone));
            cycler.Add(CreateShowcaseBlock(BlockKind.Metal));
        }

        public void FillEnemyCycler(GameObjectCycler cycler)
        {
            cycler.Add(new Enemy(FirstEnemyStart, Blocks, Color.White));
            cycler.Add(new Enemy(SecondEnemyStart, Blocks, Color.OrangeRed));
        }

        public void Update(GameTime gameTime)
        {
            foreach (IBlock block in Blocks)
            {
                block.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (IBlock block in Blocks)
            {
                block.Draw(spriteBatch);
            }
        }

        private static Block CreateShowcaseBlock(BlockKind kind)
        {
            return new Block(ShowcasePosition, BlockSpriteFactory.Instance.CreateBlockSprite(kind), false);
        }

        private void AddRow(BlockKind kind, int firstColumn, int top, int length)
        {
            for (int column = firstColumn; column < firstColumn + length; column++)
            {
                Vector2 position = new Vector2(column * BlockSpriteFactory.BlockSize, top);
                Blocks.Add(new Block(position, BlockSpriteFactory.Instance.CreateBlockSprite(kind), true));
            }
        }
    }
}
