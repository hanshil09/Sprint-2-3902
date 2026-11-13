using System;
using System.Collections.Generic;
using TransformersGame.Entities;

namespace TransformersGame.Core
{
    // Tracks carried items and coordinates their pickup and use.
    internal sealed class PlayerInventory
    {
        // Bombs are unlimited; each Bomb Pack makes the explosion bigger, up to this level.
        public const int MaximumBombLevel = 3;

        private readonly Dictionary<ItemKind, Action> collectActions;

        public PlayerInventory()
        {
            collectActions = new Dictionary<ItemKind, Action>
            {
                [ItemKind.Medkit] = () => MedkitCount++,
                [ItemKind.Shield] = () => ShieldCount++,
                [ItemKind.ShieldBattery] = () => ShieldCount++,
                [ItemKind.Energon] = () => EnergonCount++,
                [ItemKind.Key] = () => KeyCount++,
                [ItemKind.BombPack] = () => BombLevel = Math.Min(MaximumBombLevel, BombLevel + 1)
            };
        }

        public PlayerInventoryView View => new PlayerInventoryView(MedkitCount, ShieldCount, BombLevel, EnergonCount);

        public int MedkitCount { get; private set; }

        public int ShieldCount { get; private set; }

        public int BombLevel { get; private set; }

        public int EnergonCount { get; private set; }

        public int KeyCount { get; private set; }

        // Returns false for unsupported items or an upgrade already at its cap.
        public bool TryCollect(ItemKind kind)
        {
            if (kind == ItemKind.BombPack && BombLevel == MaximumBombLevel)
            {
                return false;
            }
            if (!collectActions.TryGetValue(kind, out Action collect))
            {
                return false;
            }
            collect();
            return true;
        }

        public bool TryCollect(ItemKind kind, PlayerHealth health)
        {
            switch (kind)
            {
                case ItemKind.HealthCell:
                    return health.TryUseHealthCell();
                case ItemKind.MaxHealthUp:
                    return health.TryUpgrade();
                default:
                    return TryCollect(kind);
            }
        }

        public bool TryUseMedkit(PlayerHealth health)
        {
            if (health.IsFull || !TryUse(MedkitCount, () => MedkitCount--))
            {
                return false;
            }

            health.Restore();
            return true;
        }

        public bool TryUseShield(PlayerShield shield)
        {
            if (shield.IsActive || !TryUse(ShieldCount, () => ShieldCount--))
            {
                return false;
            }

            shield.Start();
            return true;
        }

        // Consumes one key to open a locked door; returns false when the player has none.
        public bool TryUseKey() => TryUse(KeyCount, () => KeyCount--);


        private static bool TryUse(int count, Action consume)
        {
            if (count <= 0)
            {
                return false;
            }
            consume();
            return true;
        }
    }
}
