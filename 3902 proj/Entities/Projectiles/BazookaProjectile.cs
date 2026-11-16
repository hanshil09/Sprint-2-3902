using Microsoft.Xna.Framework;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities
{
    internal sealed class ShotgunProjectile : EnergyProjectile
    {
        private const int Damage = 5;

        public ShotgunProjectile(Vector2 position, Vector2 velocity, ISprite sprite)
            : base(position, velocity, false, velocity.X < 0, sprite)
        {
        }

        public override void HitEnemy(IEnemy enemy)
        {
            if (!enemy.IsDestroyed) enemy.TakeDamage(Damage);
            Deactivate();
        }
    }
}
