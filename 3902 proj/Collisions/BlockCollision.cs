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
    internal sealed class BlockCollision
    {
        public const float PlayerFootInset = 16f;
        public const int EnemyLedgeInset = 12;

        private readonly int worldWidth;
        private readonly ConditionalWeakTable<IGameObject, BlockCollisionState> states = new();

        public BlockCollision(int worldWidth)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(worldWidth);
            this.worldWidth = worldWidth;
        }

        public void SetBlocks(IGameObject gameObject, IReadOnlyList<IBlock> blocks, float footInset = 0)
        {
            ArgumentNullException.ThrowIfNull(gameObject);
            ArgumentNullException.ThrowIfNull(blocks);
            states.Remove(gameObject);
            states.Add(gameObject, new BlockCollisionState(blocks, footInset));
        }

        public bool IsOnGround(IGameObject gameObject) => GetState(gameObject).IsOnGround;

        public bool TryJump(IGameObject gameObject, Physics physics, float speed)
        {
            BlockCollisionState state = GetState(gameObject);
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
            BlockCollisionState state = GetState(gameObject);
            state.IsOnGround = false;
            state.SupportingBlock = null;
        }

        public Vector2 UpdateVerticalMovement(IGameObject gameObject, Physics physics, Vector2 position,
            int width, int height, GameTime gameTime)
        {
            BlockCollisionState state = GetState(gameObject);
            Vector2 previousPosition = position;
            Vector2 desiredPosition = physics.Apply(position, gameTime);
            position = BlockCollisionHelper.ResolveVerticalMovement(previousPosition, desiredPosition, width, height,
                physics.VerticalVelocity, state.Blocks, state.FootInset, out bool isOnGround, out IBlock supportingBlock);
            if (isOnGround || position.Y != desiredPosition.Y)
            {
                physics.VerticalVelocity = 0;
            }
            state.IsOnGround = isOnGround;
            state.SupportingBlock = supportingBlock;
            return position;
        }

        public void UpdatePlayerMovement(Player player, Physics physics, GameTime gameTime, float horizontalDistance,
            int worldHeight)
        {
            BlockCollisionState state = GetState(player);
            bool wasOnGround = state.LastOnGround;
            player.Position = ResolveMovingBlockMovement(player, state);

            player.Position = BlockCollisionHelper.MoveHorizontally(player.Position, player.Width, player.Height, horizontalDistance,
                0, worldWidth - player.Width, state.Blocks);
            Vector2 previousPosition = player.Position;
            Vector2 desiredPosition = physics.Apply(player.Position, gameTime);
            player.Position = BlockCollisionHelper.ResolveVerticalMovement(previousPosition, desiredPosition, player.Width, player.Height,
                physics.VerticalVelocity, state.Blocks, state.FootInset, out bool isOnGround, out IBlock supportingBlock);
            if (isOnGround || player.Position.Y != desiredPosition.Y)
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
            float distance, float minimumX, float maximumX) =>
            BlockCollisionHelper.MoveHorizontally(position, width, height, distance,
                minimumX, maximumX, GetState(gameObject).Blocks);

        public Vector2 MoveVertically(IGameObject gameObject, Vector2 position, int width, int height,
            float distance, float minimumY, float maximumY) =>
            BlockCollisionHelper.MoveVertically(position, width, height, distance,
                minimumY, maximumY, GetState(gameObject).Blocks);

        public bool HasGroundAhead(IGameObject gameObject, int direction, float distance,
            Vector2 position, int width, int height)
        {
            float probeX = direction > 0
                ? position.X + width - EnemyLedgeInset + distance
                : position.X + EnemyLedgeInset - distance;
            Rectangle probe = new Rectangle((int)probeX, (int)(position.Y + height) + 1, 1, 2);
            foreach (IBlock block in GetState(gameObject).Blocks)
            {
                if (BlockCollisionHelper.IntersectsBlock(probe, block))
                {
                    return true;
                }
            }
            return false;
        }

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
                case Bomb bomb when bomb.Bounds.Intersects(targetBounds) && bomb.IsExploding && block is Block crate:
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

        private Vector2 ResolveMovingBlockMovement(Player player, BlockCollisionState state)
        {
            Rectangle body = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);
            foreach (IBlock block in state.Blocks)
            {
                if (block is IMovingBlock platform && platform != state.SupportingBlock
                    && platform.Delta.X != 0 && BlockCollisionHelper.IntersectsBlock(body, platform))
                {
                    float targetX = platform.Delta.X > 0 ? platform.Bounds.Right : platform.Bounds.Left - player.Width;
                    player.Position = BlockCollisionHelper.MoveHorizontally(player.Position, player.Width, player.Height,
                        targetX - player.Position.X, 0, worldWidth - player.Width, state.Blocks);
                    body = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);
                }
            }

            if (state.SupportingBlock is IMovingBlock supportingPlatform
                && supportingPlatform.Delta != Vector2.Zero)
            {
                player.Position = BlockCollisionHelper.MoveHorizontally(player.Position, player.Width,
                    player.Height, supportingPlatform.Delta.X, 0, worldWidth - player.Width, state.Blocks);
                float playerY = player.Position.Y;
                if (supportingPlatform.Delta.Y != 0)
                {
                    playerY = supportingPlatform.Bounds.Top - player.Height;
                }
                player.Position = new Vector2(player.Position.X, playerY);
            }
            return player.Position;
        }

        private BlockCollisionState GetState(IGameObject gameObject)
        {
            ArgumentNullException.ThrowIfNull(gameObject);
            if (!states.TryGetValue(gameObject, out BlockCollisionState state))
            {
                throw new InvalidOperationException("Block collision has not been configured for this game object.");
            }
            return state;
        }

        private class BlockCollisionState
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
