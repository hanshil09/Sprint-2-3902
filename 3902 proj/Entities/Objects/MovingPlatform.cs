using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
   
    internal sealed class MovingPlatform : IMovingBlock
    {
        public const int Thickness = 16;
        private const double PauseSeconds = 0.6;

        private readonly ISprite tileSprite;
        private readonly int tileCount;
        private readonly Vector2 start;
        private readonly Vector2 travel;
        private readonly float speed;
        private float progress;
        private int direction = 1;
        private double pauseRemaining;

        public MovingPlatform(Vector2 start, int tileCount, Vector2 travel, float speed, ISprite tileSprite)
        {
            this.start = start;
            this.tileCount = tileCount;
            this.travel = travel;
            this.speed = speed;
            this.tileSprite = tileSprite;
            Position = start;
        }

        public Vector2 Position { get; set; }

        public Vector2 Delta { get; private set; }

        public bool IsSolid => true;

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, tileCount * tileSprite.Width, Thickness);

        public void Update(GameTime gameTime)
        {
            Vector2 before = Position;
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
            if (pauseRemaining > 0)
            {
                pauseRemaining -= elapsed;
            }
            else
            {
                Advance((float)elapsed);
            }
            Delta = Position - before;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < tileCount; i++)
            {
                tileSprite.Draw(spriteBatch, Position + new Vector2(i * tileSprite.Width, 0));
            }
        }

        private void Advance(float elapsed)
        {
            float length = travel.Length();
            progress += direction * speed * elapsed / length;
            if (progress >= 1f || progress <= 0f)
            {
                progress = MathHelper.Clamp(progress, 0f, 1f);
                direction = -direction;
                pauseRemaining = PauseSeconds;
            }
            Position = start + travel * progress;
        }
    }
}
