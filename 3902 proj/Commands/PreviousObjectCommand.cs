using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Commands
{
    internal class PreviousObjectCommand : ICommand
    {
        private readonly IGameObjectCycler cycler;

        public PreviousObjectCommand(IGameObjectCycler cycler)
        {
            this.cycler = cycler;
        }

        public void Execute()
        {
            cycler.Previous();
        }
    }
}
