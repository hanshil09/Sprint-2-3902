using System;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Collisions
{
    internal static class PlayerCollision
    {
        private const int RollDamage = 1;
        private const float RollKnockbackDistance = 24f;

        public static void HandleContact(IPlayer player, IGameObject other, Action<Door> enterDoor,
            Action<IPlayer> damagePlayer, RollHitTracker rollHits)
        {
            switch (other)
            {
                case Door door:
                    enterDoor?.Invoke(door);
                    break;
                case IPlayerTrigger trigger:
                    trigger.OnPlayerContact(player);
                    break;
                case IEnemy enemy when !enemy.IsDestroyed:
                    HandleEnemyContact(player, enemy, damagePlayer, rollHits);
                    break;
            }
        }

        private static void HandleEnemyContact(IPlayer player, IEnemy enemy, Action<IPlayer> damagePlayer,
            RollHitTracker rollHits)
        {
            if (player.IsInBallForm)
            {
                HandleRollContact(player, enemy, rollHits);
            }
            else if (player.IsInvincible)
            {
                enemy.TakeDamage(1);
            }
            else
            {
                damagePlayer(player);
            }
        }

        // Rolling damages and shoves the enemy once per contact (non-chainable).
        private static void HandleRollContact(IPlayer player, IEnemy enemy, RollHitTracker rollHits)
        {
            if (rollHits == null || !rollHits.TryRegisterHit(enemy))
            {
                return;
            }
            float playerCenterX = player.Position.X + player.Width / 2f;
            int direction = enemy.Bounds.Center.X >= playerCenterX ? 1 : -1;
            enemy.TakeDamage(RollDamage);
            enemy.ApplyKnockback(direction, RollKnockbackDistance);
        }
    }
}
