using TransformersGame.Core;

namespace TransformersGame.Interfaces
{
    internal interface IItemDropBlock : IBlock
    {
        bool TryReleaseItem(out ItemKind kind);
    }
}
