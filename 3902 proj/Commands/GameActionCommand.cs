using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Commands
{
    internal sealed class GameActionCommand : ICommand
    {
        private readonly System.Action action;

        public GameActionCommand(System.Action action) => this.action = action;
        public void Execute() => action();
    }
}
