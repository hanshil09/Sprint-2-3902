using System;

namespace TransformersGame.Entities;

internal sealed class PlayerShield
{
    private const double DurationMilliseconds = 5000;
    private double remainingMilliseconds;

    public bool IsActive => remainingMilliseconds > 0;

    public void Start() => remainingMilliseconds = DurationMilliseconds;

    public void Update(TimeSpan elapsed)
    {
        remainingMilliseconds = Math.Max(0, remainingMilliseconds - elapsed.TotalMilliseconds);
    }
}
