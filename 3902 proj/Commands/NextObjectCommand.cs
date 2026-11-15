using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Commands
{
    internal class NextObjectCommand : ICommand
    {
        private readonly IGameObjectCycler cycler;

        public NextObjectCommand(IGameObjectCycler cycler)
        {
            this.cycler = cycler;
        }

        public void Execute()
        {
            cycler.NextObject();
        }
    }
}
