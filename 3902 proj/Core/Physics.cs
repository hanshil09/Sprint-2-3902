using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    public class Physics
    {
        private const float Gravity = 1500f;

        private List<IBlock> blocks;
        private float verticalVelocity;

        public Physics(List<IBlock> blocks)
        {
            this.blocks = blocks;
            verticalVelocity = 0;
            IsOnGround = false;
        }

        public bool IsOnGround { get; private set; }

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
            float previousBottom = position.Y + height;
            verticalVelocity += Gravity * elapsed;
            position.Y += verticalVelocity * elapsed;
            IsOnGround = false;
            if (verticalVelocity >= 0)
            {
                position.Y = LandOnBlocks(position, width, height, previousBottom);
            }
            return position;
        }

        private float LandOnBlocks(Vector2 position, int width, int height, float previousBottom)
        {
            foreach (IBlock block in blocks)
            {
                if (block.IsSolid && IsLandingOn(block.Bounds, position, width, height, previousBottom))
                {
                    verticalVelocity = 0;
                    IsOnGround = true;
                    return block.Bounds.Top - height;
                }
            }
            return position.Y;
        }

        private static bool IsLandingOn(Rectangle bounds, Vector2 position, int width, int height, float previousBottom)
        {
            bool overlapsHorizontally = position.X + width > bounds.Left && position.X < bounds.Right;
            bool crossesTop = previousBottom <= bounds.Top && position.Y + height >= bounds.Top;
            return overlapsHorizontally && crossesTop;
        }
    }
}
