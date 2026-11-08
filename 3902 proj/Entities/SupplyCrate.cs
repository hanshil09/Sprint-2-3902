using Microsoft.Xna.Framework;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    internal sealed class SupplyCrate : Block, IItemDropBlock
    {
        private readonly ItemKind itemKind;
        private bool released;

        public SupplyCrate(Vector2 position, ISprite sprite, ItemKind itemKind, string progressId,
            bool requiresMorphBall = false, bool opened = false)
            : base(position, sprite, true, requiresMorphBall ? BlockKind.MorphBallCrate : BlockKind.Crate)
        {
            this.itemKind = itemKind;
            ProgressId = progressId;
            RequiresMorphBall = requiresMorphBall;
            if (opened)
            {
                Destroy();
                released = true;
            }
        }

        public string ProgressId { get; }

        public bool RequiresMorphBall { get; }

        public ItemKind ItemKind => itemKind;

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
