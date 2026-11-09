using System;

namespace TransformersGame.Entities;

internal sealed class PlayerHealth
{
    private const int StartingHealth = 100;
    private const int MaximumHealthCap = 200;
    private const int UpgradeAmount = 20;
    private const int HealthCellAmount = 10;
    private const int DamageAmount = 3;

    public int Current { get; private set; } = StartingHealth;
    public int Maximum { get; private set; } = StartingHealth;
    public bool IsFull => Current == Maximum;

    public void TakeDamage() => Current = Math.Max(0, Current - DamageAmount);

    public void Restore() => Heal(Maximum);

    public bool TryUseHealthCell()
    {
        if (IsFull)
        {
            return false;
        }

        Heal(HealthCellAmount);
        return true;
    }

    public bool TryUpgrade()
    {
        if (Maximum == MaximumHealthCap && IsFull)
        {
            return false;
        }

        Maximum = Math.Min(MaximumHealthCap, Maximum + UpgradeAmount);
        Restore();
        return true;
    }

    private void Heal(int amount) => Current = Math.Min(Maximum, Current + amount);
}
