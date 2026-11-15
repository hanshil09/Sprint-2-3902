using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Commands
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
