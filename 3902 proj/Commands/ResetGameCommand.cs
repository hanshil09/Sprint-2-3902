using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Commands
{
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
}
