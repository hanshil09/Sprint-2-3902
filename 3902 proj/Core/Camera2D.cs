using Microsoft.Xna.Framework;

namespace TransformersGame.Core
{
    // A 2D camera that follows a target and clamps to the room's world bounds.
    // Movement is smoothed with a frame-rate independent lerp so scrolling feels fluid.
    internal sealed class Camera2D
    {
        private readonly int viewWidth;
        private readonly int viewHeight;
        private readonly float followSpeed;
        private Vector2 position; // Top-left corner of the view in world space.
        private bool initialized;

        public Camera2D(int viewWidth, int viewHeight, float followSpeed = 8f)
        {
            this.viewWidth = viewWidth;
            this.viewHeight = viewHeight;
            this.followSpeed = followSpeed;
        }

        // View transform to pass to SpriteBatch.Begin for world-space rendering.
        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(-(float)System.Math.Round(position.X), -(float)System.Math.Round(position.Y), 0f);
        }

        public Rectangle ViewBounds =>
            new Rectangle((int)System.Math.Round(position.X), (int)System.Math.Round(position.Y),
                viewWidth, viewHeight);

        // Follow the target (its center) and clamp to the room bounds. Called each gameplay frame.
        public void Follow(Rectangle target, Rectangle worldBounds, GameTime gameTime)
        {
            Vector2 desired = ClampToBounds(CenterOn(target), worldBounds);
            if (!initialized)
            {
                position = desired;
                initialized = true;
                return;
            }
            float t = 1f - (float)System.Math.Exp(-followSpeed * gameTime.ElapsedGameTime.TotalSeconds);
            position = Vector2.Lerp(position, desired, t);
            position = ClampToBounds(position, worldBounds);
        }

        // Snap instantly to the target, e.g. right after a room switch so there is no slide.
        public void SnapTo(Rectangle target, Rectangle worldBounds)
        {
            position = ClampToBounds(CenterOn(target), worldBounds);
            initialized = true;
        }

        private Vector2 CenterOn(Rectangle target)
        {
            return new Vector2(
                target.Center.X - viewWidth / 2f,
                target.Center.Y - viewHeight / 2f);
        }

        private Vector2 ClampToBounds(Vector2 candidate, Rectangle worldBounds)
        {
            float x;
            if (worldBounds.Width <= viewWidth)
            {
                // Room narrower than the view: center it horizontally.
                x = worldBounds.Left + (worldBounds.Width - viewWidth) / 2f;
            }
            else
            {
                x = MathHelper.Clamp(candidate.X, worldBounds.Left, worldBounds.Right - viewWidth);
            }

            float y;
            if (worldBounds.Height <= viewHeight)
            {
                y = worldBounds.Top + (worldBounds.Height - viewHeight) / 2f;
            }
            else
            {
                y = MathHelper.Clamp(candidate.Y, worldBounds.Top, worldBounds.Bottom - viewHeight);
            }

            return new Vector2(x, y);
        }
    }
}
