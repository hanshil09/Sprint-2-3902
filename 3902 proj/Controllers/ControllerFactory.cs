using Microsoft.Xna.Framework.Input;
using ArmadilloGame.Commands;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Controllers
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
            RegisterWeaponCommands(keyboardController, game);
            keyboardController.RegisterSinglePressCommand(Keys.F1, new ToggleHelpCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.H, new ToggleHelpCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.R, new ResetGameCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.Q, new QuitCommand(game));
            keyboardController.RegisterSinglePressCommand(Keys.Escape, new QuitCommand(game));
            // ` (backtick/tilde) toggles the dev-only keys so they do not clutter normal play.
            keyboardController.RegisterSinglePressCommand(Keys.OemTilde, new GameActionCommand(game.ToggleDevMode));

            // Left-click fires the equipped weapon. Right-click is unused for now.
            MouseController mouseController = new MouseController(
                new GameActionCommand(game.FireSelectedShot),
                new GameActionCommand(() => { }));

            return new CompositeController(keyboardController, mouseController);
        }

        // Dev-only shortcuts (object cycling, room skipping, self-damage), gated behind dev mode.
        public static IController CreateDevController(Game1 game, IGameObjectCycler blocks, IGameObjectCycler items, IGameObjectCycler enemies)
        {
            KeyboardController keyboard = new KeyboardController();
            RegisterCycleCommands(keyboard, blocks, items, enemies);
            keyboard.RegisterSinglePressCommand(Keys.PageUp, new RoomNavigationCommand(game, false));
            keyboard.RegisterSinglePressCommand(Keys.PageDown, new RoomNavigationCommand(game, true));
            keyboard.RegisterSinglePressCommand(Keys.E, new DamagePlayerCommand(game));
            return keyboard;
        }

        private static void RegisterMovementCommands(KeyboardController keyboardController, IPlayer player)
        {
            keyboardController.RegisterCommand(new MovePlayerCommand(player, Direction.Left), Keys.A, Keys.Left);
            keyboardController.RegisterCommand(new MovePlayerCommand(player, Direction.Right), Keys.D, Keys.Right);
            keyboardController.RegisterCommand(new MovePlayerCommand(player, Direction.Down), Keys.S, Keys.Down);
            keyboardController.RegisterSinglePressCommand(new JumpPlayerCommand(player), Keys.W, Keys.Up, Keys.Space);
        }

        // Shift transforms to/from ball form; 1-4 equip weapons; C/V use medkit and shield.
        private static void RegisterWeaponCommands(KeyboardController keyboardController, Game1 game)
        {
            keyboardController.RegisterSinglePressCommand(new TransformPlayerCommand(game.Player), Keys.LeftShift, Keys.RightShift);
            keyboardController.RegisterSinglePressCommand(new GameActionCommand(() => game.SelectWeapon(0)), Keys.D1, Keys.NumPad1);
            keyboardController.RegisterSinglePressCommand(new GameActionCommand(() => game.SelectWeapon(1)), Keys.D2, Keys.NumPad2);
            keyboardController.RegisterSinglePressCommand(new GameActionCommand(() => game.SelectWeapon(2)), Keys.D3, Keys.NumPad3);
            keyboardController.RegisterSinglePressCommand(new GameActionCommand(() => game.SelectWeapon(3)), Keys.D4, Keys.NumPad4);
            keyboardController.RegisterSinglePressCommand(new UseInventoryItemCommand(game.Player, ItemKind.Medkit), Keys.C);
            keyboardController.RegisterSinglePressCommand(new UseInventoryItemCommand(game.Player, ItemKind.Shield), Keys.V);
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
