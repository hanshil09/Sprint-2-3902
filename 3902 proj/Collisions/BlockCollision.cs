using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using ArmadilloGame.Core;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Collisions
{
    internal sealed class BlockCollision
    {
        public const float PlayerFootInset = 16f;
        public const int EnemyLedgeInset = 12;

        private Rectangle worldBounds;
        private readonly ConditionalWeakTable<IGameObject, BlockCollisionState> states = new();

        public BlockCollision(Rectangle worldBounds)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(worldBounds.Width);
            this.worldBounds = worldBounds;
        }

        public void SetWorldBounds(Rectangle bounds)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bounds.Width);
            worldBounds = bounds;
        }

        public void SetBlocks(IGameObject gameObject, IReadOnlyList<IBlock> blocks, float footInset = 0)
        {
            ArgumentNullException.ThrowIfNull(gameObject);
            ArgumentNullException.ThrowIfNull(blocks);
            states.Remove(gameObject);
            states.Add(gameObject, new BlockCollisionState(blocks, footInset));
        }

        public bool IsOnGround(IGameObject gameObject) => GetState(gameObject).IsOnGround;

        public bool IsSpaceAvailable(IGameObject gameObject, Rectangle bounds)
        {
            foreach (IBlock block in GetState(gameObject).Blocks)
            {
                if (BlockCollisionOperations.IntersectsBlock(bounds, block)) return false;
            }
            return true;
        }

        public bool TryGroundedJump(IGameObject gameObject, Physics physics, float speed)
        {
            BlockCollisionState state = GetState(gameObject);
            if (!state.IsOnGround)
            {
                return false;
            }
            physics.Jump(speed);
            state.IsOnGround = false;
            // Apply the platform's next movement before resolving the takeoff.
            return true;
        }

        public void Launch(IGameObject gameObject, Physics physics, float speed)
        {
            physics.Launch(speed);
            BlockCollisionState state = GetState(gameObject);
            state.IsOnGround = false;
            state.SupportingBlock = null;
        }

        public Vector2 ApplyGravityAndResolveVerticalCollisions(IGameObject gameObject, Physics physics, Vector2 position,
            int width, int height, GameTime gameTime)
        {
            BlockCollisionState state = GetState(gameObject);
            Vector2 previousPosition = position;
            Vector2 desiredPosition = physics.Apply(position, gameTime);
            position = BlockCollisionOperations.ResolveVerticalMovement(previousPosition, desiredPosition, width, height,
                physics.VerticalVelocity, state.Blocks, state.FootInset, out bool isOnGround, out IBlock supportingBlock);
            if (isOnGround || position.Y != desiredPosition.Y)
            {
                physics.VerticalVelocity = 0;
            }
            state.IsOnGround = isOnGround;
            state.SupportingBlock = supportingBlock;
            return position;
        }

        public void ResolveFrameMovement(IGameObject entity, int width, int height, Physics physics, GameTime gameTime, float horizontalDistance)
        {
            BlockCollisionState state = GetState(entity);
            bool wasOnGround = state.LastOnGround;
            if (entity is IPlayer)
            {
                entity.Position = MovingPlatformCollision.ResolvePlayerPosition(entity.Position, new Point(width, height),
                    worldBounds, state.Blocks, state.SupportingBlock);
                entity.Position = BlockCollisionOperations.RecoverOverlap(entity.Position, new Point(width, height),
                    worldBounds, state.Blocks);
            }
            entity.Position = entity is Player rollingPlayer
                ? MovePlayerHorizontally(rollingPlayer, horizontalDistance, state.Blocks)
                : BlockCollisionOperations.MoveHorizontally(entity.Position, width, height, horizontalDistance,
                    worldBounds.Left, worldBounds.Right - width, state.Blocks);
            Vector2 previousPosition = entity.Position;
            Vector2 desiredPosition = physics.Apply(entity.Position, gameTime);
            entity.Position = BlockCollisionOperations.ResolveVerticalMovement(previousPosition, desiredPosition, width, height,
                physics.VerticalVelocity, state.Blocks, 0, out bool isOnGround, out IBlock supportingBlock);
            if (isOnGround || entity.Position.Y != desiredPosition.Y)
            {
                physics.VerticalVelocity = 0;
            }

            state.IsOnGround = isOnGround;
            state.LastOnGround = isOnGround;
            state.SupportingBlock = supportingBlock;
            Rectangle bounds = new Rectangle((int)entity.Position.X, (int)entity.Position.Y, width, height);
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
            if (entity is Player && wasOnGround != isOnGround)
            {
                Player player = (Player)entity;
                RefreshGroundState(player);
            }
        }

        private static void RefreshGroundState(Player player)
        {
            player.RefreshGroundState();
        }

        private Vector2 MovePlayerHorizontally(Player player, float distance, IReadOnlyList<IBlock> blocks)
        {
            float minimumX = worldBounds.Left;
            float maximumX = worldBounds.Right - player.Width;
            Vector2 resolved = BlockCollisionOperations.MoveHorizontally(player.Position, player.Width, player.Height,
                distance, minimumX, maximumX, blocks, out IBlock hitBlock);
            if (player.IsInBallForm && hitBlock is SupplyCrate crate && crate.TryBreakFromRoll())
            {
                player.PlaySound(GameSound.CrateBreak);
                // Finish the movement against the remaining solid blocks.
                resolved = BlockCollisionOperations.MoveHorizontally(player.Position, player.Width, player.Height,
                    distance, minimumX, maximumX, blocks);
            }
            return resolved;
        }

        public Vector2 MoveHorizontally(IGameObject gameObject, Vector2 position, int width, int height,
            float distance, float minimumX, float maximumX) =>
            BlockCollisionOperations.MoveHorizontally(position, width, height, distance,
                minimumX, maximumX, GetState(gameObject).Blocks);

        public Vector2 MoveVertically(IGameObject gameObject, Vector2 position, int width, int height,
            float distance, float minimumY, float maximumY) =>
            BlockCollisionOperations.MoveVertically(position, width, height, distance,
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
                if (BlockCollisionOperations.IntersectsBlock(probe, block))
                {
                    return true;
                }
            }
            return false;
        }

        private BlockCollisionState GetState(IGameObject gameObject)
        {
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
