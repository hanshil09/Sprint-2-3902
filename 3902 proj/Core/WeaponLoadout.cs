using System.Collections.Generic;

namespace ArmadilloGame.Core
{
    // Single source of truth for which weapons are unlocked and their ammo.
    // Boomerang and Pistol have unlimited ammo; Shotgun and Molotov are limited.
    internal sealed class WeaponLoadout
    {
        public const int ShotgunUnlockAmmo = 5;
        public const int MolotovUnlockAmmo = 3;

        private static readonly IReadOnlyDictionary<WeaponKind, bool> Unlimited = new Dictionary<WeaponKind, bool>
        {
            [WeaponKind.Boomerang] = true,
            [WeaponKind.Pistol] = true,
            [WeaponKind.Shotgun] = false,
            [WeaponKind.Molotov] = false
        };

        private static readonly IReadOnlyDictionary<WeaponKind, int> UnlockAmmo = new Dictionary<WeaponKind, int>
        {
            [WeaponKind.Shotgun] = ShotgunUnlockAmmo,
            [WeaponKind.Molotov] = MolotovUnlockAmmo
        };

        private readonly HashSet<WeaponKind> unlocked = new HashSet<WeaponKind> { WeaponKind.Boomerang };
        private readonly Dictionary<WeaponKind, int> ammo = new Dictionary<WeaponKind, int>();

        public bool IsUnlocked(WeaponKind kind) => unlocked.Contains(kind);

        public static bool HasUnlimitedAmmo(WeaponKind kind) => Unlimited[kind];

        public int Ammo(WeaponKind kind) => ammo.TryGetValue(kind, out int value) ? value : 0;

        // Grants a weapon and its starting ammo. Re-unlocking tops up limited ammo.
        public void Unlock(WeaponKind kind)
        {
            unlocked.Add(kind);
            if (UnlockAmmo.TryGetValue(kind, out int starting))
            {
                AddAmmo(kind, starting);
            }
        }

        public void AddAmmo(WeaponKind kind, int amount)
        {
            if (HasUnlimitedAmmo(kind) || amount <= 0)
            {
                return;
            }
            ammo[kind] = Ammo(kind) + amount;
        }

        public bool CanFire(WeaponKind kind) =>
            IsUnlocked(kind) && (HasUnlimitedAmmo(kind) || Ammo(kind) > 0);

        // Spends one shot; returns false when the weapon is locked or empty.
        public bool TryConsume(WeaponKind kind)
        {
            if (!CanFire(kind))
            {
                return false;
            }
            if (!HasUnlimitedAmmo(kind))
            {
                ammo[kind] = Ammo(kind) - 1;
            }
            return true;
        }
    }
}
