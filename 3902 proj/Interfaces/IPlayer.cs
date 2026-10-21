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

        // Bombs are unlimited; this is the blast-size upgrade level from Bomb Packs.
        int BombLevel { get; }

        int EnergonCount { get; }

        bool IsShieldActive { get; }

        // True while a power-up makes the player immune to damage; contact then damages enemies instead.
        bool IsInvincible { get; }

        // Multiplier applied to the current form's movement speed (Turbo Boost raises it temporarily).
        float SpeedScale { get; set; }

        void Move(Direction direction);

        void Jump();

        // Launches the player upward even when airborne (spring pads).
        void Bounce(float speed);

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
