using Microsoft.Xna.Framework;

namespace ArmadilloGame.Interfaces
{
    interface IBlock : IGameObject
    {
        bool IsSolid { get; }

        Rectangle Bounds { get; }
    }
}
