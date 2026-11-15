using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Commands
{
    internal sealed class BarrelStormCommand : ICommand
    {
        private readonly Game1 game;
        public BarrelStormCommand(Game1 game) => this.game = game;
        public void Execute() => game.ActivateBarrelStorm();
    }
}
