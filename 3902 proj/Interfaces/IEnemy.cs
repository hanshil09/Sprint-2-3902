using Microsoft.Xna.Framework;
using ArmadilloGame.Core;

namespace ArmadilloGame.Interfaces
{
    interface IEnemy : IGameObject
    {
        Rectangle Bounds { get; }
        EnemyKind Kind { get; }

        bool IsDestroyed { get; }

        void TakeDamage(int amount);

        // Horizontal shove; direction is -1 (left) or +1 (right).
        void ApplyKnockback(int direction, float distance);
    }
}
