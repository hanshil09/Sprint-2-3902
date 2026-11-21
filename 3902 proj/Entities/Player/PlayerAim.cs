using System;
using Microsoft.Xna.Framework;

namespace ArmadilloGame.Entities;

// Owns the player's aim direction and the rules that shape it: free aim toward a world target,
// with a blocked cone straight up that the aim cannot point into. Kept separate from Player so the
// aiming rules live in one place and can be reused by weapons and the on-screen aim preview.
internal sealed class PlayerAim
{
    // Half-angle (radians) of the blocked cone pointing straight up; the aim clamps to its edge.
    private static readonly float DeadZoneHalfAngle = MathHelper.ToRadians(15f);

    private Vector2 direction = new Vector2(1f, 0f);

    // Current aim as a unit vector. Defaults to pointing right.
    public Vector2 Direction => direction;

    // Points the aim from an origin toward a world target, then clamps out of the straight-up cone.
    public void SetTarget(Vector2 origin, Vector2 worldTarget)
    {
        Vector2 delta = worldTarget - origin;
        if (delta.LengthSquared() <= float.Epsilon)
        {
            return;
        }
        direction = ClampOutOfUpwardDeadZone(Vector2.Normalize(delta));
    }

    // Rejects directions inside the straight-up cone by snapping them to the nearest cone edge.
    private static Vector2 ClampOutOfUpwardDeadZone(Vector2 aim)
    {
        // "Up" is negative Y. Angle away from straight up, measured on the horizontal axis.
        float angleFromUp = MathF.Acos(Math.Clamp(-aim.Y, -1f, 1f));
        if (angleFromUp >= DeadZoneHalfAngle)
        {
            return aim;
        }
        // Inside the cone: rebuild the aim at the cone edge, keeping the left/right side it leaned to.
        float side = aim.X >= 0f ? 1f : -1f;
        return new Vector2(MathF.Sin(DeadZoneHalfAngle) * side, -MathF.Cos(DeadZoneHalfAngle));
    }
}
