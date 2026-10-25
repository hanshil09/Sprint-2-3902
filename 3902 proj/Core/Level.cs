using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    internal class Level : ILevel
    {
        private const int ScreenHeight = 540;
        private const int ScreenWidth = 960;
        private const int TileSize = BlockSpriteFactory.BlockSize;

        private readonly LevelData data;
        private readonly List<IGameObject> objs;
        private readonly List<IBlock> blocks;
        private readonly List<IItem> items;
        private readonly List<Door> doors = new();
        private readonly List<IPlayerTrigger> triggers;
        private Rectangle playerSpawnBounds;

        public Level(LevelData data, Random random = null)
        {
            ArgumentNullException.ThrowIfNull(data);
            this.data = data;
            objs = new List<IGameObject>();
            blocks = new List<IBlock>();
            items = new List<IItem>();

            foreach (Placement<BlockKind> placement in data.Blocks)
            {
                blocks.Add(CreateBlock(placement));
            }
            if (random != null) CrumblingPlatformLayout.Apply(blocks, data, CellPosition, random);
            LevelObjects levelObjects = new LevelObjects(data.Objects, CellPosition, data.Gravity);
            blocks.AddRange(levelObjects.Platforms);
            triggers = levelObjects.Triggers;
            Goal = levelObjects.Goal;

            foreach (Placement<ItemKind> placement in data.Items)
            {
                items.Add(EntityFactory.CreateItem(placement.Kind, CellPosition(placement.Column, placement.Row)));
            }
            foreach (Placement<int> placement in data.Doors)
            {
                doors.Add(new Door(CellPosition(placement.Column, placement.Row), placement.Kind,
                    BlockSpriteFactory.Instance.CreateDoorSprite(placement.Kind)));
            }
            PlacePlayerStart();
        }

        public IReadOnlyList<IBlock> Blocks => blocks;

        public IReadOnlyList<IItem> Items => items;

        public Vector2 PlayerStart { get; private set; }

        public string Name => data.Name;

        public float Gravity => data.Gravity;

        // The end-of-level beacon, or null when this room has none.
        public Goal Goal { get; }

        public void FillEnemyCycler(IGameObjectCycler cycler, IPlayer player, IProjectileManager projectiles)
        {
            List<Rectangle> occupied = SolidBlockBounds();
            occupied.Add(playerSpawnBounds);
            foreach (Placement<EnemyKind> placement in data.Enemies)
            {
                Point size = EntityFactory.SpawnSize(placement.Kind);
                Vector2 start = FindSafeSpawn(CellPosition(placement.Column, placement.Row), size.X, size.Y, occupied);
                IEnemy enemy = placement.Kind == EnemyKind.Warlord
                    ? new Boss(start, player, projectiles)
                    : new Enemy(start, blocks, EntityFactory.CreateEnemyConfiguration(placement.Kind), player, projectiles, Gravity);
                cycler.Add(enemy);
                occupied.Add(new Rectangle((int)start.X, (int)start.Y, size.X, size.Y));
            }
        }

        // Returns the items picked up this frame so power-ups can be applied by the game.
        public IReadOnlyList<IItem> CollectItems(IPlayer player)
        {
            Rectangle playerBounds = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);
            List<IItem> collected = items.FindAll(item => playerBounds.Intersects(item.Bounds) && item.TryCollect(player));
            foreach (IItem item in collected)
            {
                items.Remove(item);
            }
            return collected;
        }

        public void Update(GameTime gameTime)
        {
            blocks.ForEach(block => block.Update(gameTime));
            ReleaseCrateItems();
            items.ForEach(item => item.Update(gameTime));
            triggers.ForEach(trigger => trigger.Update(gameTime));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            blocks.ForEach(block => block.Draw(spriteBatch));
            triggers.ForEach(trigger => trigger.Draw(spriteBatch));
            items.ForEach(item => item.Draw(spriteBatch));
            doors.ForEach(door => door.Draw(spriteBatch));
        }

        public IReadOnlyList<IGameObject> GetOnScreenObjects()
        {
            objs.Clear();
            objs.AddRange(blocks);
            objs.AddRange(items);
            objs.AddRange(doors);
            objs.AddRange(triggers);
            return objs.AsReadOnly();
        }

        private IBlock CreateBlock(Placement<BlockKind> placement)
        {
            Vector2 position = CellPosition(placement.Column, placement.Row);
            foreach (Placement<ItemKind> drop in data.CrateDrops)
            {
                if (placement.Kind == BlockKind.Crate && drop.Column == placement.Column && drop.Row == placement.Row)
                {
                    ISprite crateSprite = BlockSpriteFactory.Instance.CreateSupplyCrateSprite(drop.Kind);
                    return new SupplyCrate(position, crateSprite, drop.Kind);
                }
            }
            ISprite sprite = BlockSpriteFactory.Instance.CreateBlockSprite(placement.Kind);
            return new Block(position, sprite, placement.Kind != BlockKind.Hazard, placement.Kind);
        }

        private void ReleaseCrateItems()
        {
            foreach (IBlock block in blocks)
            {
                if (block is IItemDropBlock crate && crate.TryReleaseItem(out ItemKind kind))
                {
                    items.Add(EntityFactory.CreateItem(kind, block.Position));
                }
            }
        }

        private void PlacePlayerStart()
        {
            ISprite playerSprite = PlayerSpriteFactory.Instance.CreateStandingRobotSprite(Direction.Right);
            PlayerStart = FindSafeSpawn(CellPosition(data.PlayerColumn, data.PlayerRow),
                playerSprite.Width, playerSprite.Height, SolidBlockBounds());
            playerSpawnBounds = new Rectangle((int)PlayerStart.X, (int)PlayerStart.Y, playerSprite.Width, playerSprite.Height);
        }

        private Vector2 CellPosition(int column, int row)
        {
            return new Vector2(column * TileSize, ScreenHeight - (data.Rows - row) * TileSize);
        }

        private List<Rectangle> SolidBlockBounds()
        {
            return blocks.FindAll(block => block.IsSolid).ConvertAll(block => block.Bounds);
        }

        // Nearest free position to the requested one, searched on a 16-pixel grid.
        private static Vector2 FindSafeSpawn(Vector2 requested, int width, int height, List<Rectangle> occupied)
        {
            const int searchStep = 16;
            float bestDistance = float.MaxValue;
            Vector2? best = null;
            for (int y = 0; y <= ScreenHeight - height; y += searchStep)
            {
                for (int x = 0; x <= ScreenWidth - width; x += searchStep)
                {
                    Rectangle candidate = new Rectangle(x, y, width, height);
                    float distance = Vector2.DistanceSquared(new Vector2(x, y), requested);
                    if (distance < bestDistance && !occupied.Exists(bounds => candidate.Intersects(bounds)))
                    {
                        bestDistance = distance;
                        best = new Vector2(x, y);
                    }
                }
            }
            return best ?? throw new InvalidOperationException("The level has no non-overlapping spawn position for an entity.");
        }
    }
}
