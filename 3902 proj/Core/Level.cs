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

        private static readonly Vector2 BlockShowcasePosition = new Vector2(912, 16);
        private static readonly Vector2 ItemShowcasePosition = new Vector2(864, 16);
        private static readonly Vector2 MedkitStart = new Vector2(192, 380);
        private static readonly Vector2 ShieldStart = new Vector2(448, 284);
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
            Items = new List<IItem>();
            Items.Add(new Medkit(MedkitStart, ItemSpriteFactory.Instance.CreateItemSprite(ItemKind.Medkit)));
            Items.Add(new Shield(ShieldStart, ItemSpriteFactory.Instance.CreateItemSprite(ItemKind.Shield)));
        }

        public List<IBlock> Blocks { get; private set; }

        public List<IItem> Items { get; private set; }

        public static void FillBlockCycler(GameObjectCycler cycler)
        {
            cycler.Add(CreateShowcaseBlock(BlockKind.Ground));
            cycler.Add(CreateShowcaseBlock(BlockKind.Brick));
            cycler.Add(CreateShowcaseBlock(BlockKind.Stone));
            cycler.Add(CreateShowcaseBlock(BlockKind.Metal));
        }

        public static void FillItemCycler(GameObjectCycler cycler)
        {
            cycler.Add(new Medkit(ItemShowcasePosition, ItemSpriteFactory.Instance.CreateItemSprite(ItemKind.Medkit)));
            cycler.Add(new Shield(ItemShowcasePosition, ItemSpriteFactory.Instance.CreateItemSprite(ItemKind.Shield)));
        }

        public void FillEnemyCycler(GameObjectCycler cycler)
        {
            cycler.Add(new Enemy(FirstEnemyStart, Blocks, Color.White));
            cycler.Add(new Enemy(SecondEnemyStart, Blocks, Color.OrangeRed));
        }

        public void CollectItems(IPlayer player)
        {
            Rectangle playerBounds = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);
            foreach (IItem item in Items.ToArray())
            {
                if (playerBounds.Intersects(item.Bounds))
                {
                    item.Collect(player);
                    Items.Remove(item);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (IBlock block in Blocks)
            {
                block.Update(gameTime);
            }
            foreach (IItem item in Items)
            {
                item.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (IBlock block in Blocks)
            {
                block.Draw(spriteBatch);
            }
            foreach (IItem item in Items)
            {
                item.Draw(spriteBatch);
            }
        }

        private static Block CreateShowcaseBlock(BlockKind kind)
        {
            return new Block(BlockShowcasePosition, BlockSpriteFactory.Instance.CreateBlockSprite(kind), false);
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
