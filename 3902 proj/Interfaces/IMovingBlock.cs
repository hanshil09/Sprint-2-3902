using Microsoft.Xna.Framework;

namespace ArmadilloGame.Interfaces
{
    interface IMovingBlock : IBlock
    {
        Vector2 Delta { get; }
    }
}
