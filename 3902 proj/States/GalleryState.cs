using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TransformersGame.Commands;
using TransformersGame.Controllers;
using TransformersGame.Interfaces;
using TransformersGame.UI;

namespace TransformersGame.States
{
    // Owns the gallery's input and rendering while Game1 controls transitions.
    internal sealed class GalleryState : IGameState, IDisposable
    {
        private readonly SelfieGalleryScreen screen;
        private readonly GraphicsDevice graphics;
        private readonly Action close;
        private readonly Action toggleMute;
        private IController controller;
        private bool active;
        private readonly Color background;

        public GalleryState(GraphicsDevice graphics, SpriteFont font, Action close, Action toggleMute, Color background)
        {
            this.graphics = graphics;
            this.close = close;
            this.toggleMute = toggleMute;
            this.background = background;
            screen = new SelfieGalleryScreen(graphics, font);
        }

        public void Open()
        {
            screen.Open();
            KeyboardController keyboard = new KeyboardController();
            keyboard.RegisterSinglePressCommand(Keys.F3, new GameActionCommand(Close));
            keyboard.RegisterSinglePressCommand(Keys.Escape, new GameActionCommand(CloseViewerOrGallery));
            keyboard.RegisterSinglePressCommand(Keys.Left, new GameActionCommand(screen.Previous));
            keyboard.RegisterSinglePressCommand(Keys.Right, new GameActionCommand(screen.Next));
            keyboard.RegisterSinglePressCommand(Keys.M, new GameActionCommand(toggleMute));
            controller = keyboard;
            active = true;
        }

        public void Update(GameTime gameTime)
        {
            controller.Update();
            if (active) screen.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            graphics.Clear(background);
            screen.Draw(spriteBatch);
        }

        public void Dispose() => screen.Dispose();

        private void Close()
        {
            active = false;
            close();
        }

        private void CloseViewerOrGallery()
        {
            if (!screen.CloseViewer()) Close();
        }
    }
}
