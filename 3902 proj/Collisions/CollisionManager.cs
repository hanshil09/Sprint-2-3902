using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Collisions;
using Microsoft.Xna.Framework;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.Collisions
{
    internal class CollisionManager
    {
        public const float PlayerFootInset = 16f;
        public const int EnemyLedgeInset = 12;

        private readonly Action damagePlayer;
        private readonly Action<int> enterDoor;
        private readonly Rectangle worldBounds;
        private readonly ConditionalWeakTable<IGameObject, BlockCollisionState> blockStates = new();
        private List<IGameObject> objects = new List<IGameObject>();

        public CollisionManager(int maxWidth, int maxHeight, Action damagePlayer = null, Action<int> enterDoor = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxWidth);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxHeight);
            this.damagePlayer = damagePlayer;
            this.enterDoor = enterDoor;
            worldBounds = new Rectangle(0, 0, maxWidth, maxHeight);
        }

        public void SetBlocks(IGameObject gameObject, IReadOnlyList<IBlock> blocks, float footInset = 0)
        {
            ArgumentNullException.ThrowIfNull(gameObject);
            ArgumentNullException.ThrowIfNull(blocks);
            blockStates.Remove(gameObject);
            blockStates.Add(gameObject, new BlockCollisionState(blocks, footInset));
        }

        public bool IsOnGround(IGameObject gameObject) => GetBlockState(gameObject).IsOnGround;

        public bool TryJump(IGameObject gameObject, Physics physics, float speed)
        {
            BlockCollisionState state = GetBlockState(gameObject);
            if (!state.IsOnGround)
            {
                return false;
            }
            physics.Jump(speed);
            state.IsOnGround = false;
            state.SupportingBlock = null;
            return true;
        }

        public void Launch(IGameObject gameObject, Physics physics, float speed)
        {
            physics.Launch(speed);
            BlockCollisionState state = GetBlockState(gameObject);
            state.IsOnGround = false;
            state.SupportingBlock = null;
        }

        public Vector2 UpdateVerticalMovement(IGameObject gameObject, Physics physics, Vector2 position,
            int width, int height, GameTime gameTime)
        {
            BlockCollisionState state = GetBlockState(gameObject);
            Vector2 previousPosition = position;
            Vector2 desiredPosition = physics.Apply(position, gameTime);
            position = ResolveVerticalMovement(previousPosition, desiredPosition, width, height,
                physics.VerticalVelocity, state.Blocks, state.FootInset, out bool isOnGround, out IBlock supportingBlock);
            if (position.Y != desiredPosition.Y)
            {
                physics.VerticalVelocity = 0;
            }
            state.IsOnGround = isOnGround;
            state.SupportingBlock = supportingBlock;
            return position;
        }

        public void UpdatePlayerMovement(Player player, Physics physics, GameTime gameTime, float horizontalDistance)
        {
            BlockCollisionState state = GetBlockState(player);
            bool wasOnGround = state.LastOnGround;
            PushPlayerFromMovingBlocks(player, state);
            if (state.SupportingBlock is IMovingBlock platform && platform.Delta != Vector2.Zero)
            {
                player.Position = MoveHorizontally(player.Position, player.Width, player.Height, platform.Delta.X,
                    0, worldBounds.Width - player.Width, state.Blocks);
                player.Position = new Vector2(player.Position.X, player.Position.Y + platform.Delta.Y);
            }

            player.Position = MoveHorizontally(player.Position, player.Width, player.Height, horizontalDistance,
                0, worldBounds.Width - player.Width, state.Blocks);
            Vector2 previousPosition = player.Position;
            Vector2 desiredPosition = physics.Apply(player.Position, gameTime);
            player.Position = ResolveVerticalMovement(previousPosition, desiredPosition, player.Width, player.Height,
                physics.VerticalVelocity, state.Blocks, state.FootInset, out bool isOnGround, out IBlock supportingBlock);
            if (player.Position.Y != desiredPosition.Y)
            {
                physics.VerticalVelocity = 0;
            }

            state.IsOnGround = isOnGround;
            state.LastOnGround = isOnGround;
            state.SupportingBlock = supportingBlock;
            Rectangle bounds = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);
            foreach (IBlock block in state.Blocks)
            {
                if (block is IStandingBlock standingBlock)
                {
                    standingBlock.SetPlayerBounds(bounds);
                }
            }
            if (supportingBlock is IStandingBlock supportingPlatform)
            {
                supportingPlatform.OnStand();
            }
            if (wasOnGround != isOnGround)
            {
                player.RefreshGroundState();
            }
        }

        public Vector2 MoveHorizontally(IGameObject gameObject, Vector2 position, int width, int height,
            float distance, float minimumX, float maximumX)
        {
            return MoveHorizontally(position, width, height, distance, minimumX, maximumX,
                GetBlockState(gameObject).Blocks);
        }

        public Vector2 MoveVertically(IGameObject gameObject, Vector2 position, int width, int height,
            float distance, float minimumY, float maximumY)
        {
            return MoveVertically(position, width, height, distance, minimumY, maximumY,
                GetBlockState(gameObject).Blocks);
        }

        public bool HasGroundAhead(IGameObject gameObject, int direction, float distance, Vector2 position, int width, int height)
        {
            float probeX = direction > 0
                ? position.X + width - EnemyLedgeInset + distance
                : position.X + EnemyLedgeInset - distance;
            Rectangle probe = new Rectangle((int)probeX, (int)(position.Y + height) + 1, 1, 2);
            foreach (IBlock block in GetBlockState(gameObject).Blocks)
            {
                if (IntersectsBlock(probe, block))
                {
                    return true;
                }
            }
            return false;
        }

        private void PushPlayerFromMovingBlocks(Player player, BlockCollisionState state)
        {
            Rectangle body = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);
            foreach (IBlock block in state.Blocks)
            {
                if (block is IMovingBlock platform && platform != state.SupportingBlock
                    && platform.Delta.X != 0 && IntersectsBlock(body, platform))
                {
                    float targetX = platform.Delta.X > 0 ? platform.Bounds.Right : platform.Bounds.Left - player.Width;
                    player.Position = MoveHorizontally(player.Position, player.Width, player.Height,
                        targetX - player.Position.X, 0, worldBounds.Width - player.Width, state.Blocks);
                }
            }
        }

        private BlockCollisionState GetBlockState(IGameObject gameObject)
        {
            ArgumentNullException.ThrowIfNull(gameObject);
            if (!blockStates.TryGetValue(gameObject, out BlockCollisionState state))
            {
                throw new InvalidOperationException("Block collision has not been configured for this game object.");
            }
            return state;
        }

        public void BuildTree(IReadOnlyList<IGameObject> objs)
        {
            ArgumentNullException.ThrowIfNull(objs);
            objects = new List<IGameObject>(objs);
        }

        public static Vector2 MoveHorizontally(Vector2 position, int width, int height, float distance,
            float minimumX, float maximumX, IReadOnlyList<IBlock> blocks)
        {
            ArgumentNullException.ThrowIfNull(blocks);
            float startX = position.X;
            float desiredX = MathHelper.Clamp(startX + distance, minimumX, maximumX);
            float resolvedX = desiredX;
            foreach (IBlock block in blocks)
            {
                Rectangle bounds = block.Bounds;
                if (!block.IsSolid || !OverlapsVertically(position.Y, height, bounds))
                {
                    continue;
                }

                if (distance > 0 && startX + width <= bounds.Left && desiredX + width > bounds.Left)
                {
                    resolvedX = Math.Min(resolvedX, bounds.Left - width);
                }
                else if (distance < 0 && startX >= bounds.Right && desiredX < bounds.Right)
                {
                    resolvedX = Math.Max(resolvedX, bounds.Right);
                }
            }
            position.X = MathHelper.Clamp(resolvedX, minimumX, maximumX);
            return position;
        }

        public static Vector2 MoveVertically(Vector2 position, int width, int height, float distance,
            float minimumY, float maximumY, IReadOnlyList<IBlock> blocks)
        {
            ArgumentNullException.ThrowIfNull(blocks);
            float startY = position.Y;
            float desiredY = MathHelper.Clamp(startY + distance, minimumY, maximumY);
            float resolvedY = desiredY;
            foreach (IBlock block in blocks)
            {
                Rectangle bounds = block.Bounds;
                if (!block.IsSolid || !OverlapsHorizontally(position.X, width, bounds))
                {
                    continue;
                }

                if (distance > 0 && startY + height <= bounds.Top && desiredY + height > bounds.Top)
                {
                    resolvedY = Math.Min(resolvedY, bounds.Top - height);
                }
                else if (distance < 0 && startY >= bounds.Bottom && desiredY < bounds.Bottom)
                {
                    resolvedY = Math.Max(resolvedY, bounds.Bottom);
                }
            }
            position.Y = MathHelper.Clamp(resolvedY, minimumY, maximumY);
            return position;
        }

        public static Vector2 ResolveVerticalMovement(Vector2 previousPosition, Vector2 desiredPosition,
            int width, int height, float verticalVelocity, IReadOnlyList<IBlock> blocks, float footInset,
            out bool isOnGround, out IBlock supportingBlock)
        {
            ArgumentNullException.ThrowIfNull(blocks);
            float desiredY = desiredPosition.Y;
            isOnGround = false;
            supportingBlock = null;
            float feetX = previousPosition.X + footInset;
            float feetWidth = width - 2 * footInset;
            foreach (IBlock block in blocks)
            {
                Rectangle bounds = block.Bounds;
                if (!block.IsSolid || !OverlapsHorizontally(previousPosition.X, width, bounds))
                {
                    continue;
                }

                if (verticalVelocity >= 0 && feetWidth > 0
                    && OverlapsHorizontally(feetX, feetWidth, bounds)
                    && previousPosition.Y + height <= bounds.Top
                    && desiredY + height >= bounds.Top)
                {
                    desiredY = Math.Min(desiredY, bounds.Top - height);
                    isOnGround = true;
                    supportingBlock = block;
                }
                else if (verticalVelocity < 0
                    && previousPosition.Y >= bounds.Bottom
                    && desiredY <= bounds.Bottom)
                {
                    desiredY = Math.Max(desiredY, bounds.Bottom);
                }
            }
            desiredPosition.Y = desiredY;
            return desiredPosition;
        }

        public static bool IntersectsBlock(Rectangle bounds, IBlock block)
        {
            ArgumentNullException.ThrowIfNull(block);
            return block.IsSolid && bounds.Intersects(block.Bounds);
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
                        || !CollisionHelper.TryGetBounds(target, out Rectangle bounds)) continue;
                    float impact = CollisionHelper.ImpactTime(projectile, bounds);
                    if (!float.IsPositiveInfinity(impact)) impacts.Add((target, bounds, impact));
                }
                impacts.Sort((a, b) => a.Time.CompareTo(b.Time));
                foreach (var impact in impacts)
                {
                    HandleProjectileCollision(projectile, impact.Bounds, impact.Target);
                    // A wall stops anything behind it from being hit this frame, even by weapons that survive the wall.
                    bool hitWall = impact.Target is IBlock wall && wall.IsSolid && projectile is not Bomb;
                    if (!projectile.IsActive || hitWall) break;
                }
            }
            for (int i = 0; i < objects.Count; i++)
            {
                IGameObject objectA = objects[i];
                if (!CollisionHelper.TryGetBounds(objectA, out Rectangle boundsA) || IsUnavailable(objectA))
                {
                    continue;
                }

                for (int j = i + 1; j < objects.Count; j++)
                {
                    IGameObject objectB = objects[j];
                    if (ReferenceEquals(objectA, objectB)
                        || !CollisionHelper.TryGetBounds(objectB, out Rectangle boundsB)
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
            switch (other)
            {
                case Door door:
                    enterDoor?.Invoke(door.Direction);
                    break;
                case IPlayerTrigger trigger:
                    trigger.OnPlayerContact(player);
                    break;
                case Block block when block.IsHazard:
                    DamagePlayer(player);
                    break;
                case IEnemy enemy when !enemy.IsDestroyed:
                    HandleEnemyContact(player, enemy);
                    break;
            }
        }

        // An invincible player (Spark Core) damages enemies by touching them instead of being hurt.
        private void HandleEnemyContact(IPlayer player, IEnemy enemy)
        {
            if (player.IsInvincible)
            {
                enemy.TakeDamage(1);
            }
            else
            {
                DamagePlayer(player);
            }
        }

        private void HandleProjectileCollision(IProjectile projectile, Rectangle targetBounds, IGameObject target)
        {
            if (!projectile.IsActive || !CollisionHelper.SweptIntersects(projectile, targetBounds))
            {
                return;
            }
            switch (projectile)
            {
                case Bomb bomb:
                    HandleBombCollision(bomb, targetBounds, target);
                    break;
                case IPlayerWeapon weapon:
                    HandleWeaponCollision(weapon, target);
                    break;
                case EnemyProjectile:
                    HandleEnemyShotCollision(projectile, target);
                    break;
            }
        }

        private static void HandleBombCollision(Bomb bomb, Rectangle targetBounds, IGameObject target)
        {
            if (!bomb.Bounds.Intersects(targetBounds))
            {
                return;
            }
            if (target is IEnemy enemy)
            {
                bomb.DamageEnemy(enemy);
            }
            else if (bomb.IsExploding && target is Block crate)
            {
                crate.Destroy();
            }
        }

        // Each weapon decides its own response (pierce, return, break crate...).
        private static void HandleWeaponCollision(IPlayerWeapon weapon, IGameObject target)
        {
            if (target is IEnemy enemy)
            {
                weapon.HitEnemy(enemy);
            }
            else if (target is IBlock block && block.IsSolid)
            {
                weapon.HitSolid(block);
            }
        }

        private void HandleEnemyShotCollision(IProjectile shot, IGameObject target)
        {
            if (target is IPlayer player)
            {
                DamagePlayer(player);
                shot.Deactivate();
            }
            else if (target is IBlock block && block.IsSolid)
            {
                shot.Deactivate();
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

        private static bool OverlapsHorizontally(float x, float width, Rectangle bounds)
        {
            return x + width > bounds.Left && x < bounds.Right;
        }

        private static bool OverlapsVertically(float y, int height, Rectangle bounds)
        {
            return y + height > bounds.Top && y < bounds.Bottom;
        }

        private sealed class BlockCollisionState
        {
            public BlockCollisionState(IReadOnlyList<IBlock> blocks, float footInset)
            {
                Blocks = blocks;
                FootInset = footInset;
            }

            public IReadOnlyList<IBlock> Blocks { get; }
            public float FootInset { get; }
            public bool IsOnGround { get; set; }
            public bool LastOnGround { get; set; }
            public IBlock SupportingBlock { get; set; }
        }
    }
}
