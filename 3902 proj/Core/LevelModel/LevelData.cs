using System.Collections.Generic;

namespace TransformersGame.Core
{
    internal sealed class Placement<TKind>
    {
        public Placement(TKind kind, int column, int row)
        {
            Kind = kind;
            Column = column;
            Row = row;
        }

        public TKind Kind { get; }

        public int Column { get; }

        public int Row { get; }
    }

    internal sealed class LevelData
    {
        public LevelData(
            string name,
            int columns,
            int rows,
            int playerColumn,
            int playerRow,
            IReadOnlyList<Placement<BlockKind>> blocks,
            IReadOnlyList<Placement<ItemKind>> items,
            IReadOnlyList<Placement<EnemyKind>> enemies,
            IReadOnlyList<Placement<int>> doors = null)
        {
            Name = name;
            Columns = columns;
            Rows = rows;
            PlayerColumn = playerColumn;
            PlayerRow = playerRow;
            Blocks = blocks;
            Items = items;
            Enemies = enemies;
            Doors = doors ?? System.Array.Empty<Placement<int>>();
        }

        public string Name { get; }

        public int Columns { get; }

        public int Rows { get; }

        public int PlayerColumn { get; }

        public int PlayerRow { get; }

        public IReadOnlyList<Placement<BlockKind>> Blocks { get; }

        public IReadOnlyList<Placement<ItemKind>> Items { get; }

        public IReadOnlyList<Placement<EnemyKind>> Enemies { get; }
        public IReadOnlyList<Placement<int>> Doors { get; }
    }
}
