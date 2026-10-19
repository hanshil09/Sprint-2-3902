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
            bool immediate = kind == ItemKind.HealthCell || kind == ItemKind.ShieldBattery;
            int frameIndex = kind == ItemKind.Medkit || kind == ItemKind.HealthCell ? 0 : 1;
            Rectangle frame = new Rectangle(frameIndex * ItemSize, 0, ItemSize, ItemSize);
            return new TextureRegionSprite(itemSpriteSheet, frame, ItemSize, ItemSize, immediate ? Color.Cyan : Color.White);
        }
    }
}
 
