using Microsoft.Xna.Framework;

namespace ArmadilloGame.Interfaces
{
    interface IEnemyState
    {
        int Facing { get; }

        void ChangeDirection();

        void BeDestroyed();

        void Update(GameTime gameTime);
    }
}
