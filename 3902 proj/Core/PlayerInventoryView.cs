namespace ArmadilloGame.Core;

internal readonly record struct PlayerInventoryView(int MedkitCount, int ShieldCount, int BombLevel, int EnergonCount,
    bool HasShotgunCall, int ShotgunAmmo)
{
    public bool ShotgunLoaded => ShotgunAmmo > 0;
}
