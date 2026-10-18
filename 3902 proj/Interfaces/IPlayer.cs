using System.Collections.Generic;
using Microsoft.Xna.Framework;
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

        void Shoot(bool angled);

        void UseItem(int itemNumber);

        void Transform();

        void TakeDamage();

        void EnterRoom(IReadOnlyList<IBlock> blocks, IProjectileManager projectiles, Vector2 position);
    }
}
