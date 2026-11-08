
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
        private const int ShotSize = 30;
        private const int OrbSize = 28;
        private const int BombSize = 40;
        private const int ExplosionSize = 44;
        private const int BlastGrowthPerLevel = 18;
        private const int PelletSize = 16;
        private const int BoomerangSize = 28;
        private const int FireballSize = 24;
        private const double PelletFrameTime = 70;
        private const double BoomerangFrameTime = 50;
        private const double FireballFrameTime = 90;

        // Horizontal fireball animations.
        private static readonly Rectangle[] RightShotFrames =
        {
            new Rectangle(0, 0, 48, 48),
            new Rectangle(48, 0, 48, 48),
            new Rectangle(96, 0, 48, 48),
            new Rectangle(144, 0, 48, 48)
        };

        private static readonly Rectangle[] LeftShotFrames =
        {
            new Rectangle(0, 192, 48, 48),
            new Rectangle(48, 192, 48, 48),
            new Rectangle(96, 192, 48, 48),
            new Rectangle(144, 192, 48, 48)
        };

        // Upward diagonal fireball animations.
        private static readonly Rectangle[] UpRightShotFrames =
        {
            new Rectangle(0, 336, 48, 48),
            new Rectangle(48, 336, 48, 48),
            new Rectangle(96, 336, 48, 48),
            new Rectangle(144, 336, 48, 48)
        };

        private static readonly Rectangle[] UpLeftShotFrames =
        {
            new Rectangle(0, 240, 48, 48),
            new Rectangle(48, 240, 48, 48),
            new Rectangle(96, 240, 48, 48),
            new Rectangle(144, 240, 48, 48)
        };

        // Bomb and explosion sprites.
        private static readonly Rectangle BombFrame =
            new Rectangle(0, 0, 48, 48);

        private static readonly Rectangle[] ExplosionFrames =
        {
            new Rectangle(48, 0, 48, 48),
            new Rectangle(96, 0, 48, 48),
            new Rectangle(144, 0, 48, 48),
            new Rectangle(192, 0, 48, 48),
            new Rectangle(240, 0, 48, 48),
            new Rectangle(288, 0, 48, 48),
            new Rectangle(336, 0, 48, 48)
        };

        private static readonly Rectangle RightOrbFrame = new Rectangle(704, 638, 70, 70);
        private static readonly Rectangle LeftOrbFrame = new Rectangle(686, 631, 70, 70);

        private static ProjectileSpriteFactory instance = new ProjectileSpriteFactory();

        private Texture2D rightSpriteSheet;
        private Texture2D leftSpriteSheet;
        private Texture2D objectSpriteSheet;
        private Texture2D directionalFireballSheet;
        private Texture2D explosionSpriteSheet;

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
            objectSpriteSheet = content.Load<Texture2D>(SpriteAtlas.AssetName);
            directionalFireballSheet = content.Load<Texture2D>("Sprites/fireballDirectional");
            explosionSpriteSheet = content.Load<Texture2D>("Sprites/explosionSprites");
        }

        public ISprite CreateShotSprite(bool facingLeft, bool aimingUp = false)
        {
            Rectangle[] frames;

            if (aimingUp)
            {
                frames = facingLeft ? UpLeftShotFrames : UpRightShotFrames;
            }
            else
            {
                frames = facingLeft ? LeftShotFrames : RightShotFrames;
            }

            return new AnimatedSprite(
                directionalFireballSheet,
                frames,
                ShotSize,
                ShotSize,
                Color.White,
                ShotFrameTime);
        }

        public ISprite CreateOrbSprite(bool facingLeft)
        {
            Texture2D sheet = facingLeft ? leftSpriteSheet : rightSpriteSheet;
            Rectangle frame = facingLeft ? LeftOrbFrame : RightOrbFrame;
            return new TextureRegionSprite(sheet, frame, OrbSize, OrbSize, Color.LightGreen);
        }

        public ISprite CreateBombSprite()
        {
            return new TextureRegionSprite(
                explosionSpriteSheet,
                BombFrame,
                BombSize,
                BombSize,
                Color.White);
        }

        public ISprite CreateExplosionSprite(int blastLevel = 0)
        {
            int size = ExplosionSize + blastLevel * BlastGrowthPerLevel;

            return new AnimatedSprite(
                explosionSpriteSheet,
                ExplosionFrames,
                size,
                size,
                Color.White,
                ExplosionFrameTime);
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
