using System;
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
        private const int ScreenHeight = 540;
        private const int TileSize = BlockSpriteFactory.BlockSize;

        private static readonly Vector2 BlockShowcasePosition = new Vector2(912, 16);
        private static readonly Vector2 ItemShowcasePosition = new Vector2(864, 16);

        private readonly LevelData data;
        private readonly List<IGameObject> objs;
        private readonly List<IBlock> blocks;
        private readonly List<IItem> items;

        public Level(LevelData data)
        {
            ArgumentNullException.ThrowIfNull(data);
            this.data = data;
            objs = new List<IGameObject>();
            blocks = new List<IBlock>();
            items = new List<IItem>();

            foreach (Placement<BlockKind> placement in data.Blocks)
            {
                ISprite sprite = BlockSpriteFactory.Instance.CreateBlockSprite(placement.Kind);
                blocks.Add(new Block(CellPosition(placement.Column, placement.Row), sprite, true));
            }

            foreach (Placement<ItemKind> placement in data.Items)
            {
                items.Add(CreateItem(placement.Kind, CellPosition(placement.Column, placement.Row)));
            }

            PlayerStart = CellPosition(data.PlayerColumn, data.PlayerRow);
        }

        public IReadOnlyList<IBlock> Blocks => blocks;

        public IReadOnlyList<IItem> Items => items;

        public Vector2 PlayerStart { get; }

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
            foreach (Placement<EnemyKind> placement in data.Enemies)
            {
                Vector2 start = CellPosition(placement.Column, placement.Row);
                cycler.Add(new Enemy(start, blocks, CreateEnemyConfiguration(placement.Kind), player, projectiles));
            }
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
            objs.Clear();
            foreach (IBlock block in blocks)
            {
                block.Update(gameTime);
                objs.Add(block);
            }
            foreach (IItem item in items)
            {
                item.Update(gameTime);
                objs.Add(item);
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

        public List<IGameObject> GetOnScreenObjects()
        {
            return objs;
        }

        private Vector2 CellPosition(int column, int row)
        {
            return new Vector2(column * TileSize, ScreenHeight - (data.Rows - row) * TileSize);
        }

        private static Block CreateShowcaseBlock(BlockKind kind)
        {
            return new Block(BlockShowcasePosition, BlockSpriteFactory.Instance.CreateBlockSprite(kind), false);
        }

        private static IItem CreateItem(ItemKind kind, Vector2 position)
        {
            ISprite sprite = ItemSpriteFactory.Instance.CreateItemSprite(kind);
            switch (kind)
            {
                case ItemKind.Medkit:
                    return new Medkit(position, sprite);
                case ItemKind.Shield:
                    return new Shield(position, sprite);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "No item is defined for this kind.");
            }
        }

        private static EnemyConfiguration CreateEnemyConfiguration(EnemyKind kind)
        {
            switch (kind)
            {
                case EnemyKind.Flyer:
                    return new EnemyConfiguration(kind, new EnemyStats(60f, 110f, 280f, 330f, 2.5), new SwoopBehavior());
                case EnemyKind.Crawler:
                    return new EnemyConfiguration(kind, new EnemyStats(50f, 100f, 240f, 120f, 1.0), new StalkBehavior());
                case EnemyKind.Hopper:
                    return new EnemyConfiguration(kind, new EnemyStats(60f, 80f, 320f, 170f, 0.9), new LeapBehavior());
                case EnemyKind.Beetle:
                    return new EnemyConfiguration(kind, new EnemyStats(90f, 60f, 360f, 260f, 1.4), new ChargeBehavior());
                case EnemyKind.Waver:
                    return new EnemyConfiguration(kind, new EnemyStats(70f, 0f, 420f, 95f, 0.0), new DriftBehavior());
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "No enemy is defined for this kind.");
            }
        }
    }
}