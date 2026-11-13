using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TransformersGame.Interfaces
{
    internal interface IGameState
    {
        void Update(GameTime gameTime);
        void Draw(SpriteBatch batch);
    }
}
