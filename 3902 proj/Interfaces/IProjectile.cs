using Microsoft.Xna.Framework;

namespace ArmadilloGame.Interfaces
{
    internal interface IProjectile : IGameObject
    {
        bool IsActive { get; }

        Vector2 PreviousPosition { get; }

        Rectangle Bounds { get; }

        void Deactivate();
    }
}
