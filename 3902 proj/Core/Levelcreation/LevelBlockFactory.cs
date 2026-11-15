using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    internal sealed class LevelBlockFactory
    {
        private readonly LevelData data;
        private readonly RoomProgress progress;

        public LevelBlockFactory(LevelData data, Random random, RoomProgress progress)
        {
            this.data = data;
            this.progress = progress;
            ReinforcedSupply = SelectReinforcedSupply(data, random);
        }

        public Placement<ItemKind> ReinforcedSupply { get; }

        public IBlock Create(Placement<BlockKind> placement, Vector2 position)
        {
            foreach (Placement<ItemKind> drop in data.CrateDrops)
            {
                if (drop.Column == placement.Column && drop.Row == placement.Row)
                {
                    bool reinforced = placement.Kind == BlockKind.ReinforcedCrate || drop == ReinforcedSupply;
                    bool requiresMorphBall = placement.Kind == BlockKind.MorphBallCrate;
                    string crateId = $"crate:{placement.Column}:{placement.Row}";
                    ISprite crateSprite = BlockSpriteFactory.Instance.CreateSupplyCrateSprite(
                        drop.Kind, requiresMorphBall, reinforced);
                    return new SupplyCrate(position, crateSprite, drop.Kind, crateId, requiresMorphBall,
                        progress.IsCrateOpened(crateId), reinforced);
                }
            }
            ISprite sprite = BlockSpriteFactory.Instance.CreateBlockSprite(placement.Kind);
            return new Block(position, sprite, placement.Kind != BlockKind.Hazard, placement.Kind);
        }

        private static Placement<ItemKind> SelectReinforcedSupply(LevelData data, Random random)
        {
            if (random == null)
            {
                return null;
            }
            List<Placement<ItemKind>> candidates = new();
            foreach (Placement<ItemKind> drop in data.CrateDrops)
            {
                bool isMorphBallCrate = data.Blocks.Any(block =>
                    block.Column == drop.Column && block.Row == drop.Row
                    && block.Kind == BlockKind.MorphBallCrate);
                if (isMorphBallCrate || drop.Kind == ItemKind.BazookaCall)
                {
                    continue;
                }
                foreach (Placement<BlockKind> block in data.Blocks)
                {
                    if (block.Column == drop.Column && block.Row == drop.Row + 1
                        && block.Kind is not (BlockKind.Crate or BlockKind.ReinforcedCrate or BlockKind.Hazard))
                    {
                        candidates.Add(drop);
                        break;
                    }
                }
            }
            // Keep the authored tutorial crate fixed and reserve randomization for rooms with a choice.
            if (candidates.Count > 1)
            {
#pragma warning disable CA5394 // Gameplay layouts use repeatable seeds and have no security purpose.
                return candidates[random.Next(candidates.Count)];
#pragma warning restore CA5394
            }

            return null;
        }
    }
}
