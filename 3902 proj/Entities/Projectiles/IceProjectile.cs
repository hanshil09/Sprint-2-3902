using Microsoft.Xna.Framework;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities;

internal sealed class IceProjectile : EnergyProjectile
{
    public IceProjectile(Vector2 position, Vector2 velocity, bool angled, bool facingLeft, ISprite sprite)
        : base(position, velocity, angled, facingLeft, sprite)
    {
    }

    public override void HitEnemy(IEnemy enemy)
    {
        if (enemy is Enemy regularEnemy)
        {
            regularEnemy.Freeze();
            Deactivate();
            return;
        }

        if (enemy is Boss boss)
        {
            boss.Freeze();
            Deactivate();
            return;
        }

        base.HitEnemy(enemy);
    }
}
