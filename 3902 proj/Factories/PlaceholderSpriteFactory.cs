using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;
namespace TransformersGame.Factories;
public sealed class PlaceholderSpriteFactory
{
    private static readonly Rectangle RobotFrame = new(0, 160, 150, 200);
    private static readonly Rectangle BallFrame = new(460, 390, 140, 160);
    private static readonly Rectangle[] RightRobotRunFrames = new Rectangle[]
    {
        new Rectangle(134, 160, 142, 200),
        new Rectangle(286, 160, 142, 200),
        new Rectangle(416, 160, 142, 200),
        new Rectangle(557, 160, 142, 200),
        new Rectangle(693, 160, 142, 200),
        new Rectangle(833, 160, 142, 200),
        new Rectangle(974, 160, 142, 200)
    };
    private static readonly Rectangle[] LeftRobotRunFrames = new Rectangle[]
    {
        new Rectangle(137, 160, 142, 200),
        new Rectangle(272, 160, 142, 200),
        new Rectangle(409, 160, 142, 200),
        new Rectangle(554, 160, 142, 200),
        new Rectangle(697, 160, 142, 200),
        new Rectangle(840, 160, 142, 200),
        new Rectangle(1006, 160, 142, 200)
    };
    private static readonly Rectangle[] RightBallRollFrames = new Rectangle[]
    {
        new Rectangle(659, 390, 140, 160),
        new Rectangle(787, 390, 140, 160),
        new Rectangle(917, 390, 140, 160),
        new Rectangle(1042, 390, 140, 160),
        new Rectangle(1165, 390, 140, 160),
        new Rectangle(1292, 390, 140, 160)
    };
    private static readonly Rectangle[] LeftBallRollFrames = new Rectangle[]
    {
        new Rectangle(608, 390, 140, 160),
        new Rectangle(749, 390, 140, 160),
        new Rectangle(890, 390, 140, 160),
        new Rectangle(1036, 390, 140, 160),
        new Rectangle(1177, 390, 140, 160),
        new Rectangle(1319, 390, 140, 160)
    };
    private readonly Texture2D rightSpriteSheet;
    private readonly Texture2D leftSpriteSheet;

    public PlaceholderSpriteFactory(Texture2D rightSpriteSheet, Texture2D leftSpriteSheet)
    {
        this.rightSpriteSheet = rightSpriteSheet;
        this.leftSpriteSheet = leftSpriteSheet;
    }

    public ISprite CreateRobotSprite(Direction facing, bool damaged = false)
    {
        Texture2D sheet = facing == Direction.Left ? leftSpriteSheet : rightSpriteSheet;
        return new TextureRegionSprite(sheet, RobotFrame, 60, 80, damaged ? Color.OrangeRed : Color.White);
    }

    public ISprite CreateVehicleSprite(Direction facing, bool damaged = false)
    {
        Texture2D sheet = facing == Direction.Left ? leftSpriteSheet : rightSpriteSheet;
        return new TextureRegionSprite(sheet, BallFrame, 56, 56, damaged ? Color.Red : Color.White);
    }

    public ISprite CreateMovingRobotSprite(Direction facing, bool damaged = false)
    {
        Texture2D sheet = facing == Direction.Left ? leftSpriteSheet : rightSpriteSheet;
        Rectangle[] frames = facing == Direction.Left ? LeftRobotRunFrames : RightRobotRunFrames;
        return new AnimatedSprite(sheet, frames, 57, 80, damaged ? Color.OrangeRed : Color.White);
    }

    public ISprite CreateMovingVehicleSprite(Direction facing, bool damaged = false)
    {
        Texture2D sheet = facing == Direction.Left ? leftSpriteSheet : rightSpriteSheet;
        Rectangle[] frames = facing == Direction.Left ? LeftBallRollFrames : RightBallRollFrames;
        return new AnimatedSprite(sheet, frames, 56, 56, damaged ? Color.Red : Color.White);
    }
}
