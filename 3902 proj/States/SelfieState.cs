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
    internal sealed class SelfieState : IGameState, IDisposable
    {
        private readonly SelfieScreen screen;
        private readonly Action close;
        private readonly Action toggleMute;
        private readonly Action quit;
        private readonly Action shutter;
        private IController controller;
        private bool active;
        private readonly Func<Room> room;
        private readonly Color background;

        public SelfieState(SelfieScreen screen, Action close, Action toggleMute, Action quit, Action shutter,
            Func<Room> room, Color background)
        {
            this.screen = screen;
            this.close = close;
            this.toggleMute = toggleMute;
            this.quit = quit;
            this.shutter = shutter;
            this.room = room;
            this.background = background;
        }

        public bool Open(Room room, IPlayer player, Matrix camera)
        {
            if (!screen.Start(room, player, camera)) return false;
            KeyboardController keyboard = new KeyboardController();
            keyboard.RegisterSinglePressCommand(Keys.Escape, new GameActionCommand(Close));
            keyboard.RegisterSinglePressCommand(Keys.M, new GameActionCommand(toggleMute));
            keyboard.RegisterSinglePressCommand(Keys.Q, new GameActionCommand(quit));
            controller = keyboard;
            active = true;
            return true;
        }

        public void Update(GameTime gameTime)
        {
            controller.Update();
            if (!active) return;
            screen.Update(gameTime);
            if (screen.Finished) Close();
        }

        public void Draw(SpriteBatch batch)
        {
            if (screen.Draw(batch, room(), background)) shutter();
        }

        public void Dispose() => screen.Dispose();

        private void Close()
        {
            active = false;
            close();
        }
    }
}
