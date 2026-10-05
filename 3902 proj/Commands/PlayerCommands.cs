using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Commands
{
    public class MovePlayerCommand : ICommand
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

    public class JumpPlayerCommand : ICommand
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

    public class ShootPlayerCommand : ICommand
    {
        private IPlayer player;
        private bool angled;

        public ShootPlayerCommand(IPlayer player)
        {
            this.player = player;
            angled = false;
        }

        public void Execute()
        {
            player.Shoot(angled);
        }
    }

    public class AngledShootPlayerCommand : ICommand
    {
        private IPlayer player;
        private bool angled;

        public AngledShootPlayerCommand(IPlayer player)
        {
            this.player = player;
            angled = true;
        }

        public void Execute()
        {
            player.Shoot(angled);
        }
    }

    public class UseItemCommand : ICommand
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

    public class TransformPlayerCommand : ICommand
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
