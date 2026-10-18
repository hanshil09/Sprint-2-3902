using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    public interface IProjectile : IGameObject
    {
        bool IsActive { get; }

        Vector2 PreviousPosition { get; }

        Rectangle Bounds { get; }

        void Deactivate();
    }
}
