using System;
using Collisions;
using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.Collisions
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
                BlockCollision.HandleProjectileImpact(projectile, targetBounds, block);
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
