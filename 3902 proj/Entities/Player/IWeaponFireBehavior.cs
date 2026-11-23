using Microsoft.Xna.Framework;
using ArmadilloGame.Controllers;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities;

// How a weapon reacts to the fire button over time. Each weapon kind has its own behavior so the
// firing rules (fire-once, autofire after a windup, charge-and-release) live with the weapon
// instead of in a growing switch.
internal interface IWeaponFireBehavior
{
    WeaponKind Kind { get; }

    // Called every gameplay frame with the current fire-button phase. The behavior decides when to
    // actually fire, consuming ammo through the player's loadout.
    void Update(IPlayer player, FirePhase phase, GameTime gameTime);

    // Dropped weapon selection or left gameplay: clear any in-progress windup or charge.
    void Reset();
}
