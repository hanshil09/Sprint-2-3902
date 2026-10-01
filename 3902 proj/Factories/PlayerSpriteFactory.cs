using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;

namespace TransformersGame.Factories
{
    public class PlayerSpriteFactory
    {
        private const int CellWidth = 170;
        private const int CellHeight = 140;
        private const int FramesPerRow = 8;
        private const int RobotWidth = 68;
        private const int RobotHeight = 56;
        private const int ShootingWidth = 64;
        private const int BallWidth = 56;
        private const int BallHeight = 40;
        private const double RunFrameTime = 85;
        private const double ShootFrameTime = 140;
        private const double RollFrameTime = 90;
        private const int RightRowTop = 7;
        private const int LeftRowTop = 157;
        private const int UpRowTop = 309;
        private const int DownRowTop = 452;

        private static readonly Rectangle JumpFrame = new Rectangle(361, 592, 170, 140);
        private static readonly Rectangle[] RightShootFrames = new Rectangle[]
        {
            new Rectangle(30, 606, 160, 140),
            new Rectangle(200, 606, 160, 140)
        };
        private static readonly Rectangle[] LeftShootFrames = new Rectangle[]
        {
            new Rectangle(1005, 606, 160, 140),
            new Rectangle(1176, 606, 160, 140)
        };
        private static readonly Rectangle RightBallFrame = new Rectangle(460, 426, 140, 100);
        private static readonly Rectangle LeftBallFrame = new Rectangle(465, 427, 140, 100);
        private static readonly Rectangle[] RightRollFrames = new Rectangle[]
        {
            new Rectangle(659, 426, 140, 100),
            new Rectangle(787, 426, 140, 100),
            new Rectangle(917, 426, 140, 100),
            new Rectangle(1042, 426, 140, 100),
            new Rectangle(1165, 426, 140, 100),
            new Rectangle(1292, 426, 140, 100)
        };
        private static readonly Rectangle[] LeftRollFrames = new Rectangle[]
        {
            new Rectangle(608, 427, 140, 100),
            new Rectangle(749, 427, 140, 100),
            new Rectangle(890, 427, 140, 100),
            new Rectangle(1036, 427, 140, 100),
            new Rectangle(1177, 427, 140, 100),
            new Rectangle(1319, 427, 140, 100)
        };

        private static PlayerSpriteFactory instance = new PlayerSpriteFactory();

        private Texture2D robotSpriteSheet;
        private Texture2D rightBallSpriteSheet;
        private Texture2D leftBallSpriteSheet;

        private PlayerSpriteFactory()
        {
        }

        public static PlayerSpriteFactory Instance
        {
            get
            {
                return instance;
            }
        }

        public void LoadAllTextures(ContentManager content)
        {
            robotSpriteSheet = content.Load<Texture2D>("Sprites/transformer-directional-combat-spritesheet");
            rightBallSpriteSheet = content.Load<Texture2D>("Sprites/transformer-platformer-spritesheet");
            leftBallSpriteSheet = content.Load<Texture2D>("Sprites/transformer-platformer-spritesheet-left");
        }

        public ISprite CreateStandingRobotSprite(Direction facing)
        {
            Rectangle frame = new Rectangle(0, RowTop(facing), CellWidth, CellHeight);
            return new TextureRegionSprite(robotSpriteSheet, frame, RobotWidth, RobotHeight, Color.White);
        }

        public ISprite CreateRunningRobotSprite(bool facingLeft)
        {
            Rectangle[] frames = RowFrames(RowTop(facingLeft ? Direction.Left : Direction.Right));
            return new AnimatedSprite(robotSpriteSheet, frames, RobotWidth, RobotHeight, Color.White, RunFrameTime);
        }

        public ISprite CreateJumpingRobotSprite(bool facingLeft)
        {
            SpriteEffects effects = facingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            return new TextureRegionSprite(robotSpriteSheet, JumpFrame, RobotWidth, RobotHeight, Color.White, effects);
        }

        public ISprite CreateShootingRobotSprite(bool facingLeft)
        {
            Rectangle[] frames = facingLeft ? LeftShootFrames : RightShootFrames;
            return new AnimatedSprite(robotSpriteSheet, frames, ShootingWidth, RobotHeight, Color.White, ShootFrameTime);
        }

        public ISprite CreateBallSprite(bool facingLeft)
        {
            Texture2D sheet = facingLeft ? leftBallSpriteSheet : rightBallSpriteSheet;
            Rectangle frame = facingLeft ? LeftBallFrame : RightBallFrame;
            return new TextureRegionSprite(sheet, frame, BallWidth, BallHeight, Color.White);
        }

        public ISprite CreateRollingBallSprite(bool facingLeft)
        {
            Texture2D sheet = facingLeft ? leftBallSpriteSheet : rightBallSpriteSheet;
            Rectangle[] frames = facingLeft ? LeftRollFrames : RightRollFrames;
            return new AnimatedSprite(sheet, frames, BallWidth, BallHeight, Color.White, RollFrameTime);
        }

        private static int RowTop(Direction facing)
        {
            switch (facing)
            {
                case Direction.Left:
                    return LeftRowTop;
                case Direction.Up:
                    return UpRowTop;
                case Direction.Down:
                    return DownRowTop;
                default:
                    return RightRowTop;
            }
        }

        private static Rectangle[] RowFrames(int top)
        {
            Rectangle[] frames = new Rectangle[FramesPerRow];
            for (int index = 0; index < FramesPerRow; index++)
            {
                frames[index] = new Rectangle(index * CellWidth, top, CellWidth, CellHeight);
            }
            return frames;
        }
    }
}
