using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;

namespace TransformersGame.Factories
{
    internal class PlayerSpriteFactory
    {
        private const int PlayerWidth = 68;
        private const int PlayerHeight = 80;

        private const int BallWidth = 58;
        private const int BallHeight = 58;

        private const double IdleFrameTime = 250;
        private const double RunFrameTime = 83;
        private const double RollFrameTime = 90;

        // Idle: 3 frames
        private static readonly Rectangle[] IdleFrames =
        {
            new Rectangle(4, 58, 70, 100),
            new Rectangle(76, 58, 70, 100),
            new Rectangle(148, 58, 70, 100)
        };

        // Run: 3 frames
        private static readonly Rectangle[] RunFrames =
        {
            new Rectangle(220, 58, 86, 100),
            new Rectangle(308, 58, 88, 100),
            new Rectangle(398, 58, 86, 100)
        };

        // Shoot: 1 frame
        private static readonly Rectangle ShootFrame =
            new Rectangle(486, 58, 74, 100);

        // Jump: 1 frame
        private static readonly Rectangle JumpFrame =
            new Rectangle(562, 58, 82, 100);

        // Ball: 2 frames
        private static readonly Rectangle[] BallFrames =
        {
            new Rectangle(646, 78, 76, 80),
            new Rectangle(724, 78, 76, 80)
        };

        private static readonly PlayerSpriteFactory instance =
            new PlayerSpriteFactory();

        private Texture2D dilloSpriteSheet;

        private PlayerSpriteFactory()
        {
        }

        public static PlayerSpriteFactory Instance => instance;

        public void LoadAllTextures(ContentManager content)
        {
            dilloSpriteSheet = content.Load<Texture2D>(
                "Sprites/dilloSpriteTest2");
        }

        private static SpriteEffects GetFlipEffect(bool facingLeft)
        {
            return facingLeft
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;
        }

        public ISprite CreateStandingRobotSprite(Direction facing)
        {
            return new AnimatedSprite(
                dilloSpriteSheet,
                IdleFrames,
                PlayerWidth,
                PlayerHeight,
                Color.White,
                IdleFrameTime,
                GetFlipEffect(facing == Direction.Left));
        }

        public ISprite CreateRunningRobotSprite(bool facingLeft)
        {
            return new AnimatedSprite(
                dilloSpriteSheet,
                RunFrames,
                PlayerWidth,
                PlayerHeight,
                Color.White,
                RunFrameTime,
                GetFlipEffect(facingLeft));
        }

        public ISprite CreateShootingRobotSprite(bool facingLeft)
        {
            return new TextureRegionSprite(
                dilloSpriteSheet,
                ShootFrame,
                PlayerWidth,
                PlayerHeight,
                Color.White,
                GetFlipEffect(facingLeft));
        }

        public ISprite CreateJumpingRobotSprite(bool facingLeft)
        {
            return new TextureRegionSprite(
                dilloSpriteSheet,
                JumpFrame,
                PlayerWidth,
                PlayerHeight,
                Color.White,
                GetFlipEffect(facingLeft));
        }

        public ISprite CreateBallSprite(bool facingLeft)
        {
            return new TextureRegionSprite(
                dilloSpriteSheet,
                BallFrames[0],
                BallWidth,
                BallHeight,
                Color.White,
                GetFlipEffect(facingLeft));
        }

        public ISprite CreateRollingBallSprite(bool facingLeft)
        {
            return new AnimatedSprite(
                dilloSpriteSheet,
                BallFrames,
                BallWidth,
                BallHeight,
                Color.White,
                RollFrameTime,
                GetFlipEffect(facingLeft));
        }
    }
}
