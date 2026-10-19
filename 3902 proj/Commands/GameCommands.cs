using TransformersGame.Interfaces;

namespace TransformersGame.Commands
{
    internal sealed class ToggleHelpCommand : ICommand
    {
        private readonly Game1 game;
        public ToggleHelpCommand(Game1 game) => this.game = game;
        public void Execute() => game.ToggleHelp();
    }

    internal class StartGameCommand : ICommand
    {
        private readonly Game1 game;

        public StartGameCommand(Game1 game)
        {
            this.game = game;
        }

        public void Execute()
        {
            game.StartGame();
        }
    }

    internal class ResetGameCommand : ICommand
    {
        private readonly Game1 game;

        public ResetGameCommand(Game1 game)
        {
            this.game = game;
        }

        public void Execute()
        {
            game.ResetGame();
        }
    }

    internal class QuitCommand : ICommand
    {
        private readonly Game1 game;

        public QuitCommand(Game1 game)
        {
            this.game = game;
        }

        public void Execute()
        {
            game.Exit();
        }
    }

    internal class DamagePlayerCommand : ICommand
    {
        private readonly Game1 game;

        public DamagePlayerCommand(Game1 game)
        {
            this.game = game;
        }

        public void Execute()
        {
            game.DamagePlayer();
        }
    }

    internal class RoomNavigationCommand : ICommand
    {
        private readonly Game1 game;
        private readonly bool next;

        public RoomNavigationCommand(Game1 game, bool next)
        {
            this.game = game;
            this.next = next;
        }

        public void Execute()
        {
            if (next)
            {
                game.NextRoom();
            }
            else
            {
                game.PreviousRoom();
            }
        }
    }
}
