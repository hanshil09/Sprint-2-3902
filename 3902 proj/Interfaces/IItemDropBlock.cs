using TransformersGame.Core;

namespace TransformersGame.Interfaces
{
    interface IItemDropBlock : IBlock
    {
        bool TryReleaseItem(out ItemKind kind);
    }
}
