using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    // Non-solid level objects that react when the player touches them (spring pads, the goal beacon).
    interface IPlayerTrigger : IGameObject
    {
        Rectangle Bounds { get; }

        void OnPlayerContact(IPlayer player);
    }
}
