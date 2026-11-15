using TransformersGame.Interfaces;

namespace TransformersGame.Commands
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
