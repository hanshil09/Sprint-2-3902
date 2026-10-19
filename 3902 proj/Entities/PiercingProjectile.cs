using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    public sealed class PiercingProjectile : EnergyProjectile
    {
        private readonly HashSet<IEnemy> hitEnemies = new();
        public PiercingProjectile(Vector2 position, Vector2 velocity, ISprite sprite)
            : base(position, velocity, false, velocity.X < 0, sprite) { }
        public void Hit(IEnemy enemy)
        {
            if (hitEnemies.Add(enemy)) enemy.TakeDamage(1);
            if (hitEnemies.Count >= 3) Deactivate();
        }
    }
}
