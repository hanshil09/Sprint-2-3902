using System;

namespace ArmadilloGame.Entities;

internal sealed class PlayerShield
{
    private const double DurationMilliseconds = 15000;
    private double remainingMilliseconds;

    public bool IsActive => remainingMilliseconds > 0;

    public void Start() => remainingMilliseconds = DurationMilliseconds;

    public void Update(TimeSpan elapsed)
    {
        remainingMilliseconds = Math.Max(0, remainingMilliseconds - elapsed.TotalMilliseconds);
    }
}
