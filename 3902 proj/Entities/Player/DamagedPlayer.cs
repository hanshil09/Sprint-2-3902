using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    
    internal class DamagedPlayer : PlayerDecorator
    {
        private const double DamageDuration = 1000;
        private const double BlinkInterval = 100;

        public DamagedPlayer(IPlayer decoratedPlayer, IPlayerHost host)
            : base(decoratedPlayer, host, DamageDuration)
        {
        }

        public override bool TakeDamage() => false;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if ((int)(RemainingMilliseconds / BlinkInterval) % 2 == 0)
            {
                Inner.Draw(spriteBatch);
            }
        }
    }
}
