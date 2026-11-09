using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    
    internal sealed class InvinciblePlayer : PlayerDecorator
    {
        public const double Duration = 8000;
        private const double WarningTime = 2000;
        private const double WarningBlinkInterval = 80;

        private readonly ISprite aura;

        public InvinciblePlayer(IPlayer decoratedPlayer, IPlayerHost host)
            : base(decoratedPlayer, host, Duration)
        {
            aura = ItemSpriteFactory.Instance.CreateAuraSprite();
        }

        public override bool IsInvincible => true;

        public override bool TakeDamage() => false;

        public override void Update(GameTime gameTime)
        {
            aura.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Inner.Draw(spriteBatch);
            bool warningBlinkOff = RemainingMilliseconds < WarningTime
                && (int)(RemainingMilliseconds / WarningBlinkInterval) % 2 == 1;
            if (!warningBlinkOff)
            {
                DrawAura(spriteBatch);
            }
        }

        private void DrawAura(SpriteBatch spriteBatch)
        {
            // Centre the square glow on the player in either form.
            float x = Position.X + Width / 2f - aura.Width / 2f;
            float y = Position.Y + Height / 2f - aura.Height / 2f;
            aura.Draw(spriteBatch, new Vector2(x, y));
        }
    }
}
