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
    internal sealed class MenuState : IGameState
    {
        private readonly ControlsOverlay overlay;
        private readonly GraphicsDevice graphics;
        private readonly Action start;
        private readonly Action reset;
        private readonly Action mute;
        private readonly Action quit;
        private IController controller;
        private readonly Func<GameState> state;
        private readonly Func<IPlayer> player;
        private readonly Color background;

        public MenuState(ControlsOverlay overlay, GraphicsDevice graphics,
            Action start, Action reset, Action mute, Action quit,
            Func<GameState> state, Func<IPlayer> player, Color background)
        {
            this.overlay = overlay;
            this.graphics = graphics;
            this.start = start;
            this.reset = reset;
            this.mute = mute;
            this.quit = quit;
            this.state = state;
            this.player = player;
            this.background = background;
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

        public void Update(GameTime gameTime) => controller.Update();

        public void Draw(SpriteBatch batch)
        {
            graphics.Clear(background);
            batch.Begin(samplerState: SamplerState.PointClamp);
            overlay.DrawMenu(batch, graphics.Viewport, state(), player());
            batch.End();
        }
    }
}
