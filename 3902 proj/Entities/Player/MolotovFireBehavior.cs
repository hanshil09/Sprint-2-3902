using System;
using Microsoft.Xna.Framework;
using ArmadilloGame.Controllers;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities;

// Molotov: hold to charge (longer = farther), release to throw. The player is rooted while charging.
// The current charge (0..1) is exposed so an aim preview can show the arc.
internal sealed class MolotovFireBehavior : IWeaponFireBehavior
{
    // Time held to reach full charge.
    private const double FullChargeMs = 900;

    private IPlayer rootedPlayer;
    private double chargeMs;
    private bool charging;

    public WeaponKind Kind => WeaponKind.Molotov;

    // Charge fraction 0..1 while holding; 0 when not charging. Used by the aim preview.
    public float Charge => (float)Math.Clamp(chargeMs / FullChargeMs, 0d, 1d);
    public bool IsCharging => charging;

    public void Update(IPlayer player, FirePhase phase, GameTime gameTime)
    {
        switch (phase)
        {
            case FirePhase.Pressed:
                BeginCharge(player);
                break;
            case FirePhase.Held when charging:
                chargeMs += gameTime.ElapsedGameTime.TotalMilliseconds;
                break;
            case FirePhase.Released when charging:
                Throw(player);
                break;
            case FirePhase.Up:
                // Lost the hold without a release event (e.g. weapon switch): cancel cleanly.
                if (charging)
                {
                    Reset();
                }
                break;
        }
    }

    private void BeginCharge(IPlayer player)
    {
        charging = true;
        chargeMs = 0;
        rootedPlayer = player;
        player.MovementLocked = true;
    }

    private void Throw(IPlayer player)
    {
        float charge = Charge;
        Release();
        player.FireMolotov(charge);
    }

    public void Reset() => Release();

    // Ends the charge and frees the player's movement.
    private void Release()
    {
        charging = false;
        chargeMs = 0;
        if (rootedPlayer != null)
        {
            rootedPlayer.MovementLocked = false;
            rootedPlayer = null;
        }
    }
}
