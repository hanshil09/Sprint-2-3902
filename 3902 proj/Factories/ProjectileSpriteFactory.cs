using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;

namespace TransformersGame.Factories
{
    internal class ProjectileSpriteFactory
    {
        private const double ShotFrameTime = 60;
        private const double ExplosionFrameTime = 80;
        private const int ShotWidth = 36;
        private const int ShotHeight = 26;
        private const int OrbSize = 28;
        private const int BombSize = 20;
        private const int ExplosionSize = 44;
        private const int BlastGrowthPerLevel = 18;
        private const int PelletSize = 16;
        private const int BoomerangSize = 28;
        private const int FireballSize = 24;
        private const double PelletFrameTime = 70;
        private const double BoomerangFrameTime = 50;
        private const double FireballFrameTime = 90;

        private static readonly Rectangle[] RightShotFrames = new Rectangle[]
        {
            new Rectangle(800, 640, 90, 64),
            new Rectangle(906, 640, 90, 64)
        };
        private static readonly Rectangle RightOrbFrame = new Rectangle(704, 638, 70, 70);
        private static readonly Rectangle LeftOrbFrame = new Rectangle(686, 631, 70, 70);
        private static readonly Rectangle BombFrame = new Rectangle(1016, 1014, 50, 50);
        private static readonly Rectangle[] ExplosionFrames = new Rectangle[]
        {
            new Rectangle(1040, 612, 110, 110),
            new Rectangle(1152, 614, 110, 110),
            new Rectangle(1268, 614, 110, 110),
            new Rectangle(1387, 612, 110, 110)
        };

        private static ProjectileSpriteFactory instance = new ProjectileSpriteFactory();

        private Texture2D rightSpriteSheet;
        private Texture2D leftSpriteSheet;
        private Texture2D bombSpriteSheet;
        private Texture2D objectSpriteSheet;

        private ProjectileSpriteFactory()
        {
        }

        public static ProjectileSpriteFactory Instance
        {
            get
            {
                return instance;
            }
        }

        public void LoadAllTextures(ContentManager content)
        {
            rightSpriteSheet = content.Load<Texture2D>("Sprites/transformer-platformer-spritesheet");
            leftSpriteSheet = content.Load<Texture2D>("Sprites/transformer-platformer-spritesheet-left");
            bombSpriteSheet = content.Load<Texture2D>("Sprites/transformer-directional-combat-spritesheet");
            objectSpriteSheet = content.Load<Texture2D>(SpriteAtlas.AssetName);
        }

        public ISprite CreateShotSprite(bool facingLeft)
        {
            SpriteEffects effects = facingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            return new AnimatedSprite(
                rightSpriteSheet, RightShotFrames,
                ShotWidth, ShotHeight, Color.White, ShotFrameTime, effects);
        }

        public ISprite CreateOrbSprite(bool facingLeft)
        {
            Texture2D sheet = facingLeft ? leftSpriteSheet : rightSpriteSheet;
            Rectangle frame = facingLeft ? LeftOrbFrame : RightOrbFrame;
            return new TextureRegionSprite(sheet, frame, OrbSize, OrbSize, Color.White);
        }

        public ISprite CreateBombSprite()
        {
            return new TextureRegionSprite(bombSpriteSheet, BombFrame, BombSize, BombSize, Color.White);
        }

        public ISprite CreateExplosionSprite(int blastLevel = 0)
        {
            int size = ExplosionSize + blastLevel * BlastGrowthPerLevel;
            return new AnimatedSprite(rightSpriteSheet, ExplosionFrames, size, size, Color.White, ExplosionFrameTime);
        }

        public ISprite CreateSpreadPelletSprite()
        {
            return new AnimatedSprite(objectSpriteSheet, SpriteAtlas.SpreadPellet, PelletSize, PelletSize, Color.White, PelletFrameTime);
        }

        public ISprite CreateBoomerangSprite()
        {
            return new AnimatedSprite(objectSpriteSheet, SpriteAtlas.Boomerang, BoomerangSize, BoomerangSize, Color.White, BoomerangFrameTime);
        }

        public ISprite CreateBossFireballSprite()
        {
            return new AnimatedSprite(objectSpriteSheet, SpriteAtlas.BossFireball, FireballSize, FireballSize, Color.White, FireballFrameTime);
        }
    }
}
