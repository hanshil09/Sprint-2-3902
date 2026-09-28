using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;

namespace TransformersGame.Factories
{
    public class BlockSpriteFactory
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
            Rectangle frame = new Rectangle((int)kind * BlockSize, 0, BlockSize, BlockSize);
            return new TextureRegionSprite(blockSpriteSheet, frame, BlockSize, BlockSize, Color.White);
        }
    }
}
