using System;
using System.Collections.Generic;
using Collisions;
using Microsoft.Xna.Framework;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.Collisions
{
    internal class CollisionManager
    {
        public const float PlayerFootInset = BlockCollision.PlayerFootInset;
        public const int EnemyLedgeInset = BlockCollision.EnemyLedgeInset;

        private readonly Action damagePlayer;
        private readonly Action<int> enterDoor;
        private readonly Rectangle worldBounds;
        private readonly BlockCollision blockCollision;
        private List<IGameObject> objects = new List<IGameObject>();

        public CollisionManager(int maxWidth, int maxHeight, Action damagePlayer = null, Action<int> enterDoor = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxWidth);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxHeight);
            this.damagePlayer = damagePlayer;
            this.enterDoor = enterDoor;
            worldBounds = new Rectangle(0, 0, maxWidth, maxHeight);
            blockCollision = new BlockCollision(maxWidth);
        }

        public void SetBlocks(IGameObject gameObject, IReadOnlyList<IBlock> blocks, float footInset = 0) =>
            blockCollision.SetBlocks(gameObject, blocks, footInset);

        public bool IsOnGround(IGameObject gameObject) => blockCollision.IsOnGround(gameObject);

        public bool TryJump(IGameObject gameObject, Physics physics, float speed) =>
            blockCollision.TryJump(gameObject, physics, speed);

        public void Launch(IGameObject gameObject, Physics physics, float speed) =>
            blockCollision.Launch(gameObject, physics, speed);

        public Vector2 UpdateVerticalMovement(IGameObject gameObject, Physics physics, Vector2 position,
            int width, int height, GameTime gameTime) =>
            blockCollision.UpdateVerticalMovement(gameObject, physics, position, width, height, gameTime);

        public void UpdatePlayerMovement(Player player, Physics physics, GameTime gameTime, float horizontalDistance) =>
            blockCollision.UpdatePlayerMovement(player, physics, gameTime, horizontalDistance, worldBounds.Height);

        public Vector2 MoveHorizontally(IGameObject gameObject, Vector2 position, int width, int height,
            float distance, float minimumX, float maximumX) =>
            blockCollision.MoveHorizontally(gameObject, position, width, height, distance, minimumX, maximumX);

        public Vector2 MoveVertically(IGameObject gameObject, Vector2 position, int width, int height,
            float distance, float minimumY, float maximumY) =>
            blockCollision.MoveVertically(gameObject, position, width, height, distance, minimumY, maximumY);

        public bool HasGroundAhead(IGameObject gameObject, int direction, float distance,
            Vector2 position, int width, int height) =>
            blockCollision.HasGroundAhead(gameObject, direction, distance, position, width, height);

        public static Vector2 MoveHorizontally(Vector2 position, int width, int height, float distance,
            float minimumX, float maximumX, IReadOnlyList<IBlock> blocks) =>
            BlockCollisionHelper.MoveHorizontally(position, width, height, distance, minimumX, maximumX, blocks);

        public static Vector2 MoveVertically(Vector2 position, int width, int height, float distance,
            float minimumY, float maximumY, IReadOnlyList<IBlock> blocks) =>
            BlockCollisionHelper.MoveVertically(position, width, height, distance, minimumY, maximumY, blocks);

        public static Vector2 ResolveVerticalMovement(Vector2 previousPosition, Vector2 desiredPosition,
            int width, int height, float verticalVelocity, IReadOnlyList<IBlock> blocks, float footInset,
            out bool isOnGround, out IBlock supportingBlock) =>
            BlockCollisionHelper.ResolveVerticalMovement(previousPosition, desiredPosition, width, height,
                verticalVelocity, blocks, footInset, out isOnGround, out supportingBlock);

        public static bool IntersectsBlock(Rectangle bounds, IBlock block) =>
            BlockCollisionHelper.IntersectsBlock(bounds, block);

        public void BuildTree(IReadOnlyList<IGameObject> objs)
        {
            ArgumentNullException.ThrowIfNull(objs);
            objects = new List<IGameObject>(objs);
        }

        public void DetectCollision()
        {
            foreach (IGameObject gameObject in objects)
            {
                if (gameObject is not IProjectile projectile || !projectile.IsActive) continue;
                List<(IGameObject Target, Rectangle Bounds, float Time)> impacts = new();
                foreach (IGameObject target in objects)
                {
                    if (ReferenceEquals(projectile, target) || IsUnavailable(target)
                        || !TryGetBounds(target, out Rectangle bounds)) continue;
                    float impact = CollisionHelper.ImpactTime(projectile, bounds);
                    if (!float.IsPositiveInfinity(impact)) impacts.Add((target, bounds, impact));
                }
                impacts.Sort((a, b) => a.Time.CompareTo(b.Time));
                foreach (var impact in impacts)
                {
                    HandleProjectileCollision(projectile, impact.Bounds, impact.Target);
                    bool hitWall = BlockCollisionHelper.StopsProjectileTraversal(impact.Target, projectile);
                    if (!projectile.IsActive || hitWall) break;
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
            if (!boundsA.Intersects(boundsB))
            {
                return;
            }
            if (objectA is IPlayer playerA)
            {
                HandlePlayerContact(playerA, objectB);
            }
            else if (objectB is IPlayer playerB)
            {
                HandlePlayerContact(playerB, objectA);
            }
        }

        private void HandlePlayerContact(IPlayer player, IGameObject other)
        {
            if (BlockCollision.HandlePlayerContact(player, other, DamagePlayer))
            {
                return;
            }
            PlayerCollision.HandleContact(player, other, enterDoor, DamagePlayer);
        }

        private void HandleProjectileCollision(IProjectile projectile, Rectangle targetBounds, IGameObject target)
        {
            ProjectileCollision.HandleImpact(projectile, targetBounds, target, DamagePlayer);
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

        private static bool TryGetBounds(IGameObject gameObject, out Rectangle bounds)
        {
            return BlockCollisionHelper.TryGetBounds(gameObject, out bounds)
                || CollisionHelper.TryGetBounds(gameObject, out bounds);
        }

        private static bool IsUnavailable(IGameObject gameObject)
        {
            return gameObject is IProjectile projectile && !projectile.IsActive
                || gameObject is IEnemy enemy && enemy.IsDestroyed;
        }
    }
}
