
using Microsoft.Xna.Framework;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    internal sealed class SupplyCrate : Block, IItemDropBlock
    {
        private readonly ItemKind itemKind;
        private bool released;

        public SupplyCrate(Vector2 position, ISprite sprite,
            ItemKind itemKind, string progressId,
            bool requiresMorphBall = false,
            bool opened = false,
            bool reinforced = false)
            : base(position, sprite, true,
                reinforced ? BlockKind.ReinforcedCrate
                : requiresMorphBall ? BlockKind.MorphBallCrate
                : BlockKind.Crate)
        {
            this.itemKind = itemKind;
            ProgressId = progressId;
            RequiresMorphBall = requiresMorphBall;

            if (opened)
            {
                Break();
                released = true;
            }
        }

        public SupplyCrate(Vector2 position, ISprite sprite,
            ItemKind itemKind, bool reinforced = false)
            : this(position, sprite, itemKind, null,
                false, false, reinforced)
        {
        }

        public string ProgressId { get; }

        public bool RequiresMorphBall { get; }

        public ItemKind ItemKind => itemKind;

        public bool TryBreakFromRoll()
        {
            if (Kind != BlockKind.ReinforcedCrate || IsDestroyed)
            {
                return false;
            }

            Break();
            return true;
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
