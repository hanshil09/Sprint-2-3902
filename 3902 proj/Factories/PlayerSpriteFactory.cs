using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;
using ArmadilloGame.Sprites;

namespace ArmadilloGame.Factories
{
    internal class PlayerSpriteFactory
    {
        private const int PlayerWidth = 68;
        private const int PlayerHeight = 80;

        // The ball form is exactly one block wide/tall so it fits level geometry cleanly.
        private const int BallSize = 32;

        private const double IdleFrameTime = 250;
        private const double RunFrameTime = 83;
        private const double RollFrameTime = 90;

        // Source rects cropped tight to the character (black/purple/white padding excluded) so the
        // art renders without a letterbox or squish. Frames vary in size; the sprite classes scale
        // each to the logical PlayerWidth x PlayerHeight box, bottom-center anchored.
        // Idle: 3 frames
        private static readonly Rectangle[] IdleFrames =
        {
            new Rectangle(8, 64, 62, 92),
            new Rectangle(80, 66, 62, 90),
            new Rectangle(154, 68, 60, 88)
        };

        // Run: 3 frames
        private static readonly Rectangle[] RunFrames =
        {
            new Rectangle(222, 64, 80, 88),
            new Rectangle(310, 66, 80, 86),
            new Rectangle(410, 64, 70, 88)
        };

        // Shoot: 1 frame
        private static readonly Rectangle ShootFrame =
            new Rectangle(490, 62, 66, 94);

        // Jump: 1 frame
        private static readonly Rectangle JumpFrame =
            new Rectangle(566, 62, 74, 96);

        // Crop each ball frame to its visible pixels so collision matches the sprite.
        private static readonly Rectangle[] BallFrames =
        {
            new Rectangle(662, 116, 42, 42),
            new Rectangle(738, 116, 42, 42)
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
                GetFlipEffect(facing == Direction.Left),
                preserveAspect: true);
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
                GetFlipEffect(facingLeft),
                preserveAspect: true);
        }

        public ISprite CreateShootingRobotSprite(bool facingLeft)
        {
            return new TextureRegionSprite(
                dilloSpriteSheet,
                ShootFrame,
                PlayerWidth,
                PlayerHeight,
                Color.White,
                GetFlipEffect(facingLeft),
                preserveAspect: true);
        }

        public ISprite CreateJumpingRobotSprite(bool facingLeft)
        {
            return new TextureRegionSprite(
                dilloSpriteSheet,
                JumpFrame,
                PlayerWidth,
                PlayerHeight,
                Color.White,
                GetFlipEffect(facingLeft),
                preserveAspect: true);
        }

        public ISprite CreateBallSprite(bool facingLeft)
        {
            return new TextureRegionSprite(
                dilloSpriteSheet,
                BallFrames[0],
                BallSize,
                BallSize,
                Color.White,
                GetFlipEffect(facingLeft));
        }

        public ISprite CreateRollingBallSprite(bool facingLeft)
        {
            return new AnimatedSprite(
                dilloSpriteSheet,
                BallFrames,
                BallSize,
                BallSize,
                Color.White,
                RollFrameTime,
                GetFlipEffect(facingLeft));
        }
    }
}
