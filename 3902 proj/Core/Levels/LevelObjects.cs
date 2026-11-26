using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Core
{
    // Builds the interactive objects of a room. Adjacent platform tiles in one row become one platform.
    internal sealed class LevelObjects
    {
        private static readonly IReadOnlyDictionary<ObjectKind, IPlatformObjectDefinition> PlatformDefinitions =
            new Dictionary<ObjectKind, IPlatformObjectDefinition>
            {
                [ObjectKind.MovingPlatform] = new MovingPlatformDefinition(),
                [ObjectKind.Elevator] = new ElevatorDefinition()
            };

        private static readonly IReadOnlyDictionary<ObjectKind, ITriggerObjectDefinition> TriggerDefinitions =
            new Dictionary<ObjectKind, ITriggerObjectDefinition>
            {
                [ObjectKind.Spring] = new SpringDefinition(),
                [ObjectKind.Goal] = new GoalDefinition()
            };

        public LevelObjects(IReadOnlyList<Placement<ObjectKind>> placements, Func<int, int, Vector2> cellPosition, float gravity)
        {
            foreach (Placement<ObjectKind> placement in placements)
            {
                Vector2 position = cellPosition(placement.Column, placement.Row);
                if (TriggerDefinitions.TryGetValue(placement.Kind, out ITriggerObjectDefinition definition))
                {
                    IPlayerTrigger trigger = definition.Create(position, gravity);
                    Triggers.Add(trigger);
                    if (trigger is Goal goal)
                    {
                        Goal = goal;
                    }
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
                .Where(p => PlatformDefinitions.ContainsKey(p.Kind))
                .OrderBy(p => p.Row).ThenBy(p => p.Column).ToList();
            int i = 0;
            while (i < tiles.Count)
            {
                int length = RunLength(tiles, i);
                Placement<ObjectKind> first = tiles[i];
                IPlatformObjectDefinition definition = PlatformDefinitions[first.Kind];
                Platforms.Add(definition.Create(cellPosition(first.Column, first.Row), length));
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
    }
}
