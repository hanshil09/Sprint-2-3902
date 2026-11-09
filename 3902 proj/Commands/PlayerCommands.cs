using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Commands
{
    internal class MovePlayerCommand : ICommand
    {
        private IPlayer player;
        private Direction direction;

        public MovePlayerCommand(IPlayer player, Direction direction)
        {
            this.player = player;
            this.direction = direction;
        }

        public void Execute()
        {
            player.Move(direction);
        }
    }

    internal class JumpPlayerCommand : ICommand
    {
        private IPlayer player;

        public JumpPlayerCommand(IPlayer player)
        {
            this.player = player;
        }

        public void Execute()
        {
            player.Jump();
        }
    }

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

    internal class TransformPlayerCommand : ICommand
    {
        private IPlayer player;

        public TransformPlayerCommand(IPlayer player)
        {
            this.player = player;
        }

        public void Execute()
        {
            player.Transform();
        }
    }
}
