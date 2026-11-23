using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ArmadilloGame.Behaviors;
using ArmadilloGame.Entities;
using ArmadilloGame.Factories;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Core
{
    // Creates items and enemies for a level from their kinds (moved out of Level to keep that class small).
    internal static class EntityFactory
    {
        private static readonly HashSet<ItemKind> PowerUps = new HashSet<ItemKind> { ItemKind.SparkCore, ItemKind.TurboBoost };

        public static IItem CreateItem(ItemKind kind, Vector2 position)
        {
            ISprite sprite = ItemSpriteFactory.Instance.CreateItemSprite(kind);
            if (WeaponPickup.IsWeaponPickup(kind))
            {
                return new WeaponPickup(position, sprite, kind);
            }
            if (PowerUps.Contains(kind))
            {
                return new PowerUpPickup(position, sprite, kind);
            }
            return kind switch
            {
                ItemKind.Medkit => new Medkit(position, sprite),
                ItemKind.Shield => new Shield(position, sprite),
                _ => new ResourcePickup(position, sprite, kind)
            };
        }

        public static Point SpawnSize(EnemyKind kind)
        {
            if (kind == EnemyKind.Warlord)
            {
                return new Point(EnemySpriteFactory.BossSize, EnemySpriteFactory.BossSize);
            }
            ISprite sprite = EnemySpriteFactory.Instance.CreateEnemySprite(kind, Color.White, true);
            return new Point(sprite.Width, sprite.Height);
        }

        // HP scales with a room's difficulty tier (0 = first room, 1 = hardest): roughly 2 HP
        // early up to 15 HP late. Per-kind multipliers keep relative toughness; aerial kinds sit
        // at 0.75x. The boss manages its own health elsewhere.
        private const int MinEnemyHealth = 2;
        private const int MaxEnemyHealth = 15;

        public static EnemyConfiguration CreateEnemyConfiguration(EnemyKind kind, float difficultyTier = 0f)
        {
            float tier = Math.Clamp(difficultyTier, 0f, 1f);
            int health = EnemyHealth(kind, tier);
            switch (kind)
            {
                case EnemyKind.Flyer:
                    return new EnemyConfiguration(kind, new EnemyStats(60f, 110f, 280f, 330f, 2.5), new SwoopBehavior(), health);
                case EnemyKind.Crawler:
                    return new EnemyConfiguration(kind, new EnemyStats(50f, 100f, 240f, 120f, 1.0), new StalkBehavior(), health);
                case EnemyKind.Hopper:
                    return new EnemyConfiguration(kind, new EnemyStats(60f, 80f, 320f, 170f, 0.9), new LeapBehavior(), health);
                case EnemyKind.Beetle:
                    return new EnemyConfiguration(kind, new EnemyStats(90f, 60f, 360f, 260f, 1.4), new ChargeBehavior(), health);
                case EnemyKind.Waver:
                    return new EnemyConfiguration(kind, new EnemyStats(70f, 0f, 420f, 95f, 0.0), new DriftBehavior(), health);
                case EnemyKind.Sentry:
                    return new EnemyConfiguration(kind, new EnemyStats(0, 0, 500, 0, 2.5), new GuardBehavior(false), health);
                case EnemyKind.Sentinel:
                    return new EnemyConfiguration(kind, new EnemyStats(0, 0, 500, 0, 2.5), new GuardBehavior(true), health);
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "No standard enemy is defined for this kind.");
            }
        }

        private static bool IsAerial(EnemyKind kind) => kind is EnemyKind.Flyer or EnemyKind.Waver;

        // Relative toughness per kind; chargers are sturdier, flyers lighter.
        private static float KindHealthFactor(EnemyKind kind) => kind switch
        {
            EnemyKind.Beetle => 1.3f,
            EnemyKind.Sentry or EnemyKind.Sentinel => 0.9f,
            _ => 1f
        };

        private static int EnemyHealth(EnemyKind kind, float tier)
        {
            float baseHealth = MinEnemyHealth + (MaxEnemyHealth - MinEnemyHealth) * tier;
            float scaled = baseHealth * KindHealthFactor(kind) * (IsAerial(kind) ? 0.75f : 1f);
            return Math.Clamp((int)MathF.Round(scaled), MinEnemyHealth, MaxEnemyHealth);
        }
    }
}
