using System;
using Collisions;
using Microsoft.Xna.Framework;
using ArmadilloGame.Core;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Collisions
{
    internal static class BlockContact
    {
        public static bool HandlePlayerContact(IPlayer player, IGameObject other, Action<IPlayer> damagePlayer)
        {
            if (other is Block block && block.IsHazard)
            {
                damagePlayer(player);
            }
            return other is IBlock;
        }

        public static bool HandleProjectileImpact(IProjectile projectile, Rectangle targetBounds, IBlock block)
        {
            if (!projectile.IsActive || !CollisionHelper.SweptIntersects(projectile, targetBounds))
            {
                return false;
            }

            switch (projectile)
            {
                case Bomb bomb when bomb.Bounds.Intersects(targetBounds) && bomb.IsExploding
                    && block is Block crate
                    && (crate.Kind != BlockKind.MorphBallCrate || bomb.CanDestroyMorphBallCrates):
                    crate.Destroy();
                    break;
                case IPlayerWeapon weapon when block.IsSolid:
                    weapon.HitSolid(block);
                    break;
                case EnemyProjectile shot when block.IsSolid:
                    shot.Deactivate();
                    break;
            }
            return true;
        }
    }
}
