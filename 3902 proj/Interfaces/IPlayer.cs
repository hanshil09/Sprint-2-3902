using TransformersGame.Core;

namespace TransformersGame.Interfaces
{
    public interface IPlayer : IGameObject
    {
        int Width { get; }

        int Height { get; }

        Direction Facing { get; }

        void Move(Direction direction);

        void Jump();

        void Shoot();

        void UseItem(int itemNumber);

        void Transform();

        void TakeDamage();
    }
}
