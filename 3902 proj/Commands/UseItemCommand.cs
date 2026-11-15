using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Commands
{
    internal class UseItemCommand : ICommand
    {
        private IPlayer player;
        private int itemNumber;

        public UseItemCommand(IPlayer player, int itemNumber)
        {
            this.player = player;
            this.itemNumber = itemNumber;
        }

        public void Execute()
        {
            player.UseItem(itemNumber);
        }
    }
}
