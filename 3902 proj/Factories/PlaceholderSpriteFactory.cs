using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;
namespace TransformersGame.Factories;
public sealed class PlaceholderSpriteFactory
{
    private readonly Texture2D pixel;
    public PlaceholderSpriteFactory(GraphicsDevice graphicsDevice)
    {
        pixel = new Texture2D(graphicsDevice, 1, 1);
        pixel.SetData([Color.White]);
    }
    public ISprite CreateRobotSprite(Direction facing, bool damaged = false) =>
        new SolidColorSprite(pixel, damaged ? Color.OrangeRed : FacingColor(facing), 40, 56);
    public ISprite CreateVehicleSprite(Direction facing, bool damaged = false) =>
        new SolidColorSprite(pixel, damaged ? Color.Red : FacingColor(facing), 64, 32);
    private static Color FacingColor(Direction facing) => facing switch
    {
        Direction.Up => Color.CornflowerBlue,
        Direction.Down => Color.RoyalBlue,
        Direction.Left => Color.SteelBlue,
        Direction.Right => Color.DodgerBlue,
        _ => Color.Blue
    };
}
