using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Commands
{
    internal class UseInventoryItemCommand : ICommand
    {
        private readonly IPlayer player;
        private readonly ItemKind itemKind;

        public UseInventoryItemCommand(IPlayer player, ItemKind itemKind)
        {
            this.player = player;
            this.itemKind = itemKind;
        }

        public void Execute()
        {
            if (itemKind == ItemKind.Medkit)
            {
                player.UseMedkit();
            }
            else if (itemKind == ItemKind.Shield)
            {
                player.UseShield();
            }
        }
    }
}
