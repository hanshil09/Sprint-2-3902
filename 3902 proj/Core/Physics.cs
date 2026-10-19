using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    internal class Physics
    {
        public const float DefaultGravity = 1500f;

        private IReadOnlyList<IBlock> blocks;
        private float verticalVelocity;

        public Physics(IReadOnlyList<IBlock> blocks, float gravity = DefaultGravity)
        {
            this.blocks = blocks;
            Gravity = gravity;
            verticalVelocity = 0;
            IsOnGround = false;
        }

        public bool IsOnGround { get; private set; }
        public float Gravity { get; private set; }

        public void SetBlocks(IReadOnlyList<IBlock> roomBlocks, float gravity = DefaultGravity)
        {
            blocks = roomBlocks;
            Gravity = gravity;
            verticalVelocity = 0;
            IsOnGround = false;
        }

        public void Jump(float speed)
        {
            if (IsOnGround)
            {
                verticalVelocity = -speed;
                IsOnGround = false;
            }
        }

        public Vector2 Apply(Vector2 position, int width, int height, GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float previousY = position.Y;
            verticalVelocity += Gravity * elapsed;
            float desiredY = position.Y + verticalVelocity * elapsed;
            IsOnGround = false;
            foreach (IBlock block in blocks)
            {
                if (!block.IsSolid || !OverlapsHorizontally(position.X, width, block.Bounds))
                {
                    continue;
                }

                if (verticalVelocity >= 0
                    && previousY + height <= block.Bounds.Top
                    && desiredY + height >= block.Bounds.Top)
                {
                    desiredY = System.Math.Min(desiredY, block.Bounds.Top - height);
                    verticalVelocity = 0;
                    IsOnGround = true;
                }
                else if (verticalVelocity < 0
                    && previousY >= block.Bounds.Bottom
                    && desiredY <= block.Bounds.Bottom)
                {
                    desiredY = System.Math.Max(desiredY, block.Bounds.Bottom);
                    verticalVelocity = 0;
                }
            }
            position.Y = desiredY;
            return position;
        }

        public Vector2 MoveHorizontally(Vector2 position, int width, int height, float distance, float minimumX, float maximumX)
        {
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
                    resolvedX = System.Math.Min(resolvedX, bounds.Left - width);
                }
                else if (distance < 0 && startX >= bounds.Right && desiredX < bounds.Right)
                {
                    resolvedX = System.Math.Max(resolvedX, bounds.Right);
                }
            }
            position.X = MathHelper.Clamp(resolvedX, minimumX, maximumX);
            return position;
        }

        public Vector2 MoveVertically(Vector2 position, int width, int height, float distance, float minimumY, float maximumY)
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
                    resolvedY = System.Math.Min(resolvedY, bounds.Top - height);
                }
                else if (distance < 0 && startY >= bounds.Bottom && desiredY < bounds.Bottom)
                {
                    resolvedY = System.Math.Max(resolvedY, bounds.Bottom);
                }
            }
            position.Y = MathHelper.Clamp(resolvedY, minimumY, maximumY);
            return position;
        }

        private static bool OverlapsHorizontally(float x, int width, Rectangle bounds)
        {
            return x + width > bounds.Left && x < bounds.Right;
        }

        private static bool OverlapsVertically(float y, int height, Rectangle bounds)
        {
            return y + height > bounds.Top && y < bounds.Bottom;
        }
    }
}
