using Microsoft.Xna.Framework;
using ArmadilloGame.Controllers;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities;

// Shotgun: one blast per press. Pressing starts a short windup; the blast fires once the windup
// finishes. Firing again requires releasing and pressing again. The player can keep moving.
internal sealed class ShotgunFireBehavior : IWeaponFireBehavior
{
    private const double WindupMs = 250;

    private bool winding;
    private double windupRemaining;

    public WeaponKind Kind => WeaponKind.Shotgun;

    public void Update(IPlayer player, FirePhase phase, GameTime gameTime)
    {
        if (phase == FirePhase.Up || phase == FirePhase.Released)
        {
            Reset();
            return;
        }

        if (phase == FirePhase.Pressed)
        {
            winding = true;
            windupRemaining = WindupMs;
        }

        if (!winding)
        {
            return;
        }

        windupRemaining -= gameTime.ElapsedGameTime.TotalMilliseconds;
        if (windupRemaining <= 0)
        {
            player.FireWeapon(Kind);
            // One blast per press: stop until the button is released and pressed again.
            winding = false;
        }
    }

    public void Reset()
    {
        winding = false;
        windupRemaining = 0;
    }
}
