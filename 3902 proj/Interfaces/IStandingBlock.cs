using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    interface IStandingBlock : IBlock
    {
        void OnStand();

        void SetPlayerBounds(Rectangle bounds);
    }
}
