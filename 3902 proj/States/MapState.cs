using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ArmadilloGame.Commands;
using ArmadilloGame.Controllers;
using ArmadilloGame.Core;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;
using ArmadilloGame.UI;

namespace ArmadilloGame.States
{
    internal sealed class MapState : IGameState
    {
        private readonly RoomMapScreen screen;
        private readonly GraphicsDevice graphics;
        private readonly RoomFactory rooms;
        private readonly Action close;
        private readonly Action toggleMute;
        private IController controller;
        private bool active;
        private readonly Func<RoomManager> roomManager;
        private readonly Func<IPlayer> player;
        private readonly Color background;
        private readonly Color roomColor;

        public MapState(GraphicsDevice graphics, SpriteFont font, WeaponSelection weapons,
            RoomFactory rooms, Action close, Action toggleMute, Func<RoomManager> roomManager,
            Func<IPlayer> player, Color background, Color roomColor)
        {
            this.graphics = graphics;
            this.rooms = rooms;
            this.close = close;
            this.toggleMute = toggleMute;
            this.roomManager = roomManager;
            this.player = player;
            this.background = background;
            this.roomColor = roomColor;
            screen = new RoomMapScreen(font, weapons);
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

        public void Update(GameTime gameTime)
        {
            controller.Update();
            if (active) screen.UpdateNavigation(rooms, player());
        }

        public void Draw(SpriteBatch batch)
        {
            graphics.Clear(background);
            screen.Draw(batch, roomManager(), player(), roomColor);
        }

        private void Close()
        {
            active = false;
            close();
        }
    }
}
