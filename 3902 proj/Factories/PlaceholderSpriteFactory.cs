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
}
