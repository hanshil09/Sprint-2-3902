using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    interface IMovingBlock : IBlock
    {
        Vector2 Delta { get; }
    }
}
