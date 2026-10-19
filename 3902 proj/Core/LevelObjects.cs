using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    // Builds the interactive objects of a room. Adjacent platform tiles in one row become one platform.
    internal sealed class LevelObjects
    {
        private const int Tile = BlockSpriteFactory.BlockSize;
        private const int PlatformTravelTiles = 4;
        private const int ElevatorTravelTiles = 6;
        private const float PlatformSpeed = 70f;
        private const float ElevatorSpeed = 60f;

        public LevelObjects(IReadOnlyList<Placement<ObjectKind>> placements, Func<int, int, Vector2> cellPosition, float gravity)
        {
            foreach (Placement<ObjectKind> placement in placements)
            {
                Vector2 position = cellPosition(placement.Column, placement.Row);
                if (placement.Kind == ObjectKind.Spring)
                {
                    Triggers.Add(new SpringPad(position, gravity));
                }
                else if (placement.Kind == ObjectKind.Goal)
                {
                    Goal = new Goal(position);
                    Triggers.Add(Goal);
                }
            }
            BuildPlatforms(placements, cellPosition);
        }

        public List<IBlock> Platforms { get; } = new List<IBlock>();

        public List<IPlayerTrigger> Triggers { get; } = new List<IPlayerTrigger>();

        public Goal Goal { get; }

        private void BuildPlatforms(IReadOnlyList<Placement<ObjectKind>> placements, Func<int, int, Vector2> cellPosition)
        {
            var tiles = placements
                .Where(p => p.Kind == ObjectKind.MovingPlatform || p.Kind == ObjectKind.Elevator)
                .OrderBy(p => p.Row).ThenBy(p => p.Column).ToList();
            int i = 0;
            while (i < tiles.Count)
            {
                int length = RunLength(tiles, i);
                Placement<ObjectKind> first = tiles[i];
                Platforms.Add(CreatePlatform(first.Kind, cellPosition(first.Column, first.Row), length));
                i += length;
            }
        }

        private static int RunLength(List<Placement<ObjectKind>> tiles, int start)
        {
            int length = 1;
            while (start + length < tiles.Count
                && tiles[start + length].Kind == tiles[start].Kind
                && tiles[start + length].Row == tiles[start].Row
                && tiles[start + length].Column == tiles[start].Column + length)
            {
                length++;
            }
            return length;
        }

        private static MovingPlatform CreatePlatform(ObjectKind kind, Vector2 start, int length)
        {
            bool elevator = kind == ObjectKind.Elevator;
            Vector2 travel = elevator ? new Vector2(0, -ElevatorTravelTiles * Tile) : new Vector2(PlatformTravelTiles * Tile, 0);
            ISprite tile = BlockSpriteFactory.Instance.CreatePlatformTileSprite(elevator);
            return new MovingPlatform(start, length, travel, elevator ? ElevatorSpeed : PlatformSpeed, tile);
        }
    }
}
