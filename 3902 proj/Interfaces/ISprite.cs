using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace TransformersGame.Interfaces;
public interface ISprite
{
    int Width { get; }
    int Height { get; }
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch, Vector2 position);
}
