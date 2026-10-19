using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
     internal interface IMovingBlock : IBlock
    {
        Vector2 Delta { get; }
    }
}
