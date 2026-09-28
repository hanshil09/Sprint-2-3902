using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;

namespace TransformersGame.Factories
{
    public class ItemSpriteFactory
    {
        public const int ItemSize = 32;

        private static ItemSpriteFactory instance = new ItemSpriteFactory();

        private Texture2D itemSpriteSheet;

        private ItemSpriteFactory()
        {
        }

        public static ItemSpriteFactory Instance
        {
            get
            {
                return instance;
            }
        }

        public void LoadAllTextures(ContentManager content)
        {
            itemSpriteSheet = content.Load<Texture2D>("Sprites/items");
        }

        public ISprite CreateItemSprite(ItemKind kind)
        {
            Rectangle frame = new Rectangle((int)kind * ItemSize, 0, ItemSize, ItemSize);
            return new TextureRegionSprite(itemSpriteSheet, frame, ItemSize, ItemSize, Color.White);
        }
    }
}
 
