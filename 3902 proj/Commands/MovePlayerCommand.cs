using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Commands
{
    internal class MovePlayerCommand : ICommand
    {
        private IPlayer player;
        private Direction direction;

        public MovePlayerCommand(IPlayer player, Direction direction)
        {
            this.player = player;
            this.direction = direction;
        }

        public void Execute()
        {
            player.Move(direction);
        }
    }
}
