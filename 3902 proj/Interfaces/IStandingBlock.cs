using Microsoft.Xna.Framework;

namespace ArmadilloGame.Interfaces
{
    interface IStandingBlock : IBlock
    {
        void OnStand();

        void SetPlayerBounds(Rectangle bounds);
    }
}
