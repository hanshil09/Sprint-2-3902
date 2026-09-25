using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace TransformersGame.Interfaces;
public interface IGameObject
{
    Vector2 Position { get; set; }
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch);
}
