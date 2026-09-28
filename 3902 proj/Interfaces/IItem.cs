using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    public interface IItem : IGameObject
    {
        Rectangle Bounds { get; }

        void Collect(IPlayer player);
    }
}
 