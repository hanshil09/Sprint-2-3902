using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Commands
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
