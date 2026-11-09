using Microsoft.Xna.Framework.Input;
using TransformersGame.Commands;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Controllers
{
    internal static class ControllerFactory
    {
        public static IController CreateMenuController(Game1 game)
        {
            KeyboardController keyboardController = new KeyboardController();
            keyboardController.RegisterSinglePressCommand(Keys.Enter, new StartGameCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.M, new ToggleMusicCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.Q, new QuitCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.Escape, new QuitCommand(game));
            return keyboardController;
        }

        // Game over and mission complete screens: reset or quit.
        public static IController CreateEndScreenController(Game1 game)
        {
            KeyboardController keyboardController = new KeyboardController();
            keyboardController.RegisterSinglePressCommand(Keys.M, new ToggleMusicCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.R, new ResetGameCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.Q, new QuitCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.Escape, new QuitCommand(game));
            return keyboardController;
        }

        public static IController CreateMapController(Game1 game)
        {
            KeyboardController keyboard = new KeyboardController();
            keyboard.RegisterSinglePressCommand(Keys.Tab, new MapCommand(game.CloseMap));
            keyboard.RegisterSinglePressCommand(Keys.Escape, new MapCommand(game.CloseMap));
            keyboard.RegisterSinglePressCommand(Keys.M, new ToggleMusicCommand(game));
            return keyboard;
        }

        public static IController CreateGameplayController(Game1 game, IPlayer player, IGameObjectCycler blocks, IGameObjectCycler items, IGameObjectCycler enemies)
        {
            KeyboardController keyboardController = new KeyboardController();
            keyboardController.RegisterSinglePressCommand(Keys.Tab, new MapCommand(game.OpenMap));
            RegisterMovementCommands(keyboardController, player);
            keyboardController.RegisterSinglePressCommand(Keys.M, new ToggleMusicCommand(game));
            RegisterActionCommands(keyboardController, player);
            RegisterCycleCommands(keyboardController, blocks, items, enemies);
            keyboardController.RegisterSinglePressCommand(Keys.F1, new ToggleHelpCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.H, new ToggleHelpCommand(game));
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
            keyboardController.RegisterSinglePressCommand(new ShootPlayerCommand(player), Keys.Z, Keys.N);
            keyboardController.RegisterSinglePressCommand(new AngledShootPlayerCommand(player), Keys.X, Keys.K);
            keyboardController.RegisterSinglePressCommand(new UseItemCommand(player, 1), Keys.D1, Keys.NumPad1);
            keyboardController.RegisterSinglePressCommand(new UseItemCommand(player, 2), Keys.D2, Keys.NumPad2);
            keyboardController.RegisterSinglePressCommand(new UseInventoryItemCommand(player, ItemKind.Medkit), Keys.D3, Keys.NumPad3);
            keyboardController.RegisterSinglePressCommand(new UseInventoryItemCommand(player, ItemKind.Shield), Keys.D4, Keys.NumPad4);
            keyboardController.RegisterSinglePressCommand(new UseItemCommand(player, 5), Keys.D5, Keys.NumPad5);
            keyboardController.RegisterSinglePressCommand(new UseItemCommand(player, 6), Keys.D6, Keys.NumPad6);
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
