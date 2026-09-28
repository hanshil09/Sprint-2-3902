using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.States
{
    public class RobotState : IPlayerState
    {
        public const float RobotSpeed = 180f;

        private Player player;

        public RobotState(Player player)
        {
            this.player = player;
        }

        public float Speed
        {
            get
            {
                return RobotSpeed;
            }
        }

        public void Transform()
        {
            player.SetState(new VehicleState(player));
        }

        public void Shoot()
        {
            player.FireShot();
            player.SetState(new ShootingRobotState(player));
        }

        public void UseItem(int itemNumber)
        {
            if (itemNumber == 1)
            {
                player.FireOrb();
                player.SetState(new ShootingRobotState(player));
            }
            else if (itemNumber == 2)
            {
                player.DropBomb();
            }
        }

        public void Update(GameTime gameTime)
        {
        }

        public ISprite CreateSprite()
        {
            if (!player.IsOnGround)
            {
                return PlayerSpriteFactory.Instance.CreateJumpingRobotSprite(player.FacingLeft);
            }
            if (player.IsMoving)
            {
                return PlayerSpriteFactory.Instance.CreateRunningRobotSprite(player.FacingLeft);
            }
            return PlayerSpriteFactory.Instance.CreateStandingRobotSprite(player.Facing);
        }
    }
}
