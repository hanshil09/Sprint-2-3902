using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.States
{
    internal class RobotState : IPlayerState
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

        public void Shoot(bool angled)
        {
            player.Arsenal.FireShot(angled);
            player.SetState(new ShootingRobotState(player, angled));
        }

        // Robot form can use every weapon: 1 orb, 2 bomb, 5 piercing, 6 spread, 7 boomerang.
        public void UseItem(int itemNumber)
        {
            switch (itemNumber)
            {
                case 1:
                    player.Arsenal.FireOrb();
                    break;
                case 2:
                    player.DropBomb();
                    return;
                case 5:
                    player.Arsenal.FirePiercing();
                    break;
                case 6:
                    player.Arsenal.FireSpread();
                    break;
                case 7:
                    player.Arsenal.ThrowBoomerang();
                    break;
                default:
                    return;
            }
            player.SetState(new ShootingRobotState(player, false));
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
