using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    public interface IBlock : IGameObject
    {
        bool IsSolid { get; }

        Rectangle Bounds { get; }
    }
}
