using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Commands
{
    public class NextObjectCommand : ICommand
    {
        private readonly IGameObjectCycler cycler;

        public NextObjectCommand(IGameObjectCycler cycler)
        {
            this.cycler = cycler;
        }

        public void Execute()
        {
            cycler.Next();
        }
    }
}
