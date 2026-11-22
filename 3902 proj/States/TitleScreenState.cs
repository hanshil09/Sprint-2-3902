
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ArmadilloGame.Commands;
using ArmadilloGame.Controllers;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.States
{
    // display menu art
    internal sealed class TitleScreenState : IGameState, IDisposable
    {
        private readonly Texture2D background;
        private readonly GraphicsDevice graphics;
        private readonly Action continueToMenu;
        private readonly IController controller;
        private readonly SpriteFont font;
        private readonly Texture2D pixel;
        private Rectangle startButton;
        private bool isHovering;
        private MouseState previousMouseState;

        public TitleScreenState(
            Texture2D background,
            GraphicsDevice graphics,
            SpriteFont font,
            Action continueToMenu)
        {
            this.background = background;
            this.graphics = graphics;
            this.font = font;
            this.continueToMenu = continueToMenu;

            pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData(new[] { Color.White });

            startButton = new Rectangle(
                graphics.Viewport.Width / 2 - 120,
                graphics.Viewport.Height - 100,
                240,
                60);

            KeyboardController keyboard = new KeyboardController();
            keyboard.RegisterSinglePressCommand(
                Keys.Enter,
                new GameActionCommand(continueToMenu));

            controller = keyboard;
        }

        public void Update(GameTime gameTime)
        {
            controller.Update();

            MouseState mouse = Mouse.GetState();

            isHovering = startButton.Contains(mouse.Position);

            if (isHovering &&
                mouse.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released)
            {
                continueToMenu();
            }

            previousMouseState = mouse;
        }

        public void Draw(SpriteBatch batch)
        {
            graphics.Clear(Color.Black);

            float scale = Math.Max(
                (float)graphics.Viewport.Width / background.Width,
                (float)graphics.Viewport.Height / background.Height);

            int width = (int)(background.Width * scale);
            int height = (int)(background.Height * scale);

            Rectangle destination = new Rectangle(
                (graphics.Viewport.Width - width) / 2,
                (graphics.Viewport.Height - height) / 2,
                width,
                height);

            batch.Begin(samplerState: SamplerState.LinearClamp);
            batch.Draw(background, destination, Color.White);

            Color buttonBackground = isHovering ? Color.White : Color.Black;
            Color buttonText = isHovering ? Color.Black : Color.White;
            Color buttonBorder = isHovering ? Color.Black : Color.White;

            int borderThickness = 2;

            Rectangle top = new Rectangle(
                startButton.X,
                startButton.Y,
                startButton.Width,
                borderThickness);

            Rectangle bottom = new Rectangle(
                startButton.X,
                startButton.Bottom - borderThickness,
                startButton.Width,
                borderThickness);

            Rectangle left = new Rectangle(
                startButton.X,
                startButton.Y,
                borderThickness,
                startButton.Height);

            Rectangle right = new Rectangle(
                startButton.Right - borderThickness,
                startButton.Y,
                borderThickness,
                startButton.Height);

            batch.Draw(pixel, startButton, buttonBackground);

            batch.Draw(pixel, top, buttonBorder);
            batch.Draw(pixel, bottom, buttonBorder);
            batch.Draw(pixel, left, buttonBorder);
            batch.Draw(pixel, right, buttonBorder);

            string text = "START GAME";
            float textScale = 0.5f;

            Vector2 textSize = font.MeasureString(text) * textScale;

            Vector2 textPosition = new Vector2(
                startButton.Center.X - textSize.X / 2,
                startButton.Center.Y - textSize.Y / 2);

            batch.DrawString(
                font,
                text,
                textPosition,
                buttonText,
                0f,
                Vector2.Zero,
                textScale,
                SpriteEffects.None,
                0f);
            batch.End();
        }
        public void Dispose()
        {
            pixel.Dispose();
        }
    }

}
