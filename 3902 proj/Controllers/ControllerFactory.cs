using Microsoft.Xna.Framework.Input;
using TransformersGame.Commands;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Controllers
{
    public static class ControllerFactory
    {
        public static IController CreateMenuController(Game1 game)
        {
            KeyboardController keyboardController = new KeyboardController();
            keyboardController.RegisterSinglePressCommand(Keys.Enter, new StartGameCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.Q, new QuitCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.Escape, new QuitCommand(game));
            return keyboardController;
        }

        public static IController CreateGameplayController(Game1 game, IPlayer player, IGameObjectCycler blocks, IGameObjectCycler items, IGameObjectCycler enemies)
        {
            KeyboardController keyboardController = new KeyboardController();
            RegisterMovementCommands(keyboardController, player);
            RegisterActionCommands(keyboardController, player);
            RegisterCycleCommands(keyboardController, blocks, items, enemies);
            keyboardController.RegisterSinglePressCommand(Keys.E, new DamagePlayerCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.R, new ResetGameCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.Q, new QuitCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.Escape, new QuitCommand(game));
            return keyboardController;
        }

        private static void RegisterMovementCommands(KeyboardController keyboardController, IPlayer player)
        {
            keyboardController.RegisterCommand(Keys.A, new MovePlayerCommand(player, Direction.Left));
            keyboardController.RegisterCommand(Keys.Left, new MovePlayerCommand(player, Direction.Left));
            keyboardController.RegisterCommand(Keys.D, new MovePlayerCommand(player, Direction.Right));
            keyboardController.RegisterCommand(Keys.Right, new MovePlayerCommand(player, Direction.Right));
            keyboardController.RegisterCommand(Keys.S, new MovePlayerCommand(player, Direction.Down));
            keyboardController.RegisterCommand(Keys.Down, new MovePlayerCommand(player, Direction.Down));
            keyboardController.RegisterSinglePressCommand(Keys.W, new JumpPlayerCommand(player));
            keyboardController.RegisterSinglePressCommand(Keys.Up, new JumpPlayerCommand(player));
            keyboardController.RegisterSinglePressCommand(Keys.J, new JumpPlayerCommand(player));
        }

        private static void RegisterActionCommands(KeyboardController keyboardController, IPlayer player)
        {
            keyboardController.RegisterSinglePressCommand(Keys.Space, new TransformPlayerCommand(player));
            keyboardController.RegisterSinglePressCommand(Keys.Z, new ShootPlayerCommand(player));
            keyboardController.RegisterSinglePressCommand(Keys.N, new ShootPlayerCommand(player));
            keyboardController.RegisterSinglePressCommand(Keys.D1, new UseItemCommand(player, 1));
            keyboardController.RegisterSinglePressCommand(Keys.NumPad1, new UseItemCommand(player, 1));
            keyboardController.RegisterSinglePressCommand(Keys.D2, new UseItemCommand(player, 2));
            keyboardController.RegisterSinglePressCommand(Keys.NumPad2, new UseItemCommand(player, 2));
        }

        private static void RegisterCycleCommands(KeyboardController keyboardController, IGameObjectCycler blocks, IGameObjectCycler items, IGameObjectCycler enemies)
        {
            keyboardController.RegisterSinglePressCommand(Keys.T, new PreviousObjectCommand(blocks));
            keyboardController.RegisterSinglePressCommand(Keys.Y, new NextObjectCommand(blocks));
            keyboardController.RegisterSinglePressCommand(Keys.U, new PreviousObjectCommand(items));
            keyboardController.RegisterSinglePressCommand(Keys.I, new NextObjectCommand(items));
            keyboardController.RegisterSinglePressCommand(Keys.O, new PreviousObjectCommand(enemies));
            keyboardController.RegisterSinglePressCommand(Keys.P, new NextObjectCommand(enemies));
        }
    }
}
