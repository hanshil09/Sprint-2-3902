using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TransformersGame.Core;

namespace TransformersGame.Interfaces
{
    internal interface IPlayer : IGameObject
    {
        int Width { get; }

        int Height { get; }
        bool IsInBallForm { get; }

        Direction Facing { get; }

        int Health { get; }

        int MaxHealth { get; }

        int MedkitCount { get; }

        int ShieldCount { get; }

        // Bombs are unlimited; this is the blast-size upgrade level from Bomb Packs.
        int BombLevel { get; }

        int EnergonCount { get; }

        // Keys collected; used to open locked doors. Persists across rooms.
        int KeyCount { get; }

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

        void UseItem(int itemNumber, bool angled = false);

        void Transform();

        bool TakeDamage();

        bool TryAddItem(ItemKind kind);

        bool UseMedkit();

        bool UseShield();

        // Spends one key to open a locked door; returns false when the player has none.
        bool TryUseKey();

        void EnterRoom(IReadOnlyList<IBlock> blocks, IProjectileManager projectiles, Vector2 position, float gravity = Physics.DefaultGravity);
    }
}
