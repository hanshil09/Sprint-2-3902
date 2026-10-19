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
        private readonly Rectangle worldBounds;
        private List<IGameObject> objects = new List<IGameObject>();

        public CollisionManager(int maxWidth, int maxHeight, Action damagePlayer = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxWidth);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxHeight);
            this.damagePlayer = damagePlayer;
            worldBounds = new Rectangle(0, 0, maxWidth, maxHeight);
        }

        public void BuildTree(List<IGameObject> objs)
        {
            objects = objs ?? throw new ArgumentNullException(nameof(objs));
        }

        public void DetectCollision()
        {
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
            if (objectA is IProjectile projectileA)
            {
                HandleProjectileCollision(projectileA, boundsB, objectB);
            }
            if (objectB is IProjectile projectileB)
            {
                HandleProjectileCollision(projectileB, boundsA, objectA);
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
            }
            else if (projectile is EnergyProjectile && target is IEnemy enemy)
            {
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
                case IProjectile projectile:
                    bounds = projectile.Bounds;
                    return true;
                case IBlock block:
                    bounds = block.Bounds;
                    return block.IsSolid;
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
            Rectangle current = projectile.Bounds;
            if (current.Intersects(target))
            {
                return true;
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
                && ClipAxis(startY, deltaY, minY, maxY, ref enter, ref exit);
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
