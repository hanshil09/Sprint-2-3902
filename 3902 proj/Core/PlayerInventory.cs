using System;
using System.Collections.Generic;

namespace TransformersGame.Core
{
    // Counts of collected things the player carries between rooms (extracted from Player to keep it focused).
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
                [ItemKind.Energon] = () => EnergonCount++,
                [ItemKind.BombPack] = () => BombLevel = Math.Min(MaximumBombLevel, BombLevel + 1)
            };
        }

        public int MedkitCount { get; private set; }

        public int ShieldCount { get; private set; }

        public int BombLevel { get; private set; }

        public int EnergonCount { get; private set; }

        // Returns false when the kind is not something the inventory stores or counts.
        public bool TryCollect(ItemKind kind)
        {
            if (!collectActions.TryGetValue(kind, out Action collect))
            {
                return false;
            }
            collect();
            return true;
        }

        public bool TryUseMedkit() => TryUse(MedkitCount, () => MedkitCount--);

        public bool TryUseShield() => TryUse(ShieldCount, () => ShieldCount--);


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
