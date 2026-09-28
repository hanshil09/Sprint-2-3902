using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    public interface IEnemyState
    {
        void ChangeDirection();

        void Hop();

        void BeDestroyed();

        void Update(GameTime gameTime);
    }
}
