using System;
using System.Collections.Generic;
using TransformersGame.Entities;
using Microsoft.Xna.Framework;
using TransformersGame.Interfaces;

namespace TransformersGame.Collisions
{
    internal static class BlockCollisionHelper
    {
        public static bool TryGetBounds(IGameObject gameObject, out Rectangle bounds)
        {
            ArgumentNullException.ThrowIfNull(gameObject);
            if (gameObject is IBlock block)
            {
                bounds = block.Bounds;
                return block.IsSolid || block is Block hazard && hazard.IsHazard;
            }
            bounds = Rectangle.Empty;
            return false;
        }

        public static bool StopsProjectileTraversal(IGameObject target, IProjectile projectile) =>
            target is IBlock { IsSolid: true } && projectile is not Bomb;

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

        private static bool OverlapsHorizontally(float x, float width, Rectangle bounds) =>
            x + width > bounds.Left && x < bounds.Right;

        private static bool OverlapsVertically(float y, int height, Rectangle bounds) =>
            y + height > bounds.Top && y < bounds.Bottom;
    }
}
