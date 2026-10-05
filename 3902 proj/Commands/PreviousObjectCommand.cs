using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Commands
{
    public class PreviousObjectCommand : ICommand
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
