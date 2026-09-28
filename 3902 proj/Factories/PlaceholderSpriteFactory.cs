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
        new Rectangle(280, 175, 145, 180),
        new Rectangle(420, 175, 140, 180),
        new Rectangle(550, 175, 145, 180),
        new Rectangle(690, 175, 145, 180),
        new Rectangle(825, 175, 150, 180),
        new Rectangle(970, 175, 155, 180)
    };
    private static readonly Rectangle[] LeftRobotRunFrames = new Rectangle[]
    {
        new Rectangle(270, 175, 125, 180),
        new Rectangle(400, 175, 150, 180),
        new Rectangle(545, 175, 155, 180),
        new Rectangle(695, 175, 145, 180),
        new Rectangle(830, 175, 160, 180),
        new Rectangle(995, 175, 155, 180)
    };
    private static readonly Rectangle[] RightShootFrames =
    {
        new Rectangle(0, 580, 160, 190),
        new Rectangle(150, 580, 165, 190),
        new Rectangle(300, 580, 165, 190),
        new Rectangle(450, 580, 180, 190)
    };
    private static readonly Rectangle[] LeftShootFrames =
    {
        new Rectangle(0, 580, 160, 190),
        new Rectangle(150, 580, 165, 190),
        new Rectangle(300, 580, 165, 190),
        new Rectangle(450, 580, 170, 190)
    };
    private static readonly Rectangle ProjectileFrame = new(705, 638, 70, 70);
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
    private readonly Texture2D directionalSpriteSheet;

    public PlaceholderSpriteFactory(Texture2D rightSpriteSheet, Texture2D leftSpriteSheet, Texture2D directionalSpriteSheet)
    {
        this.rightSpriteSheet = rightSpriteSheet;
        this.leftSpriteSheet = leftSpriteSheet;
        this.directionalSpriteSheet = directionalSpriteSheet;
    }

    public ISprite CreateRobotSprite(Direction facing, bool damaged = false)
    {
        Rectangle frame = DirectionFrames(facing)[0];
        return new TextureRegionSprite(directionalSpriteSheet, frame, 60, 80, damaged ? Color.OrangeRed : Color.White);
    }

    public ISprite CreateVehicleSprite(Direction facing, bool damaged = false)
    {
        Texture2D sheet = facing == Direction.Left ? leftSpriteSheet : rightSpriteSheet;
        return new TextureRegionSprite(sheet, BallFrame, 56, 56, damaged ? Color.Red : Color.White);
    }

    public ISprite CreateMovingRobotSprite(Direction facing, bool damaged = false)
    {
        return new AnimatedSprite(directionalSpriteSheet, DirectionFrames(facing), 60, 80,
            damaged ? Color.OrangeRed : Color.White, 85);
    }

    public ISprite CreateMovingVehicleSprite(Direction facing, bool damaged = false)
    {
        Texture2D sheet = facing == Direction.Left ? leftSpriteSheet : rightSpriteSheet;
        Rectangle[] frames = facing == Direction.Left ? LeftBallRollFrames : RightBallRollFrames;
        return new AnimatedSprite(sheet, frames, 56, 56, damaged ? Color.Red : Color.White);
    }

    public ISprite CreateShootingRobotSprite(Direction facing, bool damaged = false)
    {
        Texture2D sheet = facing == Direction.Left ? leftSpriteSheet : rightSpriteSheet;
        Rectangle[] frames = facing == Direction.Left ? LeftShootFrames : RightShootFrames;
        return new AnimatedSprite(sheet, frames, 68, 80, damaged ? Color.OrangeRed : Color.White, 70);
    }

    public ISprite CreateProjectileSprite() =>
        new TextureRegionSprite(rightSpriteSheet, ProjectileFrame, 24, 24, Color.White);

    public ISprite CreateEnemySprite(Vector2 movement, bool damaged = false)
    {
        int column = DirectionColumn(movement);
        Rectangle frame = new(column * 170, 760, 170, 180);
        return new TextureRegionSprite(directionalSpriteSheet, frame, 68, 76,
            damaged ? Color.Red : Color.White);
    }

    public ISprite CreateDefeatedEnemySprite() =>
        new TextureRegionSprite(directionalSpriteSheet, new Rectangle(680, 940, 340, 217), 90, 58, Color.White);

    private static Rectangle[] DirectionFrames(Direction facing)
    {
        int rowY = facing switch
        {
            Direction.Right => 0,
            Direction.Left => 155,
            Direction.Up => 305,
            _ => 455
        };
        Rectangle[] frames = new Rectangle[8];
        for (int index = 0; index < frames.Length; index++)
            frames[index] = new Rectangle(index * 170, rowY, 170, 155);
        return frames;
    }

    private static int DirectionColumn(Vector2 movement)
    {
        if (movement == Vector2.Zero) return 0;
        double angle = System.Math.Atan2(movement.Y, movement.X);
        int octant = (int)System.Math.Round(8 * angle / (2 * System.Math.PI));
        return (octant + 8) % 8;
    }
}
