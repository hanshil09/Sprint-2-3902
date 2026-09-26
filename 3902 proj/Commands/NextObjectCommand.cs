using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Commands
{
    public class NextObjectCommand : ICommand
    {
        private GameObjectCycler cycler;

        public NextObjectCommand(GameObjectCycler cycler)
        {
            this.cycler = cycler;
        }

        public void Execute()
        {
            cycler.Next();
        }
    }
}
