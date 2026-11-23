using Microsoft.Xna.Framework;
using ArmadilloGame.Controllers;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities;

// Fires once on the press edge, ignoring held and release. Used by the pistol and boomerang.
internal sealed class InstantFireBehavior : IWeaponFireBehavior
{
    public InstantFireBehavior(WeaponKind kind) => Kind = kind;

    public WeaponKind Kind { get; }

    public void Update(IPlayer player, FirePhase phase, GameTime gameTime)
    {
        if (phase == FirePhase.Pressed)
        {
            player.FireWeapon(Kind);
        }
    }

    public void Reset()
    {
    }
}
