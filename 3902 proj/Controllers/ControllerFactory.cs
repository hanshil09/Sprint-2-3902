using Microsoft.Xna.Framework.Input;
using TransformersGame.Commands;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Controllers
{
    internal static class ControllerFactory
    {
        public static IController CreateSelfieShortcut(Game1 game)
        {
            KeyboardController keyboard = new KeyboardController();
            keyboard.RegisterSinglePressCommand(Keys.F2, new GameActionCommand(game.OpenSelfie));
            keyboard.RegisterSinglePressCommand(Keys.F3, new GameActionCommand(game.OpenGallery));
            return keyboard;
        }

        public static IController CreateGameplayController(Game1 game, IPlayer player, IGameObjectCycler blocks, IGameObjectCycler items, IGameObjectCycler enemies)
        {
            KeyboardController keyboardController = new KeyboardController();
            keyboardController.RegisterSinglePressCommand(Keys.Tab, new GameActionCommand(game.OpenMap));
            RegisterMovementCommands(keyboardController, player);
            keyboardController.RegisterSinglePressCommand(Keys.M, new ToggleMuteCommand(game));
            RegisterActionCommands(keyboardController, player);
            keyboardController.RegisterSinglePressCommand(Keys.X, new GameActionCommand(game.FireSelectedShot));
            RegisterCycleCommands(keyboardController, blocks, items, enemies);
            keyboardController.RegisterSinglePressCommand(Keys.F1, new ToggleHelpCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.H, new ToggleHelpCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.B, new BarrelStormCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.PageUp, new RoomNavigationCommand(game, false));
            keyboardController.RegisterSinglePressCommand(Keys.PageDown, new RoomNavigationCommand(game, true));
            keyboardController.RegisterSinglePressCommand(Keys.E, new DamagePlayerCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.R, new ResetGameCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.Q, new QuitCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.Escape, new QuitCommand(game));

            MouseController mouseController = new MouseController(
                new RoomNavigationCommand(game, false),
                new RoomNavigationCommand(game, true));

            return new CompositeController(keyboardController, mouseController);
        }

        private static void RegisterMovementCommands(KeyboardController keyboardController, IPlayer player)
        {
            keyboardController.RegisterCommand(new MovePlayerCommand(player, Direction.Left), Keys.A, Keys.Left);
            keyboardController.RegisterCommand(new MovePlayerCommand(player, Direction.Right), Keys.D, Keys.Right);
            keyboardController.RegisterCommand(new MovePlayerCommand(player, Direction.Down), Keys.S, Keys.Down);
            keyboardController.RegisterSinglePressCommand(new JumpPlayerCommand(player), Keys.W, Keys.Up, Keys.J);
        }

        private static void RegisterActionCommands(KeyboardController keyboardController, IPlayer player)
        {
            keyboardController.RegisterSinglePressCommand(new TransformPlayerCommand(player), Keys.Space);
            keyboardController.RegisterSinglePressCommand(new UseItemCommand(player, 2), Keys.D2, Keys.NumPad2);
            keyboardController.RegisterSinglePressCommand(new UseInventoryItemCommand(player, ItemKind.Medkit), Keys.D3, Keys.NumPad3);
            keyboardController.RegisterSinglePressCommand(new UseInventoryItemCommand(player, ItemKind.Shield), Keys.D4, Keys.NumPad4);
            keyboardController.RegisterSinglePressCommand(new UseItemCommand(player, 7), Keys.D7, Keys.NumPad7);
        }

        private static void RegisterCycleCommands(KeyboardController keyboardController, IGameObjectCycler blocks, IGameObjectCycler items, IGameObjectCycler enemies)
        {
            keyboardController.RegisterSinglePressCommand(new PreviousObjectCommand(blocks), Keys.T);
            keyboardController.RegisterSinglePressCommand(new NextObjectCommand(blocks), Keys.Y);
            keyboardController.RegisterSinglePressCommand(new PreviousObjectCommand(items), Keys.U);
            keyboardController.RegisterSinglePressCommand(new NextObjectCommand(items), Keys.I);
            keyboardController.RegisterSinglePressCommand(new PreviousObjectCommand(enemies), Keys.O);
            keyboardController.RegisterSinglePressCommand(new NextObjectCommand(enemies), Keys.P);
        }
    }
}
