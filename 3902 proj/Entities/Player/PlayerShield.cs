using System;
using TransformersGame.Core;

namespace TransformersGame.Entities;

internal sealed class PlayerShield
{
    private const double DurationMilliseconds = 5000;
    private double remainingMilliseconds;

    public bool IsActive => remainingMilliseconds > 0;

    public bool TryActivate(PlayerInventory inventory)
    {
        if (IsActive || !inventory.TryUseShield())
        {
            return false;
        }

        remainingMilliseconds = DurationMilliseconds;
        return true;
    }

    public void Update(TimeSpan elapsed)
    {
        remainingMilliseconds = Math.Max(0, remainingMilliseconds - elapsed.TotalMilliseconds);
    }
}
