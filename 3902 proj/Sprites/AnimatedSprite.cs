using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Sprites
{
    public class AnimatedSprite : ISprite
    {
        private readonly Texture2D texture;
        private readonly Rectangle[] frames;
        private readonly Color tint;
        private readonly double millisecondsPerFrame;
        private readonly SpriteEffects effects;
        private int currentFrame;
        private int totalFrames;
        private double timeSinceLastFrame;

        public AnimatedSprite(Texture2D texture, Rectangle[] frames, int width, int height, Color tint, double millisecondsPerFrame = 90, SpriteEffects effects = SpriteEffects.None)
        {
            this.texture = texture;
            this.frames = frames;
            this.tint = tint;
            this.millisecondsPerFrame = millisecondsPerFrame;
            this.effects = effects;
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
            Rectangle sourceRectangle = frames[currentFrame];
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, Width, Height);
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, tint, 0f, Vector2.Zero, effects, 0f);
        }
    }
}
