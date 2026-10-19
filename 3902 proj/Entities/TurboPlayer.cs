using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
       internal sealed class TurboPlayer : PlayerDecorator
    {
        public const double Duration = 6000;
        private const float BoostedSpeedScale = 1.7f;
        private const float MinimumTrailSpeed = 0.5f;

        private readonly ISprite rightTrail;
        private readonly ISprite leftTrail;
        private Vector2 previousPosition;

        public TurboPlayer(IPlayer decoratedPlayer, IPlayerHost host)
            : base(decoratedPlayer, host, Duration)
        {
            rightTrail = ItemSpriteFactory.Instance.CreateSpeedTrailSprite(false);
            leftTrail = ItemSpriteFactory.Instance.CreateSpeedTrailSprite(true);
            Inner.SpeedScale = BoostedSpeedScale;
            previousPosition = decoratedPlayer.Position;
        }

        public override void Update(GameTime gameTime)
        {
            previousPosition = Position;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float movedX = Position.X - previousPosition.X;
            if (System.Math.Abs(movedX) > MinimumTrailSpeed)
            {
                // Streak behind the player, on the side opposite to the movement.
                ISprite trail = movedX > 0 ? rightTrail : leftTrail;
                float x = movedX > 0 ? Position.X - trail.Width : Position.X + Width;
                trail.Draw(spriteBatch, new Vector2(x, Position.Y + Height / 2f - trail.Height / 2f));
            }
            Inner.Draw(spriteBatch);
        }

        protected override void OnExpired()
        {
            Inner.SpeedScale = 1f;
        }
    }
}
