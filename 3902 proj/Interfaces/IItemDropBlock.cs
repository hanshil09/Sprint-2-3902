using ArmadilloGame.Core;

namespace ArmadilloGame.Interfaces
{
    interface IItemDropBlock : IBlock
    {
        bool TryReleaseItem(out ItemKind kind);
    }
}
