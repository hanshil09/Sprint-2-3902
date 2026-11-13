using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TransformersGame.Commands;
using TransformersGame.Controllers;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;
using TransformersGame.UI;

namespace TransformersGame.States
{
    internal sealed class MapState
    {
        private readonly RoomMapScreen screen;
        private readonly GraphicsDevice graphics;
        private readonly RoomFactory rooms;
        private readonly Action close;
        private readonly Action toggleMute;
        private IController controller;
        private bool active;

        public MapState(GraphicsDevice graphics, SpriteFont font, FireballSelection fireballs,
            RoomFactory rooms, Action close, Action toggleMute)
        {
            this.graphics = graphics;
            this.rooms = rooms;
            this.close = close;
            this.toggleMute = toggleMute;
            screen = new RoomMapScreen(font, fireballs);
        }

        public void Open(int roomIndex)
        {
            screen.Open(roomIndex);
            KeyboardController keyboard = new KeyboardController();
            keyboard.RegisterSinglePressCommand(new GameActionCommand(Close), Keys.Tab, Keys.Escape);
            keyboard.RegisterSinglePressCommand(Keys.M, new GameActionCommand(toggleMute));
            controller = keyboard;
            active = true;
        }

        public void Update()
        {
            controller.Update();
            if (active) screen.UpdateNavigation(rooms);
        }

        public void Draw(SpriteBatch batch, RoomManager roomManager, IPlayer player, Color background, Color roomColor)
        {
            graphics.Clear(background);
            screen.Draw(batch, roomManager, player, roomColor);
        }

        private void Close()
        {
            active = false;
            close();
        }
    }
}
