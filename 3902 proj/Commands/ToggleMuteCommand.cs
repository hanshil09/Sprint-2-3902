using TransformersGame.Interfaces;

namespace TransformersGame.Commands
{
    internal sealed class ToggleMuteCommand : ICommand
    {
        private readonly Game1 game;
        public ToggleMuteCommand(Game1 game) => this.game = game;
        public void Execute() => game.ToggleMute();
    }
}
