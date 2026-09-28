using TransformersGame.Interfaces;

namespace TransformersGame.Commands
{
    public class StartGameCommand : ICommand
    {
        private Game1 myGame;

        public StartGameCommand(Game1 game)
        {
            myGame = game;
        }

        public void Execute()
        {
            myGame.StartGame();
        }
    }

    public class ResetGameCommand : ICommand
    {
        private Game1 myGame;

        public ResetGameCommand(Game1 game)
        {
            myGame = game;
        }

        public void Execute()
        {
            myGame.ResetGame();
        }
    }

    public class QuitCommand : ICommand
    {
        private Game1 myGame;

        public QuitCommand(Game1 game)
        {
            myGame = game;
        }

        public void Execute()
        {
            myGame.Exit();
        }
    }

    public class DamagePlayerCommand : ICommand
    {
        private Game1 myGame;

        public DamagePlayerCommand(Game1 game)
        {
            myGame = game;
        }

        public void Execute()
        {
            myGame.DamagePlayer();
        }
    }
}
