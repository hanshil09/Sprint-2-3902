using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Commands
{
    public class PreviousObjectCommand : ICommand
    {
        private GameObjectCycler cycler;

        public PreviousObjectCommand(GameObjectCycler cycler)
        {
            this.cycler = cycler;
        }

        public void Execute()
        {
            cycler.Previous();
        }
    }
}
