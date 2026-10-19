using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TransformersGame.Core;

namespace TransformersGame.Interfaces
{
    internal interface IPlayer : IGameObject
    {
        int Width { get; }

        int Height { get; }

        Direction Facing { get; }

        int Health { get; }

        int MaxHealth { get; }

        int MedkitCount { get; }

        int ShieldCount { get; }

        bool IsShieldActive { get; }

        void Move(Direction direction);

        void Jump();

        void Shoot(bool angled);

        void UseItem(int itemNumber);

        void Transform();

        bool TakeDamage();

        void AddItem(ItemKind kind);

        bool UseMedkit();

        bool UseShield();

        void EnterRoom(IReadOnlyList<IBlock> blocks, IProjectileManager projectiles, Vector2 position, float gravity = Physics.DefaultGravity);
    }
}
