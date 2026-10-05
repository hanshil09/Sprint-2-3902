using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Behaviors;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    public class Level : ILevel
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
        private static readonly Vector2 FlyerStart = new Vector2(260, 280);
        private static readonly Vector2 CrawlerStart = new Vector2(110, 460);
        private static readonly Vector2 HopperStart = new Vector2(456, 268);
        private static readonly Vector2 BeetleStart = new Vector2(728, 364);
        private static readonly Vector2 WaverStart = new Vector2(780, 280);
        private readonly List<IBlock> blocks;
        private readonly List<IItem> items;

        public Level()
        {
            blocks = new List<IBlock>();
            AddRow(BlockKind.Ground, 0, FloorTop, FloorLength);
            AddRow(BlockKind.Brick, 5, LowPlatformTop, 5);
            AddRow(BlockKind.Stone, 12, MiddlePlatformTop, 6);
            AddRow(BlockKind.Metal, 19, HighPlatformTop, 2);
            AddRow(BlockKind.Metal, 21, LowPlatformTop, 5);
            items = new List<IItem>();
            items.Add(new Medkit(MedkitStart, ItemSpriteFactory.Instance.CreateItemSprite(ItemKind.Medkit)));
            items.Add(new Shield(ShieldStart, ItemSpriteFactory.Instance.CreateItemSprite(ItemKind.Shield)));
        }

        public IReadOnlyList<IBlock> Blocks => blocks;

        public IReadOnlyList<IItem> Items => items;

        public void FillBlockCycler(IGameObjectCycler cycler)
        {
            cycler.Add(CreateShowcaseBlock(BlockKind.Ground));
            cycler.Add(CreateShowcaseBlock(BlockKind.Brick));
            cycler.Add(CreateShowcaseBlock(BlockKind.Stone));
            cycler.Add(CreateShowcaseBlock(BlockKind.Metal));
        }

        public void FillItemCycler(IGameObjectCycler cycler)
        {
            cycler.Add(new Medkit(ItemShowcasePosition, ItemSpriteFactory.Instance.CreateItemSprite(ItemKind.Medkit)));
            cycler.Add(new Shield(ItemShowcasePosition, ItemSpriteFactory.Instance.CreateItemSprite(ItemKind.Shield)));
        }

        public void FillEnemyCycler(IGameObjectCycler cycler, IPlayer player, IProjectileManager projectiles)
        {
            // Each preview demonstrates a distinct timer-driven behavior required for Sprint 2.
            EnemyConfiguration flyer = new EnemyConfiguration(
                EnemyKind.Flyer, new EnemyStats(60f, 110f, 280f, 330f, 2.5), new SwoopBehavior());
            EnemyConfiguration crawler = new EnemyConfiguration(
                EnemyKind.Crawler, new EnemyStats(50f, 100f, 240f, 120f, 1.0), new StalkBehavior());
            EnemyConfiguration hopper = new EnemyConfiguration(
                EnemyKind.Hopper, new EnemyStats(60f, 80f, 320f, 170f, 0.9), new LeapBehavior());
            EnemyConfiguration beetle = new EnemyConfiguration(
                EnemyKind.Beetle, new EnemyStats(90f, 60f, 360f, 260f, 1.4), new ChargeBehavior());
            EnemyConfiguration waver = new EnemyConfiguration(
                EnemyKind.Waver, new EnemyStats(70f, 0f, 420f, 95f, 0.0), new DriftBehavior());

            cycler.Add(new Enemy(FlyerStart, blocks, flyer, player, projectiles));
            cycler.Add(new Enemy(CrawlerStart, blocks, crawler, player, projectiles));
            cycler.Add(new Enemy(HopperStart, blocks, hopper, player, projectiles));
            cycler.Add(new Enemy(BeetleStart, blocks, beetle, player, projectiles));
            cycler.Add(new Enemy(WaverStart, blocks, waver, player, projectiles));
        }

        public void CollectItems(IPlayer player)
        {
            Rectangle playerBounds = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);
            foreach (IItem item in items.ToArray())
            {
                if (playerBounds.Intersects(item.Bounds))
                {
                    item.Collect(player);
                    items.Remove(item);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (IBlock block in blocks)
            {
                block.Update(gameTime);
            }
            foreach (IItem item in items)
            {
                item.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (IBlock block in blocks)
            {
                block.Draw(spriteBatch);
            }
            foreach (IItem item in items)
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
                blocks.Add(new Block(position, BlockSpriteFactory.Instance.CreateBlockSprite(kind), true));
            }
        }
    }
}
