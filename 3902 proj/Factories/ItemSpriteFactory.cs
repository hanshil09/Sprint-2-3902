using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;

namespace TransformersGame.Factories
{
    internal class ItemSpriteFactory
    {
        public const int ItemSize = 32;
        private const double ItemFrameTime = 140;
        private const double AuraFrameTime = 90;
        private const int AuraSize = 96;
        private const int TrailWidth = 48;
        private const int TrailHeight = 16;

        private static ItemSpriteFactory instance = new ItemSpriteFactory();

        private Texture2D itemSpriteSheet;
        private Texture2D objectSpriteSheet;

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
            objectSpriteSheet = content.Load<Texture2D>(SpriteAtlas.AssetName);
        }

        public ISprite CreateItemSprite(ItemKind kind)
        {
            Rectangle[] atlasFrames = AtlasFrames(kind);
            if (atlasFrames != null)
            {
                return new AnimatedSprite(objectSpriteSheet, atlasFrames, ItemSize, ItemSize, Color.White, ItemFrameTime);
            }
            bool immediate = kind == ItemKind.HealthCell || kind == ItemKind.ShieldBattery;
            int frameIndex = kind == ItemKind.Medkit || kind == ItemKind.HealthCell ? 0 : 1;
            Rectangle frame = new Rectangle(frameIndex * ItemSize, 0, ItemSize, ItemSize);
            return new TextureRegionSprite(itemSpriteSheet, frame, ItemSize, ItemSize, immediate ? Color.Cyan : Color.White);
        }

        public ISprite CreateAuraSprite()
        {
            return new AnimatedSprite(objectSpriteSheet, SpriteAtlas.Aura, AuraSize, AuraSize, Color.White, AuraFrameTime);
        }

        public ISprite CreateSpeedTrailSprite(bool facingLeft)
        {
            SpriteEffects effects = facingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            return new TextureRegionSprite(objectSpriteSheet, SpriteAtlas.SpeedTrail, TrailWidth, TrailHeight, Color.White, effects);
        }

        // New Sprint 3 items are animated frames on the shared sprite sheet; older items keep items.png.
        private static Rectangle[] AtlasFrames(ItemKind kind)
        {
            return kind switch
            {
                ItemKind.SparkCore => SpriteAtlas.SparkCore,
                ItemKind.TurboBoost => SpriteAtlas.TurboBoost,
                ItemKind.Energon => SpriteAtlas.Energon,
                ItemKind.BombPack => SpriteAtlas.BombPack,
                ItemKind.MaxHealthUp => SpriteAtlas.MaxHealth,
                _ => null
            };
        }
    }
}
