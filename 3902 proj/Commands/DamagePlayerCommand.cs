using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Commands
{
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
}
