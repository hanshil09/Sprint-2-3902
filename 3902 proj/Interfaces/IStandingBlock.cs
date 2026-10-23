using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    internal interface IStandingBlock : IBlock
    {
        void OnStand();

        void SetPlayerBounds(Rectangle bounds);
    }
}
