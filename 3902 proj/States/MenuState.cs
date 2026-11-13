using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TransformersGame.Commands;
using TransformersGame.Controllers;
using TransformersGame.Core;
using TransformersGame.Interfaces;
using TransformersGame.UI;

namespace TransformersGame.States
{
    // Handles the start, game-over and victory screens.
    internal sealed class MenuState
    {
        private readonly ControlsOverlay overlay;
        private readonly GraphicsDevice graphics;
        private readonly Action start;
        private readonly Action reset;
        private readonly Action mute;
        private readonly Action quit;
        private IController controller;

        public MenuState(ControlsOverlay overlay, GraphicsDevice graphics,
            Action start, Action reset, Action mute, Action quit)
        {
            this.overlay = overlay;
            this.graphics = graphics;
            this.start = start;
            this.reset = reset;
            this.mute = mute;
            this.quit = quit;
        }

        public void Open(bool isEndScreen)
        {
            KeyboardController keyboard = new KeyboardController();
            if (isEndScreen)
                keyboard.RegisterSinglePressCommand(Keys.R, new GameActionCommand(reset));
            else
                keyboard.RegisterSinglePressCommand(Keys.Enter, new GameActionCommand(start));
            keyboard.RegisterSinglePressCommand(Keys.M, new GameActionCommand(mute));
            keyboard.RegisterSinglePressCommand(new GameActionCommand(quit), Keys.Q, Keys.Escape);
            controller = keyboard;
        }

        public void Update() => controller.Update();

        public void Draw(SpriteBatch batch, GameState state, IPlayer player, Color background)
        {
            graphics.Clear(background);
            batch.Begin(samplerState: SamplerState.PointClamp);
            overlay.DrawMenu(batch, graphics.Viewport, state, player);
            batch.End();
        }
    }
}
