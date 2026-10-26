using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    interface IItem : IGameObject
    {
        Rectangle Bounds { get; }

        // A rejected pickup stays in the room so the player can collect it later.
        bool TryCollect(IPlayer player);
    }
}

