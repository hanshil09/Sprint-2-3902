using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.States
{
    public class VehicleState : IPlayerState
    {
        private const float VehicleSpeed = 280f;

        private Player player;

        public VehicleState(Player player)
        {
            this.player = player;
        }

        public float Speed
        {
            get
            {
                return VehicleSpeed;
            }
        }

        public void Transform()
        {
            player.SetState(new RobotState(player));
        }

        public void Shoot(bool angled)
        {
        }

        public void UseItem(int itemNumber)
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
