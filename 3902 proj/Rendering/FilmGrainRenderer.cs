
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArmadilloGame.Rendering
{
    internal sealed class FilmGrainRenderer : IDisposable
    {
        private const int TextureWidth = 512;
        private const int TextureHeight = 512;
        private const int TextureSize = TextureWidth * TextureHeight;

        private const double FrameInterval = 1.0 / 18.0;
        private const float DefaultIntensity = 0.035f;

        private readonly Texture2D noiseTexture;
        private readonly Color[] noiseData = new Color[TextureSize];
        private readonly Random random = new Random();

        private double timeUntilNextFrame;

        public float Intensity { get; set; } = DefaultIntensity;

        public FilmGrainRenderer(GraphicsDevice graphicsDevice)
        {
            ArgumentNullException.ThrowIfNull(graphicsDevice);

            noiseTexture = new Texture2D(
                graphicsDevice,
                TextureWidth,
                TextureHeight,
                false,
                SurfaceFormat.Color);

            GenerateNoise();
        }

        public void Update(GameTime gameTime)
        {
            ArgumentNullException.ThrowIfNull(gameTime);

            timeUntilNextFrame -= gameTime.ElapsedGameTime.TotalSeconds;

            if (timeUntilNextFrame > 0)
            {
                return;
            }

            GenerateNoise();
            timeUntilNextFrame += FrameInterval;

            if (timeUntilNextFrame <= 0)
            {
                timeUntilNextFrame = FrameInterval;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle destination)
        {
            ArgumentNullException.ThrowIfNull(spriteBatch);

            float intensity = MathHelper.Clamp(Intensity, 0f, 1f);

            if (intensity <= 0f)
            {
                return;
            }

            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp);

            spriteBatch.Draw(
                noiseTexture,
                destination,
                Color.White * intensity);

            spriteBatch.End();
        }

        private void GenerateNoise()
        {
            for (int i = 0; i < noiseData.Length; i++)
            {
                byte value = (byte)random.Next(256);

                noiseData[i] = new Color(
                    value,
                    value,
                    value,
                    byte.MaxValue);
            }

            noiseTexture.SetData(noiseData);
        }

        public void Dispose()
        {
            noiseTexture.Dispose();
        }
    }
}
