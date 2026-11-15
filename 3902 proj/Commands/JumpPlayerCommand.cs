using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Commands
{
    internal class JumpPlayerCommand : ICommand
    {
        private IPlayer player;

        public JumpPlayerCommand(IPlayer player)
        {
            this.player = player;
        }

        public void Execute()
        {
            player.Jump();
        }
    }
}
