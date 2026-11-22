
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArmadilloGame.Rendering
{
    internal sealed class ShadowRenderer : IDisposable
    {
        private const int ShadowAlpha = 5;
        private const int SpreadStep = 2;

        private readonly GraphicsDevice graphicsDevice;
        private readonly SpriteBatch spriteBatch;
        private readonly RenderTarget2D shadowTarget;

        public float Opacity { get; set; } = ShadowAlpha / 255f;
        public int Spread { get; set; } = 10;
        public Vector2 Offset { get; set; } = new Vector2(0f, 5f);

        public ShadowRenderer(
            GraphicsDevice graphicsDevice,
            SpriteBatch spriteBatch,
            int width,
            int height)
        {
            ArgumentNullException.ThrowIfNull(graphicsDevice);
            ArgumentNullException.ThrowIfNull(spriteBatch);

            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = spriteBatch;

            shadowTarget = new RenderTarget2D(
                graphicsDevice,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None);
        }

        public void Capture(
            Action<SpriteBatch> drawCasters,
            Matrix cameraTransform)
        {
            ArgumentNullException.ThrowIfNull(drawCasters);

            RenderTargetBinding[] previousTargets =
                graphicsDevice.GetRenderTargets();

            graphicsDevice.SetRenderTarget(shadowTarget);
            graphicsDevice.Clear(Color.Transparent);

            bool batchStarted = false;

            try
            {
                spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    transformMatrix: cameraTransform);

                batchStarted = true;

                drawCasters(spriteBatch);

                spriteBatch.End();
                batchStarted = false;
            }
            finally
            {
                if (batchStarted)
                {
                    spriteBatch.End();
                }

                graphicsDevice.SetRenderTargets(previousTargets);
            }
        }

        public void Draw()
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp);

            Color shadowColor = new Color(
                (byte)0,
                (byte)0,
                (byte)0,
                (byte)(MathHelper.Clamp(Opacity, 0f, 1f) * 255f));

            int spread = Math.Max(0, Spread);

            for (int x = -spread; x <= spread; x += SpreadStep)
            {
                for (int y = -spread; y <= spread; y += SpreadStep)
                {
                    float distance = Math.Abs(x) + Math.Abs(y);
                    float fade = 1f - distance / (spread * 2f + 2f);

                    if (fade <= 0f)
                    {
                        continue;
                    }

                    spriteBatch.Draw(
                        shadowTarget,
                        Offset + new Vector2(x, y),
                        shadowColor * fade);
                }
            }

            spriteBatch.End();
        }

        public void Dispose()
        {
            shadowTarget.Dispose();
        }
    }
}
