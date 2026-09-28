using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Sprites
{
    public class AnimatedSprite : ISprite
    {
        private const double MillisecondsPerFrame = 100;

        private Texture2D texture;
        private Rectangle[] frames;
        private Color tint;
        private int currentFrame;
        private int totalFrames;
        private double timeSinceLastFrame;

        public AnimatedSprite(Texture2D texture, Rectangle[] frames, int width, int height, Color tint)
        {
            this.texture = texture;
            this.frames = frames;
            this.tint = tint;
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
            if (timeSinceLastFrame >= MillisecondsPerFrame)
            {
                timeSinceLastFrame -= MillisecondsPerFrame;
                currentFrame = (currentFrame + 1) % totalFrames;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Rectangle sourceRectangle = frames[currentFrame];
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, Width, Height);
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, tint);
        }
    }
}
