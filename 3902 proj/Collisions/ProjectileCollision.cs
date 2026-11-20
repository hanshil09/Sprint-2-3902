using System;
using Collisions;
using Microsoft.Xna.Framework;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Collisions
{
    internal static class ProjectileCollision
    {
        public static void HandleImpact(IProjectile projectile, Rectangle targetBounds, IGameObject target,
            Action<IPlayer> damagePlayer)
        {
            ArgumentNullException.ThrowIfNull(damagePlayer);
            if (!projectile.IsActive || !CollisionHelper.SweptIntersects(projectile, targetBounds))
            {
                return;
            }
            if (target is IBlock block)
            {
                BlockContact.HandleProjectileImpact(projectile, targetBounds, block);
                return;
            }

            switch (projectile)
            {
                case Bomb bomb when target is IEnemy enemy && bomb.Bounds.Intersects(targetBounds):
                    bomb.DamageEnemy(enemy);
                    break;
                case IPlayerWeapon weapon when target is IEnemy enemy:
                    weapon.HitEnemy(enemy);
                    break;
                case EnemyProjectile shot when target is IPlayer player:
                    damagePlayer(player);
                    shot.Deactivate();
                    break;
            }
        }
    }
}
