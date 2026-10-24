using Microsoft.Xna.Framework;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    internal sealed class SupplyCrate : Block, IItemDropBlock
    {
        private readonly ItemKind itemKind;
        private bool released;

        public SupplyCrate(Vector2 position, ISprite sprite, ItemKind itemKind)
            : base(position, sprite, true, BlockKind.Crate)
        {
            this.itemKind = itemKind;
        }

        public bool TryReleaseItem(out ItemKind kind)
        {
            kind = itemKind;
            if (!IsDestroyed || released)
            {
                return false;
            }
            released = true;
            return true;
        }
    }
}
