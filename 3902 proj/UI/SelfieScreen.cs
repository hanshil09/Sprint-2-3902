using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Factories;
using TransformersGame.Interfaces;
using TransformersGame.States;

namespace TransformersGame.UI
{
    internal sealed class SelfieScreen : IDisposable
    {
        private const int PreviewWidth = 224;
        private const int PreviewHeight = 126;
        private readonly GraphicsDevice graphics;
        private readonly SelfieCast cast;
        private readonly ControlsOverlay overlay;
        private readonly RenderTarget2D photo;
        private readonly Texture2D whitePixel;
        private readonly SelfieSequence sequence = new();
        private Matrix view;
        private Task<bool> save;

        public SelfieScreen(GraphicsDevice graphics, SelfieSpriteFactory sprites,
            ControlsOverlay overlay, int width, int height)
        {
            this.graphics = graphics;
            this.overlay = overlay;
            photo = new RenderTarget2D(graphics, width, height, false, SurfaceFormat.Color,
                DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            cast = new SelfieCast(sprites);
            whitePixel = new Texture2D(graphics, 1, 1);
            whitePixel.SetData(new[] { Color.White });
        }

        public bool Finished => sequence.Phase == SelfiePhase.Finished;

        public bool Start(Room room, IPlayer player, Matrix cameraView)
        {
            Rectangle visibleWorld = new Rectangle(-(int)cameraView.M41, -(int)cameraView.M42,
                photo.Width, photo.Height);
            if (!cast.Prepare(room, player, visibleWorld)) return false;

            view = cameraView;
            sequence.Start(cast.RunDistance);
            save = null;
            return true;
        }

        public void Update(GameTime gameTime) => sequence.Update(gameTime.ElapsedGameTime.TotalSeconds);

        public bool Draw(SpriteBatch batch, Room room, Color background)
        {
            bool capturing = sequence.Phase == SelfiePhase.Capture;
            if (capturing)
            {
                Capture(batch, room, background);
            }
            graphics.Clear(background);
            DrawScene(batch, room);
            batch.Begin(samplerState: SamplerState.PointClamp);
            DrawOverlay(batch);
            batch.End();
            return capturing;
        }

        public void Dispose()
        {
            whitePixel.Dispose();
            photo.Dispose();
        }

        private void Capture(SpriteBatch batch, Room room, Color background)
        {
            graphics.SetRenderTarget(photo);
            try
            {
                graphics.Clear(background);
                DrawScene(batch, room);
            }
            finally
            {
                graphics.SetRenderTarget(null);
            }
            save = SelfiePhoto.SaveAsync(photo);
            sequence.PhotoTaken();
        }

        private void DrawScene(SpriteBatch batch, Room room)
        {
            batch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: view);
            if (sequence.IsPosing)
            {
                room.Level.Draw(batch);
                room.Projectiles.Draw(batch);
                cast.DrawEnemies(batch);
            }
            else
            {
                room.Draw(batch);
            }
            cast.DrawPlayer(batch, sequence.IsPosing);
            cast.DrawClick(batch, sequence);
            batch.End();
        }

        private void DrawOverlay(SpriteBatch batch)
        {
            if (sequence.Phase == SelfiePhase.Leaving)
            {
                if (sequence.FlashOpacity > 0)
                {
                    batch.Draw(whitePixel, photo.Bounds, Color.White * sequence.FlashOpacity);
                }
                else
                {
                    DrawPreview(batch);
                }
                return;
            }
            string message = sequence.IsPosing ? "Say Energon!" : "Selfie time!";
            overlay.DrawPanel(batch, new Rectangle(24, 20, 268, 62));
            overlay.DrawSmallText(batch, message, 42, 28, true);
            overlay.DrawSmallText(batch, "Esc: skip scene    M: mute audio", 42, 51, false);
        }

        private void DrawPreview(SpriteBatch batch)
        {
            int x = photo.Width - PreviewWidth - 22;
            const int y = 24;
            overlay.DrawPanel(batch, new Rectangle(x - 8, y - 8, PreviewWidth + 16, PreviewHeight + 36));
            batch.Draw(photo, new Rectangle(x, y, PreviewWidth, PreviewHeight), Color.White);
            string status = !save.IsCompleted ? "Saving photo..."
                : save.IsCompletedSuccessfully && save.Result ? $"Saved: {SelfiePhoto.FolderName}"
                : "Couldn't save the photo.";
            overlay.DrawSmallText(batch, status, x + 4, y + PreviewHeight + 6, true);
        }
    }
}
