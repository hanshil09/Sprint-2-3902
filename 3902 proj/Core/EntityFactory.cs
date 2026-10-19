using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TransformersGame.Behaviors;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    // Creates items and enemies for a level from their kinds (moved out of Level to keep that class small).
    internal static class EntityFactory
    {
        private static readonly HashSet<ItemKind> PowerUps = new HashSet<ItemKind> { ItemKind.SparkCore, ItemKind.TurboBoost };

        public static IItem CreateItem(ItemKind kind, Vector2 position)
        {
            ISprite sprite = ItemSpriteFactory.Instance.CreateItemSprite(kind);
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

        public static EnemyConfiguration CreateEnemyConfiguration(EnemyKind kind)
        {
            switch (kind)
            {
                case EnemyKind.Flyer:
                    return new EnemyConfiguration(kind, new EnemyStats(60f, 110f, 280f, 330f, 2.5), new SwoopBehavior());
                case EnemyKind.Crawler:
                    return new EnemyConfiguration(kind, new EnemyStats(50f, 100f, 240f, 120f, 1.0), new StalkBehavior());
                case EnemyKind.Hopper:
                    return new EnemyConfiguration(kind, new EnemyStats(60f, 80f, 320f, 170f, 0.9), new LeapBehavior());
                case EnemyKind.Beetle:
                    return new EnemyConfiguration(kind, new EnemyStats(90f, 60f, 360f, 260f, 1.4), new ChargeBehavior());
                case EnemyKind.Waver:
                    return new EnemyConfiguration(kind, new EnemyStats(70f, 0f, 420f, 95f, 0.0), new DriftBehavior());
                case EnemyKind.Sentry:
                    return new EnemyConfiguration(kind, new EnemyStats(0, 0, 500, 0, 2.5), new GuardBehavior(false));
                case EnemyKind.Sentinel:
                    return new EnemyConfiguration(kind, new EnemyStats(0, 0, 500, 0, 2.5), new GuardBehavior(true));
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, "No standard enemy is defined for this kind.");
            }
        }
    }
}
