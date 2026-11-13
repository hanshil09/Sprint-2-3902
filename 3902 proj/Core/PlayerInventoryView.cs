namespace TransformersGame.Core;

internal readonly record struct PlayerInventoryView(int MedkitCount, int ShieldCount, int BombLevel, int EnergonCount,
    bool HasBazookaCall, int BazookaAmmo)
{
    public bool BazookaLoaded => BazookaAmmo > 0;
}
