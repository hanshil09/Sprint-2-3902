using Microsoft.Xna.Framework;
using ArmadilloGame.Entities;
using ArmadilloGame.Factories;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.States
{
    internal class VehicleState : IPlayerState
    {
        private const float VehicleSpeed = 280f;

        private readonly Player player;

        public VehicleState(Player player)
        {
            this.player = player;
        }

        public float Speed => VehicleSpeed;

        public void Transform()
        {
            player.SetState(new RobotState(player));
        }

        public void Shoot(bool angled)
        {
        }

        public void UseItem(int itemNumber, bool angled = false)
        {
            if (itemNumber == 2)
            {
                player.DropBomb();
            }
        }

        public void Update(GameTime gameTime)
        {
        }

        public ISprite CreateSprite()
        {
            if (player.IsMoving)
            {
                return PlayerSpriteFactory.Instance.CreateRollingBallSprite(player.FacingLeft);
            }
            return PlayerSpriteFactory.Instance.CreateBallSprite(player.FacingLeft);
        }
    }
}
