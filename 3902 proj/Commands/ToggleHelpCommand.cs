using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Commands
{
    internal sealed class ToggleHelpCommand : ICommand
    {
        private readonly Game1 game;
        public ToggleHelpCommand(Game1 game) => this.game = game;
        public void Execute() => game.ToggleHelp();
    }
}
