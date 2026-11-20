using System;
using System.Collections.Generic;
using ArmadilloGame.Entities;
using Microsoft.Xna.Framework;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Collisions
{
    internal static class BlockCollisionOperations
    {
        public static bool TryGetBounds(IGameObject gameObject, out Rectangle bounds)
        {
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
            float minimumX, float maximumX, IReadOnlyList<IBlock> blocks) =>
            MoveHorizontally(position, width, height, distance, minimumX, maximumX, blocks, out _);

        public static Vector2 MoveHorizontally(Vector2 position, int width, int height, float distance,
            float minimumX, float maximumX, IReadOnlyList<IBlock> blocks, out IBlock hitBlock)
        {
            float startX = position.X;
            float desiredX = MathHelper.Clamp(startX + distance, minimumX, maximumX);
            float resolvedX = desiredX;
            hitBlock = null;
            foreach (IBlock block in blocks)
            {
                Rectangle bounds = block.Bounds;
                if (!block.IsSolid || !OverlapsVertically(position.Y, height, bounds))
                {
                    continue;
                }

                if (distance > 0 && startX + width <= bounds.Left && resolvedX + width > bounds.Left)
                {
                    resolvedX = bounds.Left - width;
                    hitBlock = block;
                }
                else if (distance < 0 && startX >= bounds.Right && resolvedX < bounds.Right)
                {
                    resolvedX = bounds.Right;
                    hitBlock = block;
                }
            }
            position.X = MathHelper.Clamp(resolvedX, minimumX, maximumX);
            return position;
        }

        // Maximum vertical nudge (pixels) corner correction will apply to slip into an opening.
        private const int CornerCorrectTolerance = 6;

        // When a horizontal move is blocked, see whether nudging up or down by a few pixels lines the
        // body up with an adjacent opening. Returns the nudged position when one side works and the
        // nudged body itself is clear of solids; false when no small nudge helps.
        public static bool TryCornerCorrect(Vector2 position, int width, int height, float distance,
            IReadOnlyList<IBlock> blocks, out Vector2 corrected)
        {
            corrected = position;
            int direction = Math.Sign(distance);
            if (direction == 0)
            {
                return false;
            }

            // Probe a thin column just past the leading edge, in the direction of travel.
            float leadX = direction > 0 ? position.X + width : position.X - 1;
            for (int offset = 1; offset <= CornerCorrectTolerance; offset++)
            {
                foreach (int sign in new[] { -1, 1 })
                {
                    float candidateY = position.Y + sign * offset;
                    // The body at the nudged Y must be clear, and the path ahead at that Y must be open.
                    Rectangle body = new Rectangle((int)position.X, (int)candidateY, width, height);
                    Rectangle ahead = new Rectangle((int)leadX, (int)candidateY, 1, height);
                    if (IsClear(body, blocks) && IsClear(ahead, blocks))
                    {
                        corrected = new Vector2(position.X, candidateY);
                        return true;
                    }
                }
            }
            return false;
        }

        // Jumping into a one-cell gap: when a rising body clips a ceiling, see whether nudging left or
        // right by a few pixels lines it up with the gap. Returns the nudged position when one side
        // clears both the body and the ceiling strip it is heading toward.
        public static bool TryCeilingCornerCorrect(Vector2 position, int width, int height, float desiredY,
            IReadOnlyList<IBlock> blocks, out Vector2 corrected)
        {
            corrected = position;
            for (int offset = 1; offset <= CornerCorrectTolerance; offset++)
            {
                foreach (int sign in new[] { -1, 1 })
                {
                    float candidateX = position.X + sign * offset;
                    Rectangle body = new Rectangle((int)candidateX, (int)position.Y, width, height);
                    Rectangle above = new Rectangle((int)candidateX, (int)desiredY, width, 1);
                    if (IsClear(body, blocks) && IsClear(above, blocks))
                    {
                        corrected = new Vector2(candidateX, position.Y);
                        return true;
                    }
                }
            }
            return false;
        }

        public static Vector2 MoveVertically(Vector2 position, int width, int height, float distance,
            float minimumY, float maximumY, IReadOnlyList<IBlock> blocks)
        {
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
            float desiredY = desiredPosition.Y;
            isOnGround = false;
            supportingBlock = null;
            foreach (IBlock block in blocks)
            {
                Rectangle bounds = block.Bounds;
                if (!block.IsSolid || !OverlapsHorizontally(previousPosition.X, width, bounds))
                {
                    continue;
                }

                if (verticalVelocity >= 0 && width > 2 * footInset
                    && OverlapsHorizontally(previousPosition.X + footInset, width - 2 * footInset, bounds)
                    && previousPosition.Y + height <= bounds.Top
                    && desiredY + height >= bounds.Top)
                {
                    desiredY = bounds.Top - height;
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

        public static Vector2 RecoverOverlap(Vector2 position, Point size, Rectangle room, IReadOnlyList<IBlock> blocks)
        {
            Rectangle body = new Rectangle((int)position.X, (int)position.Y, size.X, size.Y);
            if (IsClear(body, blocks))
            {
                return position;
            }

            Vector2 best = position;
            float bestDistance = float.PositiveInfinity;
            foreach (IBlock block in blocks)
            {
                if (!IntersectsBlock(body, block))
                {
                    continue;
                }

                Rectangle bounds = block.Bounds;
                Vector2[] candidates =
                {
                    new Vector2(bounds.Left - size.X, position.Y),
                    new Vector2(bounds.Right, position.Y),
                    new Vector2(position.X, bounds.Top - size.Y),
                    new Vector2(position.X, bounds.Bottom)
                };

                foreach (Vector2 candidate in candidates)
                {
                    Rectangle spot = new Rectangle((int)candidate.X, (int)candidate.Y, size.X, size.Y);
                    float distance = Vector2.DistanceSquared(position, candidate);
                    if (distance < bestDistance && room.Contains(spot) && IsClear(spot, blocks))
                    {
                        best = candidate;
                        bestDistance = distance;
                    }
                }
            }

            return best;
        }

        private static bool IsClear(Rectangle body, IReadOnlyList<IBlock> blocks)
        {
            foreach (IBlock block in blocks)
            {
                if (IntersectsBlock(body, block))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IntersectsBlock(Rectangle bounds, IBlock block)
        {
            return block.IsSolid && bounds.Intersects(block.Bounds);
        }

        private static bool OverlapsHorizontally(float x, float width, Rectangle bounds) =>
            x + width > bounds.Left && x < bounds.Right;

        private static bool OverlapsVertically(float y, int height, Rectangle bounds) =>
            y + height > bounds.Top && y < bounds.Bottom;
    }
}
