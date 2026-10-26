using System;
using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace Collisions
{
    public class CollisionHelper
    {
        
        public CollisionHelper()
        {
            
        }

        public static bool TryGetBounds(IGameObject gameObject, out Rectangle bounds)
        {
            switch (gameObject)
            {
                case Door door:
                    bounds = door.Bounds;
                    return true;
                case IProjectile projectile:
                    bounds = projectile.Bounds;
                    return true;
                case IBlock block:
                    bounds = block.Bounds;
                    return block.IsSolid || block is Block hazard && hazard.IsHazard;
                case IItem item:
                    bounds = item.Bounds;
                    return true;
                case IPlayerTrigger trigger:
                    bounds = trigger.Bounds;
                    return true;
                case IPlayer player:
                    bounds = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);
                    return true;
                case IEnemy enemy:
                    bounds = enemy.Bounds;
                    return true;
                default:
                    bounds = Rectangle.Empty;
                    return false;
            }
        }

        public static bool SweptIntersects(IProjectile projectile, Rectangle target)
        {
            return !float.IsPositiveInfinity(ImpactTime(projectile, target));
        }

        public static float ImpactTime(IProjectile projectile, Rectangle target)
        {
            Rectangle current = projectile.Bounds;
            if (current.Intersects(target))
            {
                return 0;
            }

            float halfWidth = current.Width / 2f;
            float halfHeight = current.Height / 2f;
            float startX = projectile.PreviousPosition.X + halfWidth;
            float startY = projectile.PreviousPosition.Y + halfHeight;
            float deltaX = projectile.Position.X - projectile.PreviousPosition.X;
            float deltaY = projectile.Position.Y - projectile.PreviousPosition.Y;
            float minX = target.Left - halfWidth;
            float maxX = target.Right + halfWidth;
            float minY = target.Top - halfHeight;
            float maxY = target.Bottom + halfHeight;
            float enter = 0;
            float exit = 1;

            return ClipAxis(startX, deltaX, minX, maxX, ref enter, ref exit)
                && ClipAxis(startY, deltaY, minY, maxY, ref enter, ref exit)
                ? enter : float.PositiveInfinity;
        }

        private static bool ClipAxis(float origin, float delta, float minimum, float maximum, ref float enter, ref float exit)
        {
            if (delta == 0)
            {
                return origin >= minimum && origin <= maximum;
            }

            float first = (minimum - origin) / delta;
            float second = (maximum - origin) / delta;
            if (first > second)
            {
                (first, second) = (second, first);
            }

            enter = Math.Max(enter, first);
            exit = Math.Min(exit, second);
            return enter <= exit;
        }
    }
}