using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    public interface IEnemyState
    {
        int Facing { get; }

        void ChangeDirection();

        void BeDestroyed();

        void Update(GameTime gameTime);
    }
}
