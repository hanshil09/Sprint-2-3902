using TransformersGame.Interfaces;

namespace TransformersGame.Commands
{
    internal class RoomNavigationCommand : ICommand
    {
        private readonly Game1 game;
        private readonly bool next;

        public RoomNavigationCommand(Game1 game, bool next)
        {
            this.game = game;
            this.next = next;
        }

        public void Execute()
        {
            if (next)
            {
                game.NextRoom();
            }
            else
            {
                game.PreviousRoom();
            }
        }
    }
}
