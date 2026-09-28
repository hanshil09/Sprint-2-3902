using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.States
{
    public class ShootingRobotState : IPlayerState
    {
        private const double ShootDuration = 280;

        private Player player;
        private double timeRemaining;

        public ShootingRobotState(Player player)
        {
            this.player = player;
            timeRemaining = ShootDuration;
        }

        public float Speed
        {
            get
            {
                return RobotState.RobotSpeed;
            }
        }

        public void Transform()
        {
            player.SetState(new VehicleState(player));
        }

        public void Shoot()
        {
        }

        public void UseItem(int itemNumber)
        {
        }

        public void Update(GameTime gameTime)
        {
            timeRemaining -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timeRemaining <= 0)
            {
                player.SetState(new RobotState(player));
            }
        }

        public ISprite CreateSprite()
        {
            return PlayerSpriteFactory.Instance.CreateShootingRobotSprite(player.FacingLeft);
        }
    }
}
