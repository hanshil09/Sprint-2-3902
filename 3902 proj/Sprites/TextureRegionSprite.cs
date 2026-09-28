using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Sprites;

public sealed class TextureRegionSprite(
    Texture2D texture,
    Rectangle sourceRectangle,
    int width,
    int height,
    Color tint) : ISprite
{
    public int Width { get; } = width;
    public int Height { get; } = height;

    public void Update(GameTime gameTime) { }

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        Rectangle destination = new((int)position.X, (int)position.Y, Width, Height);
        spriteBatch.Draw(texture, destination, sourceRectangle, tint);
    }
}
