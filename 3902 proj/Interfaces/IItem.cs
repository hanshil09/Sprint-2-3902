using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    internal interface IItem : IGameObject
    {
        Rectangle Bounds { get; }

        void Collect(IPlayer player);
    }
}

