using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;
namespace TransformersGame.Sprites;
public sealed class SolidColorSprite(Texture2D texture, Color color, int width, int height) : ISprite
{
    public int Width { get; } = width;
    public int Height { get; } = height;
    public void Update(GameTime gameTime) { }
    public void Draw(SpriteBatch spriteBatch, Vector2 position) =>
        spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, Width, Height), color);
}
