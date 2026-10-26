using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    interface IItem : IGameObject
    {
        Rectangle Bounds { get; }

        void Collect(IPlayer player);
    }
}

