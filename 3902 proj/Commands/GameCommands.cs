using TransformersGame.Interfaces;

namespace TransformersGame.Commands
{
    public class StartGameCommand : ICommand
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

    public class ResetGameCommand : ICommand
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

    public class QuitCommand : ICommand
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

    public class DamagePlayerCommand : ICommand
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
}
