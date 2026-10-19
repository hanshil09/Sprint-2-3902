using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    internal interface IEnemyState
    {
        int Facing { get; }

        void ChangeDirection();

        void BeDestroyed();

        void Update(GameTime gameTime);
    }
}
