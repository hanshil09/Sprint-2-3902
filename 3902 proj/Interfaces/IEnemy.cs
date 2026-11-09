using Microsoft.Xna.Framework;
using TransformersGame.Core;

namespace TransformersGame.Interfaces
{
    interface IEnemy : IGameObject
    {
        Rectangle Bounds { get; }
        EnemyKind Kind { get; }

        bool IsDestroyed { get; }

        void TakeDamage(int amount);
    }
}
