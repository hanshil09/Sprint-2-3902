using System;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.Collisions
{
    internal static class PlayerCollision
    {
        public static void HandleContact(IPlayer player, IGameObject other, Action<int> enterDoor,
            Action<IPlayer> damagePlayer)
        {
            switch (other)
            {
                case Door door:
                    if (enterDoor != null)
                    {
                        enterDoor(door.Direction);
                    }
                    break;
                case IPlayerTrigger trigger:
                    trigger.OnPlayerContact(player);
                    break;
                case IEnemy enemy when !enemy.IsDestroyed:
                    HandleEnemyContact(player, enemy, damagePlayer);
                    break;
            }
        }

        private static void HandleEnemyContact(IPlayer player, IEnemy enemy, Action<IPlayer> damagePlayer)
        {
            if (player.IsInvincible)
            {
                enemy.TakeDamage(1);
            }
            else
            {
                damagePlayer(player);
            }
        }
    }
}
