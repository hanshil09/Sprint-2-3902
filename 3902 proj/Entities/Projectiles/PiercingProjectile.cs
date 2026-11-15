using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities
{
    internal sealed class PiercingProjectile : EnergyProjectile
    {
        private readonly HashSet<IEnemy> hitEnemies = new();
        public PiercingProjectile(Vector2 position, Vector2 velocity, ISprite sprite, bool angled = false)
            : base(position, velocity, angled, velocity.X < 0, sprite) { }
        public override void HitEnemy(IEnemy enemy)
        {
            if (hitEnemies.Add(enemy)) enemy.TakeDamage(1);
            if (hitEnemies.Count >= 3) Deactivate();
        }
    }
}
