using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    internal interface IBlock : IGameObject
    {
        bool IsSolid { get; }

        Rectangle Bounds { get; }
    }
}
