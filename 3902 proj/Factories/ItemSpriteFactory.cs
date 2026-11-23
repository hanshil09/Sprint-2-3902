using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;
using ArmadilloGame.Sprites;

namespace ArmadilloGame.Factories
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
        private Texture2D ShotgunCall;
        private Texture2D keySprite;
        private Texture2D shotgunWeapon;

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
            ShotgunCall = content.Load<Texture2D>("Sprites/Shotgun-call");
            keySprite = content.Load<Texture2D>("Sprites/key");
            shotgunWeapon = content.Load<Texture2D>("Sprites/Shotgun-weapon");
        }

        public ISprite CreateItemSprite(ItemKind kind)
        {
            if (kind == ItemKind.ShotgunCall)
            {
                return new TextureRegionSprite(ShotgunCall, new Rectangle(0, 0, 32, 32), ItemSize, ItemSize, Color.White);
            }
            if (kind == ItemKind.Key)
            {
                return new TextureRegionSprite(keySprite, new Rectangle(0, 0, 32, 32), ItemSize, ItemSize, Color.White);
            }
            ISprite weaponSprite = CreateWeaponPickupSprite(kind);
            if (weaponSprite != null)
            {
                return weaponSprite;
            }
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

        // Weapon pickups: the shotgun has its own sprite; the pistol and molotov reuse projectile
        // art as placeholders until dedicated pickup sprites exist.
        private ISprite CreateWeaponPickupSprite(ItemKind kind)
        {
            switch (kind)
            {
                case ItemKind.ShotgunPickup:
                    return new TextureRegionSprite(shotgunWeapon, new Rectangle(0, 0, 64, 32), ItemSize, ItemSize, Color.White);
                case ItemKind.PistolPickup:
                    return ProjectileSpriteFactory.Instance.CreateShotSprite(false);
                case ItemKind.MolotovPickup:
                    return ProjectileSpriteFactory.Instance.CreateBombSprite();
                default:
                    return null;
            }
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
