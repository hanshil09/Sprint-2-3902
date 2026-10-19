using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    public interface IEnemy : IGameObject
    {
        Rectangle Bounds { get; }

        bool IsDestroyed { get; }

        void TakeDamage(int amount);
    }
}
