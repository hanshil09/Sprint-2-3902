using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    interface IBlock : IGameObject
    {
        bool IsSolid { get; }

        Rectangle Bounds { get; }
    }
}
