using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;

namespace TransformersGame.Factories
{
    internal class BlockSpriteFactory
    {
        public const int BlockSize = 32;
        private const int GoalHeight = 64;
        private const double GoalFrameTime = 160;

        private static BlockSpriteFactory instance = new BlockSpriteFactory();

        private Texture2D blockSpriteSheet;
        private Texture2D objectSpriteSheet;
        private Texture2D reinforcedHealth;
        private Texture2D reinforcedEnergon;
        private Texture2D additionalItemSprites;

        private BlockSpriteFactory()
        {
        }

        public static BlockSpriteFactory Instance
        {
            get
            {
                return instance;
            }
        }

        public void LoadAllTextures(ContentManager content)
        {
            blockSpriteSheet = content.Load<Texture2D>("Sprites/blocks2");
            objectSpriteSheet = content.Load<Texture2D>(SpriteAtlas.AssetName);
            reinforcedHealth = content.Load<Texture2D>("Sprites/reinforced-health");
            reinforcedEnergon = content.Load<Texture2D>("Sprites/reinforced-energon");
            additionalItemSprites = content.Load<Texture2D>("Sprites/additionalItemSprites");
        }

        public ISprite CreateBlockSprite(BlockKind kind)
        {
            if (kind == BlockKind.ReinforcedCrate)
            {
                return CreateSupplyCrateSprite(ItemKind.Energon, true);
            }
            if (kind == BlockKind.Alloy)
            {
                return new TextureRegionSprite(objectSpriteSheet, SpriteAtlas.AlloyBlock, BlockSize, BlockSize, Color.White);
            }
            int index = kind switch
            {
                BlockKind.Crate => 1,
                BlockKind.Hazard => 3,
                BlockKind.Wood => 4,
                BlockKind.Caution => 5,
                _ => (int)kind
            };
            Color tint = kind switch
            {
                BlockKind.Crate => Color.Orange,
                BlockKind.Hazard => Color.Red,
                _ => Color.White
            };
            Rectangle frame = new Rectangle(index * BlockSize, 0, BlockSize, BlockSize);
            return new TextureRegionSprite(blockSpriteSheet, frame, BlockSize, BlockSize, tint);
        }

        public ISprite CreateSupplyCrateSprite(ItemKind kind, bool reinforced = false)
        {
            Rectangle frame = reinforced
                ? new Rectangle(102, 15, 16, 17)
                : new Rectangle(71, 15, 16, 17);

            return new TextureRegionSprite(
                additionalItemSprites,
                frame,
                BlockSize,
                BlockSize,
                Color.White);
        }

        public ISprite CreateCrumblingSprite(CrumbleKind kind, bool warning, bool reforming)
        {
            Color tint;
            switch (kind)
            {
                case CrumbleKind.Fast: tint = Color.Coral; break;
                case CrumbleKind.Medium: tint = Color.Gold; break;
                default: tint = Color.MediumPurple; break;
            }
            if (warning) tint = Color.White;
            if (reforming) tint *= 0.35f;
            Rectangle frame = new Rectangle(BlockSize, 0, BlockSize, BlockSize);
            return new TextureRegionSprite(blockSpriteSheet, frame, BlockSize, BlockSize, tint);
        }

        public ISprite CreateDoorSprite(int direction, bool locked = false)
        {
            Rectangle frame = new Rectangle(130, 4, 22, 28);

            return new TextureRegionSprite(
                additionalItemSprites,
                frame,
                48,
                64,
                locked ? new Color(255, 120, 120) : Color.White);
        }

        public ISprite CreatePlatformTileSprite(bool elevator)
        {
            Rectangle frame = elevator ? SpriteAtlas.ElevatorTile : SpriteAtlas.PlatformTile;
            return new TextureRegionSprite(objectSpriteSheet, frame, BlockSize, MovingPlatform.Thickness, Color.White);
        }

        public ISprite CreateSpringSprite(SpringPose pose)
        {
            Rectangle frame = pose switch
            {
                SpringPose.Compressed => SpriteAtlas.SpringCompressed,
                SpringPose.Released => SpriteAtlas.SpringLaunched,
                _ => SpriteAtlas.SpringIdle
            };
            return new TextureRegionSprite(objectSpriteSheet, frame, BlockSize, BlockSize, Color.White);
        }

        public ISprite CreateGoalSprite(bool active)
        {
            return active
                ? new AnimatedSprite(objectSpriteSheet, SpriteAtlas.GoalActive, BlockSize, GoalHeight, Color.White, GoalFrameTime)
                : new TextureRegionSprite(objectSpriteSheet, SpriteAtlas.GoalLocked, BlockSize, GoalHeight, Color.White);
        }
    }
}
