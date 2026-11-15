using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Commands
{
    internal class TransformPlayerCommand : ICommand
    {
        private IPlayer player;

        public TransformPlayerCommand(IPlayer player)
        {
            this.player = player;
        }

        public void Execute()
        {
            player.Transform();
        }
    }
}
