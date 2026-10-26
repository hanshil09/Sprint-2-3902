using Microsoft.Xna.Framework;

namespace TransformersGame.Core
{
    internal class Physics
    {
        public const float DefaultGravity = 1500f;

        public Physics(float gravity = DefaultGravity)
        {
            Gravity = gravity;
        }

        public float Gravity { get; private set; }

        public float VerticalVelocity { get; set; }

        public void SetGravity(float gravity)
        {
            Gravity = gravity;
            VerticalVelocity = 0;
        }

        public void Jump(float speed)
        {
            VerticalVelocity = -speed;
        }

        public void Launch(float speed)
        {
            Jump(speed);
        }

        public Vector2 Apply(Vector2 position, GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            VerticalVelocity += Gravity * elapsed;
            position.Y += VerticalVelocity * elapsed;
            return position;
        }
    }
}
