using TransformersGame.Interfaces;

namespace TransformersGame.Commands
{
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
}
