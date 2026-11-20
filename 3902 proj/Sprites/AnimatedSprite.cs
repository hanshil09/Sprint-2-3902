using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Sprites
{
    internal class AnimatedSprite : ISprite
    {
        private readonly Texture2D texture;
        private readonly Rectangle[] frames;
        private readonly Color tint;
        private readonly double millisecondsPerFrame;
        private readonly SpriteEffects effects;
        // When true each frame is drawn at its true aspect, scaled to the logical height and
        // bottom-center anchored, so animation frames of different sizes do not squish or jitter.
        private readonly bool preserveAspect;
        private readonly int visualInflate;
        private int currentFrame;
        private int totalFrames;
        private double timeSinceLastFrame;

        public AnimatedSprite(Texture2D texture, Rectangle[] frames, int width, int height, Color tint,
            double millisecondsPerFrame = 90, SpriteEffects effects = SpriteEffects.None, bool preserveAspect = false,
            int visualInflate = 0)
        {
            this.texture = texture;
            this.frames = frames;
            this.tint = tint;
            this.millisecondsPerFrame = millisecondsPerFrame;
            this.effects = effects;
            this.preserveAspect = preserveAspect;
            this.visualInflate = visualInflate;
            Width = width;
            Height = height;
            currentFrame = 0;
            totalFrames = frames.Length;
            timeSinceLastFrame = 0;
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public void Update(GameTime gameTime)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.TotalMilliseconds;
            while (timeSinceLastFrame >= millisecondsPerFrame)
            {
                timeSinceLastFrame -= millisecondsPerFrame;
                currentFrame = (currentFrame + 1) % totalFrames;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Rectangle source = frames[currentFrame];
            Rectangle destination = preserveAspect
                ? SpriteDraw.BottomCenterFit(source, location, Width, Height)
                : new Rectangle((int)location.X - visualInflate, (int)location.Y - visualInflate,
                    Width + 2 * visualInflate, Height + 2 * visualInflate);
            spriteBatch.Draw(texture, destination, source, tint, 0f, Vector2.Zero, effects, 0f);
        }
    }
}
