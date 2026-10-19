using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.UI
{
    // T/Y, U/I and O/P cycle through every block/object, item and enemy kind in the game.
    // The preview panel appears for a few seconds whenever a selection changes.
    internal sealed class ObjectShowcase
    {
        private const double VisibleSeconds = 4;
        private const int ColumnWidth = 104;
        private static readonly Rectangle Panel = new Rectangle(628, 80, 3 * ColumnWidth + 8, 118);

        private readonly GameObjectCycler[] cyclers;
        private readonly IGameObject[] lastShown = new IGameObject[3];
        private double visibleTime;

        public ObjectShowcase()
        {
            Blocks = new GameObjectCycler();
            Items = new GameObjectCycler();
            Enemies = new GameObjectCycler();
            cyclers = new[] { Blocks, Items, Enemies };
            FillBlocks();
            FillItems();
            FillEnemies();
            Array.Copy(CurrentEntries(), lastShown, 3);
        }

        public GameObjectCycler Blocks { get; }

        public GameObjectCycler Items { get; }

        public GameObjectCycler Enemies { get; }

        public bool IsVisible => visibleTime > 0;

        public void Update(GameTime gameTime)
        {
            visibleTime -= gameTime.ElapsedGameTime.TotalSeconds;
            IGameObject[] current = CurrentEntries();
            for (int i = 0; i < current.Length; i++)
            {
                if (!ReferenceEquals(current[i], lastShown[i]))
                {
                    visibleTime = VisibleSeconds;
                }
                lastShown[i] = current[i];
                cyclers[i].Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch, ControlsOverlay overlay)
        {
            if (!IsVisible)
            {
                return;
            }
            overlay.DrawPanel(spriteBatch, Panel);
            string[] headings = { "T/Y BLOCK", "U/I ITEM", "O/P ENEMY" };
            for (int i = 0; i < cyclers.Length; i++)
            {
                int left = Panel.X + 8 + i * ColumnWidth;
                overlay.DrawSmallText(spriteBatch, headings[i], left + 4, Panel.Y + 6, true);
                if (cyclers[i].CurrentObject is ShowcaseEntry entry)
                {
                    entry.Position = new Vector2(left + (ColumnWidth - entry.Width) / 2, Panel.Y + 26 + (64 - entry.Height) / 2);
                    entry.Draw(spriteBatch);
                    overlay.DrawSmallText(spriteBatch, entry.Name, left + 4, Panel.Bottom - 22, false);
                }
            }
        }

        private IGameObject[] CurrentEntries() => new[] { Blocks.CurrentObject, Items.CurrentObject, Enemies.CurrentObject };

        private void FillBlocks()
        {
            BlockSpriteFactory factory = BlockSpriteFactory.Instance;
            foreach (BlockKind kind in Enum.GetValues<BlockKind>())
            {
                Blocks.Add(new ShowcaseEntry(kind + " Block", factory.CreateBlockSprite(kind)));
            }
            Blocks.Add(new ShowcaseEntry("Moving Platform", factory.CreatePlatformTileSprite(false)));
            Blocks.Add(new ShowcaseEntry("Elevator", factory.CreatePlatformTileSprite(true)));
            Blocks.Add(new ShowcaseEntry("Spring Pad", factory.CreateSpringSprite(SpringPose.Idle)));
            Blocks.Add(new ShowcaseEntry("Beacon (locked)", factory.CreateGoalSprite(false)));
            Blocks.Add(new ShowcaseEntry("Beacon (online)", factory.CreateGoalSprite(true)));
            Blocks.Add(new ShowcaseEntry("Next-Room Door", factory.CreateDoorSprite(1)));
            Blocks.Add(new ShowcaseEntry("Return Door", factory.CreateDoorSprite(-1)));
        }

        private void FillItems()
        {
            foreach (ItemKind kind in Enum.GetValues<ItemKind>())
            {
                Items.Add(new ShowcaseEntry(ItemName(kind), ItemSpriteFactory.Instance.CreateItemSprite(kind)));
            }
        }

        private void FillEnemies()
        {
            foreach (EnemyKind kind in Enum.GetValues<EnemyKind>())
            {
                ISprite sprite = EnemySpriteFactory.Instance.CreateEnemySprite(kind, Color.White, true);
                Enemies.Add(new ShowcaseEntry(kind == EnemyKind.Warlord ? "Warlord (boss)" : kind.ToString(), sprite));
            }
        }

        private static string ItemName(ItemKind kind)
        {
            return kind switch
            {
                ItemKind.HealthCell => "Health Cell",
                ItemKind.ShieldBattery => "Shield Battery",
                ItemKind.SparkCore => "Spark Core",
                ItemKind.TurboBoost => "Turbo Boost",
                ItemKind.Energon => "Energon Cube",
                ItemKind.BombPack => "Bomb Pack",
                ItemKind.MaxHealthUp => "Max Health Up",
                _ => kind.ToString()
            };
        }
    }
}
