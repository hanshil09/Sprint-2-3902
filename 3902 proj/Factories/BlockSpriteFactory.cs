using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;

namespace TransformersGame.Factories
{
    internal class BlockSpriteFactory
    {
        public const int BlockSize = 32;

        private static BlockSpriteFactory instance = new BlockSpriteFactory();

        private Texture2D blockSpriteSheet;

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
        }

        public ISprite CreateBlockSprite(BlockKind kind)
        {
            int index = kind == BlockKind.Crate || kind == BlockKind.CrateFloor ? 1
                : kind == BlockKind.Hazard || kind == BlockKind.HazardFloor ? 3 : (int)kind;
            Color tint = kind == BlockKind.Crate || kind == BlockKind.CrateFloor ? Color.Orange
                : kind == BlockKind.Hazard ? Color.Red
                : kind == BlockKind.HazardFloor ? Color.DarkOrange : Color.White;
            Rectangle frame = new Rectangle(index * BlockSize, 0, BlockSize, BlockSize);
            return new TextureRegionSprite(blockSpriteSheet, frame, BlockSize, BlockSize, tint);
        }

        public ISprite CreateDoorSprite(int direction)
        {
            Rectangle frame = new Rectangle(3 * BlockSize, 0, BlockSize, BlockSize);
            return new TextureRegionSprite(blockSpriteSheet, frame, 48, 64,
                direction > 0 ? Color.Gold : Color.Cyan);
        }
    }
}
