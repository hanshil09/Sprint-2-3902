using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
  
    internal sealed class SpringPad : IPlayerTrigger
    {
        public const int LaunchHeightPixels = 352;
        private const int TriggerInset = 4;
        private const int TriggerHeight = 14;
        private const float CenterTolerance = 18f;
        private const double CompressSeconds = 0.08;
        private const double ReleasedSeconds = 0.35;

        private readonly float launchSpeed;
        private ISprite sprite;
        private SpringPose pose = SpringPose.Idle;
        private double poseTimer;

        public SpringPad(Vector2 position, float gravity)
        {
            Position = position;
            // v = sqrt(2gh) gives the same launch height in every room regardless of its gravity.
            launchSpeed = (float)System.Math.Sqrt(2 * gravity * LaunchHeightPixels);
            sprite = BlockSpriteFactory.Instance.CreateSpringSprite(pose);
        }

        public Vector2 Position { get; set; }

        // Only the bottom of the tile triggers, where the player's feet are when standing on the block below.
        public Rectangle Bounds => new Rectangle(
            (int)Position.X + TriggerInset,
            (int)Position.Y + BlockSpriteFactory.BlockSize - TriggerHeight,
            BlockSpriteFactory.BlockSize - 2 * TriggerInset,
            TriggerHeight);

        public float LaunchSpeed => launchSpeed;

        // Fires only when the player is centred over the pad, so brushing it from the side does nothing.
        public void OnPlayerContact(IPlayer player)
        {
            float playerCenter = player.Position.X + player.Width / 2f;
            float padCenter = Position.X + BlockSpriteFactory.BlockSize / 2f;
            if (pose != SpringPose.Idle || System.Math.Abs(playerCenter - padCenter) > CenterTolerance)
            {
                return;
            }
            player.Bounce(launchSpeed);
            SetPose(SpringPose.Compressed, CompressSeconds);
        }

        public void Update(GameTime gameTime)
        {
            if (pose == SpringPose.Idle)
            {
                return;
            }
            poseTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (poseTimer > 0)
            {
                return;
            }
            if (pose == SpringPose.Compressed)
            {
                SetPose(SpringPose.Released, ReleasedSeconds);
            }
            else
            {
                SetPose(SpringPose.Idle, 0);
            }
        }

        public void Draw(SpriteBatch spriteBatch) => sprite.Draw(spriteBatch, Position);

        private void SetPose(SpringPose newPose, double seconds)
        {
            pose = newPose;
            poseTimer = seconds;
            sprite = BlockSpriteFactory.Instance.CreateSpringSprite(pose);
        }
    }

    internal enum SpringPose { Idle, Compressed, Released }
}
