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
            blockSpriteSheet = content.Load<Texture2D>("Sprites/blocks");
            objectSpriteSheet = content.Load<Texture2D>(SpriteAtlas.AssetName);
        }

        public ISprite CreateBlockSprite(BlockKind kind)
        {
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
            Color tint = kind == BlockKind.Crate ? Color.Orange : kind == BlockKind.Hazard ? Color.Red : Color.White;
            Rectangle frame = new Rectangle(index * BlockSize, 0, BlockSize, BlockSize);
            return new TextureRegionSprite(blockSpriteSheet, frame, BlockSize, BlockSize, tint);
        }

        public ISprite CreateDoorSprite(int direction)
        {
            Rectangle frame = new Rectangle(3 * BlockSize, 0, BlockSize, BlockSize);
            return new TextureRegionSprite(blockSpriteSheet, frame, 48, 64,
                direction > 0 ? Color.Gold : Color.Cyan);
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
