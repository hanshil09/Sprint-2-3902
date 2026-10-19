using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.Collisions
{
    public class CollisionManager
    {
        private readonly Action damagePlayer;
        private readonly Action<int> enterDoor;
        private readonly Rectangle worldBounds;
        private List<IGameObject> objects = new List<IGameObject>();

        public CollisionManager(int maxWidth, int maxHeight, Action damagePlayer = null, Action<int> enterDoor = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxWidth);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxHeight);
            this.damagePlayer = damagePlayer;
            this.enterDoor = enterDoor;
            worldBounds = new Rectangle(0, 0, maxWidth, maxHeight);
        }

        public void BuildTree(List<IGameObject> objs)
        {
            objects = objs ?? throw new ArgumentNullException(nameof(objs));
        }

        public void DetectCollision()
        {
            // Resolve each projectile's nearest swept impacts first, independent of object-list order.
            foreach (IGameObject gameObject in objects)
            {
                if (gameObject is not IProjectile projectile || !projectile.IsActive) continue;
                List<(IGameObject Target, Rectangle Bounds, float Time)> impacts = new();
                foreach (IGameObject target in objects)
                {
                    if (ReferenceEquals(projectile, target) || IsUnavailable(target)
                        || !TryGetBounds(target, out Rectangle bounds)) continue;
                    float impact = ImpactTime(projectile, bounds);
                    if (!float.IsPositiveInfinity(impact)) impacts.Add((target, bounds, impact));
                }
                impacts.Sort((a, b) => a.Time.CompareTo(b.Time));
                foreach (var impact in impacts)
                {
                    HandleProjectileCollision(projectile, impact.Bounds, impact.Target);
                    if (!projectile.IsActive) break;
                }
            }
            for (int i = 0; i < objects.Count; i++)
            {
                IGameObject objectA = objects[i];
                if (!TryGetBounds(objectA, out Rectangle boundsA) || IsUnavailable(objectA))
                {
                    continue;
                }

                for (int j = i + 1; j < objects.Count; j++)
                {
                    IGameObject objectB = objects[j];
                    if (ReferenceEquals(objectA, objectB)
                        || !TryGetBounds(objectB, out Rectangle boundsB)
                        || IsUnavailable(objectB))
                    {
                        continue;
                    }

                    HandleCollision(objectA, boundsA, objectB, boundsB);
                }
            }

            foreach (IGameObject gameObject in objects)
            {
                if (gameObject is IProjectile projectile && projectile.IsActive
                    && !projectile.Bounds.Intersects(worldBounds))
                {
                    projectile.Deactivate();
                }
            }
        }

        private void HandleCollision(IGameObject objectA, Rectangle boundsA, IGameObject objectB, Rectangle boundsB)
        {
            if (boundsA.Intersects(boundsB))
            {
                if (objectA is IPlayer && objectB is Door doorB) enterDoor?.Invoke(doorB.Direction);
                if (objectB is IPlayer && objectA is Door doorA) enterDoor?.Invoke(doorA.Direction);
                if (objectA is IPlayer hazardPlayerA && objectB is Block hazardB && hazardB.IsHazard)
                    DamagePlayer(hazardPlayerA);
                if (objectB is IPlayer hazardPlayerB && objectA is Block hazardA && hazardA.IsHazard)
                    DamagePlayer(hazardPlayerB);
            }
            if (objectA is IPlayer && objectB is IEnemy enemyB && !enemyB.IsDestroyed
                && boundsA.Intersects(boundsB))
            {
                DamagePlayer((IPlayer)objectA);
            }
            else if (objectB is IPlayer && objectA is IEnemy enemyA && !enemyA.IsDestroyed
                && boundsA.Intersects(boundsB))
            {
                DamagePlayer((IPlayer)objectB);
            }
        }

        private void HandleProjectileCollision(IProjectile projectile, Rectangle targetBounds, IGameObject target)
        {
            if (!projectile.IsActive || !SweptIntersects(projectile, targetBounds))
            {
                return;
            }

            if (projectile is Bomb bomb)
            {
                if (target is IEnemy bombTarget && bomb.Bounds.Intersects(targetBounds))
                {
                    bomb.DamageEnemy(bombTarget);
                }
                if (bomb.IsExploding && target is Block crate && bomb.Bounds.Intersects(targetBounds))
                {
                    crate.Destroy();
                }
            }
            else if (projectile is EnergyProjectile && target is IEnemy enemy)
            {
                if (projectile is PiercingProjectile piercing)
                {
                    piercing.Hit(enemy);
                    return;
                }
                if (!enemy.IsDestroyed)
                {
                    enemy.TakeDamage(1);
                }
                projectile.Deactivate();
            }
            else if (projectile is EnemyProjectile && target is IPlayer)
            {
                DamagePlayer((IPlayer)target);
                projectile.Deactivate();
            }
            else if (projectile is not Bomb && target is IBlock block && block.IsSolid)
            {
                if (projectile is EnergyProjectile && block is Block crate) crate.Destroy();
                projectile.Deactivate();
            }
        }

        private void DamagePlayer(IPlayer player)
        {
            if (damagePlayer != null)
            {
                damagePlayer();
            }
            else
            {
                player.TakeDamage();
            }
        }

        private static bool IsUnavailable(IGameObject gameObject)
        {
            return gameObject is IProjectile projectile && !projectile.IsActive
                || gameObject is IEnemy enemy && enemy.IsDestroyed;
        }

        private static bool TryGetBounds(IGameObject gameObject, out Rectangle bounds)
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

        private static bool SweptIntersects(IProjectile projectile, Rectangle target)
        {
            return !float.IsPositiveInfinity(ImpactTime(projectile, target));
        }

        private static float ImpactTime(IProjectile projectile, Rectangle target)
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
