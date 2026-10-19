using System;
using System.Collections.Generic;

namespace TransformersGame.Core
{
    public static class LevelParser
    {
        public const int MaxColumns = 200;
        public const int MaxRows = 100;

        public const int ScreenColumns = 30;
        public const int ScreenRows = 17;

        public const char EmptySymbol = '.';
        public const char PlayerSymbol = 'P';

        private const string TilesSection = "[tiles]";
        private const string DefaultName = "Untitled";

        private static readonly Dictionary<char, BlockKind> BlockSymbols = new Dictionary<char, BlockKind>
        {
            ['G'] = BlockKind.Ground,
            ['B'] = BlockKind.Brick,
            ['S'] = BlockKind.Stone,
            ['M'] = BlockKind.Metal
        };

        private static readonly Dictionary<char, ItemKind> ItemSymbols = new Dictionary<char, ItemKind>
        {
            ['+'] = ItemKind.Medkit,
            ['*'] = ItemKind.Shield
        };

        private static readonly Dictionary<char, EnemyKind> EnemySymbols = new Dictionary<char, EnemyKind>
        {
            ['f'] = EnemyKind.Flyer,
            ['c'] = EnemyKind.Crawler,
            ['h'] = EnemyKind.Hopper,
            ['b'] = EnemyKind.Beetle,
            ['w'] = EnemyKind.Waver
        };

        public static LevelParseResult Parse(IEnumerable<string> lines)
        {
            ArgumentNullException.ThrowIfNull(lines);

            List<string> errors = new List<string>();
            List<string> warnings = new List<string>();
            List<string> rows = new List<string>();
            List<int> rowLineNumbers = new List<int>();
            string name = null;
            bool sawTiles = false;
            int lineNumber = 0;

            foreach (string rawLine in lines)
            {
                lineNumber++;
                string line = (rawLine ?? string.Empty).Trim();

                if (line.Length == 0 || line[0] == '#')
                {
                    continue;
                }

                if (line[0] == '[')
                {
                    if (!string.Equals(line, TilesSection, StringComparison.OrdinalIgnoreCase))
                    {
                        errors.Add($"line {lineNumber}: unknown section '{line}' (the only section is {TilesSection})");
                    }
                    else
                    {
                        if (sawTiles)
                        {
                            errors.Add($"line {lineNumber}: {TilesSection} appears more than once");
                        }
                        sawTiles = true;
                    }
                    continue;
                }

                if (sawTiles)
                {
                    rows.Add(line);
                    rowLineNumbers.Add(lineNumber);
                }
                else
                {
                    ParseHeaderLine(line, lineNumber, ref name, warnings);
                }
            }

            if (!sawTiles)
            {
                errors.Add($"missing {TilesSection} section");
            }
            else if (rows.Count == 0)
            {
                errors.Add($"the {TilesSection} section has no rows");
            }

            int columns = 0;
            foreach (string row in rows)
            {
                columns = Math.Max(columns, row.Length);
            }

            if (columns > MaxColumns || rows.Count > MaxRows)
            {
                errors.Add($"room is {columns} x {rows.Count} tiles; the maximum is {MaxColumns} x {MaxRows}");
            }

            if (errors.Count > 0)
            {
                return new LevelParseResult(null, errors, warnings);
            }

            if (columns > ScreenColumns || rows.Count > ScreenRows)
            {
                warnings.Add($"room is {columns} x {rows.Count} tiles but the screen shows {ScreenColumns} x {ScreenRows} and there is no scrolling camera yet, so part of it will be off-screen");
            }

            List<Placement<BlockKind>> blocks = new List<Placement<BlockKind>>();
            List<Placement<ItemKind>> items = new List<Placement<ItemKind>>();
            List<Placement<EnemyKind>> enemies = new List<Placement<EnemyKind>>();
            List<string> spawnLocations = new List<string>();
            Dictionary<char, UnknownTile> unknownTiles = new Dictionary<char, UnknownTile>();
            int playerColumn = 0;
            int playerRow = 0;

            for (int r = 0; r < rows.Count; r++)
            {
                string row = rows[r];
                int rowLine = rowLineNumbers[r];

                if (row.Length < columns)
                {
                    warnings.Add($"line {rowLine}: row is {row.Length} tiles wide but the room is {columns}; padding the right side with empty tiles");
                }

                for (int c = 0; c < row.Length; c++)
                {
                    char symbol = row[c];

                    if (symbol == EmptySymbol)
                    {
                        continue;
                    }

                    if (symbol == PlayerSymbol)
                    {
                        spawnLocations.Add($"line {rowLine} column {c + 1}");
                        playerColumn = c;
                        playerRow = r;
                    }
                    else if (BlockSymbols.TryGetValue(symbol, out BlockKind block))
                    {
                        blocks.Add(new Placement<BlockKind>(block, c, r));
                    }
                    else if (ItemSymbols.TryGetValue(symbol, out ItemKind item))
                    {
                        items.Add(new Placement<ItemKind>(item, c, r));
                    }
                    else if (EnemySymbols.TryGetValue(symbol, out EnemyKind enemy))
                    {
                        enemies.Add(new Placement<EnemyKind>(enemy, c, r));
                    }
                    else if (unknownTiles.TryGetValue(symbol, out UnknownTile seen))
                    {
                        seen.Count++;
                    }
                    else
                    {
                        unknownTiles[symbol] = new UnknownTile(rowLine, c + 1);
                    }
                }
            }

            foreach (KeyValuePair<char, UnknownTile> entry in unknownTiles)
            {
                UnknownTile tile = entry.Value;
                warnings.Add($"line {tile.Line} column {tile.Column}: unknown tile '{entry.Key}' ({tile.Count} total); treated as empty");
            }

            if (spawnLocations.Count == 0)
            {
                errors.Add($"no player spawn: put exactly one '{PlayerSymbol}' in the {TilesSection} grid");
            }
            else if (spawnLocations.Count > 1)
            {
                errors.Add($"found {spawnLocations.Count} player spawns ({string.Join("; ", spawnLocations)}); a room needs exactly one '{PlayerSymbol}'");
            }

            if (errors.Count > 0)
            {
                return new LevelParseResult(null, errors, warnings);
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                warnings.Add($"no 'name:' header; using '{DefaultName}'");
                name = DefaultName;
            }

            LevelData data = new LevelData(name, columns, rows.Count, playerColumn, playerRow, blocks, items, enemies);
            return new LevelParseResult(data, errors, warnings);
        }

        private static void ParseHeaderLine(string line, int lineNumber, ref string name, List<string> warnings)
        {
            int colon = line.IndexOf(':');
            if (colon <= 0)
            {
                warnings.Add($"line {lineNumber}: expected 'key: value' before {TilesSection}; ignored '{line}'");
                return;
            }

            string key = line.Substring(0, colon).Trim();
            string value = line.Substring(colon + 1).Trim();

            if (string.Equals(key, "name", StringComparison.OrdinalIgnoreCase))
            {
                if (name != null)
                {
                    warnings.Add($"line {lineNumber}: 'name' is set more than once; using the last one");
                }
                name = value;
            }
            else
            {
                warnings.Add($"line {lineNumber}: unknown header '{key}' ignored");
            }
        }

        private sealed class UnknownTile
        {
            public UnknownTile(int line, int column)
            {
                Line = line;
                Column = column;
                Count = 1;
            }

            public int Line { get; }

            public int Column { get; }

            public int Count { get; set; }
        }
    }
}