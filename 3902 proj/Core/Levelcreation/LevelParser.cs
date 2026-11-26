using System;
using System.Collections.Generic;

namespace ArmadilloGame.Core
{
    internal static class LevelParser
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
            ['M'] = BlockKind.Metal,
            ['C'] = BlockKind.Crate,
            ['H'] = BlockKind.Crate,
            ['E'] = BlockKind.Crate,
            ['j'] = BlockKind.Crate,
            ['k'] = BlockKind.Crate,
            ['Q'] = BlockKind.MorphBallCrate,
            ['R'] = BlockKind.ReinforcedCrate,
            ['X'] = BlockKind.Hazard,
            ['W'] = BlockKind.Wood,
            ['Y'] = BlockKind.Caution,
            ['A'] = BlockKind.Alloy
        };

        private static readonly Dictionary<char, ItemKind> CrateSymbols = new Dictionary<char, ItemKind>
        {
            ['H'] = ItemKind.HealthCell,
            ['E'] = ItemKind.Energon,
            ['j'] = ItemKind.ShotgunCall,
            ['k'] = ItemKind.Key,
            ['Q'] = ItemKind.BombPack,
            ['R'] = ItemKind.Energon
        };

        private static readonly Dictionary<char, ItemKind> ItemSymbols = new Dictionary<char, ItemKind>
        {
            ['+'] = ItemKind.Medkit,
            ['*'] = ItemKind.Shield,
            ['!'] = ItemKind.HealthCell,
            ['$'] = ItemKind.ShieldBattery,
            ['&'] = ItemKind.SparkCore,
            ['~'] = ItemKind.TurboBoost,
            ['@'] = ItemKind.Energon,
            ['%'] = ItemKind.BombPack,
            ['?'] = ItemKind.MaxHealthUp,
            ['K'] = ItemKind.Key,
            // Weapon pickups: g = pistol, n = shotgun, o = molotov.
            ['g'] = ItemKind.PistolPickup,
            ['n'] = ItemKind.ShotgunPickup,
            ['o'] = ItemKind.MolotovPickup
        };

        private static readonly Dictionary<char, EnemyKind> EnemySymbols = new Dictionary<char, EnemyKind>
        {
            ['f'] = EnemyKind.Flyer,
            ['c'] = EnemyKind.Crawler,
            ['h'] = EnemyKind.Hopper,
            ['b'] = EnemyKind.Beetle,
            ['w'] = EnemyKind.Waver,
            ['t'] = EnemyKind.Sentry,
            ['v'] = EnemyKind.Sentinel,
            ['m'] = EnemyKind.Warlord
        };

        private static readonly Dictionary<char, ObjectKind> ObjectSymbols = new Dictionary<char, ObjectKind>
        {
            ['='] = ObjectKind.MovingPlatform,
            ['I'] = ObjectKind.Elevator,
            ['^'] = ObjectKind.Spring,
            ['F'] = ObjectKind.Goal
        };

        // Locked-door symbols -> destination room index. Chosen to avoid colliding with
        // block/item/enemy/object/player symbols; each needs a key to open.
        private static readonly Dictionary<char, int> LockedDoorSymbols = new Dictionary<char, int>
        {
            ['D'] = 0, ['J'] = 1, ['L'] = 2, ['N'] = 3, ['O'] = 4,
            ['q'] = 5, ['T'] = 6, ['U'] = 7, ['V'] = 8, ['Z'] = 9
        };

        public static LevelParseResult Parse(IEnumerable<string> lines)
        {
            ArgumentNullException.ThrowIfNull(lines);

            List<string> errors = new List<string>();
            List<string> warnings = new List<string>();
            HeaderData header = CollectHeaderAndRows(lines, warnings, errors);
            int columns = GetColumnCount(header.Rows);
            ValidateDimensions(columns, header.Rows.Count, errors);

            if (errors.Count > 0)
            {
                return CreateFailedResult(errors, warnings);
            }

            AddScreenSizeWarning(columns, header.Rows.Count, warnings);

            GridPlacements placements = PlaceGridTiles(header.Rows, header.RowLineNumbers, columns, warnings);
            ReportUnknownSymbols(placements.UnknownTiles, warnings);
            ValidatePlayerSpawns(placements.SpawnLocations, errors);

            if (errors.Count > 0)
            {
                return CreateFailedResult(errors, warnings);
            }

            return BuildLevelParseResult(header, columns, placements, errors, warnings);
        }

        private static HeaderData CollectHeaderAndRows(IEnumerable<string> lines, List<string> warnings, List<string> errors)
        {
            HeaderData header = new HeaderData();
            string name = null;
            float gravity = Physics.DefaultGravity;
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
                    header.Rows.Add(line);
                    header.RowLineNumbers.Add(lineNumber);
                }
                else
                {
                    ParseHeaderLine(line, lineNumber, ref name, ref gravity, warnings, errors);
                }
            }

            if (!sawTiles)
            {
                errors.Add($"missing {TilesSection} section");
            }
            else if (header.Rows.Count == 0)
            {
                errors.Add($"the {TilesSection} section has no rows");
            }

            header.Name = name;
            header.Gravity = gravity;
            return header;
        }

        private static int GetColumnCount(List<string> rows)
        {
            int columns = 0;
            foreach (string row in rows)
            {
                columns = Math.Max(columns, row.Length);
            }
            return columns;
        }

        private static void ValidateDimensions(int columns, int rowCount, List<string> errors)
        {
            if (columns > MaxColumns || rowCount > MaxRows)
            {
                errors.Add($"room is {columns} x {rowCount} tiles; the maximum is {MaxColumns} x {MaxRows}");
            }
        }

        private static void AddScreenSizeWarning(int columns, int rowCount, List<string> warnings)
        {
            if (columns > ScreenColumns || rowCount > ScreenRows)
            {
                warnings.Add($"room is {columns} x {rowCount} tiles, larger than the {ScreenColumns} x {ScreenRows} screen; the camera will scroll to follow the player");
            }
        }

        private static GridPlacements PlaceGridTiles(
            List<string> rows,
            List<int> rowLineNumbers,
            int columns,
            List<string> warnings)
        {
            GridPlacements placements = new GridPlacements();
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
                    PlaceTile(row[c], c, r, rowLine, placements);
                }
            }
            return placements;
        }

        private static void PlaceTile(char symbol, int column, int row, int rowLine, GridPlacements placements)
        {
            if (symbol == EmptySymbol)
            {
                return;
            }

            if (symbol == PlayerSymbol)
            {
                placements.SpawnLocations.Add($"line {rowLine} column {column + 1}");
                placements.PlayerColumn = column;
                placements.PlayerRow = row;
            }
            else if (symbol == '<' || symbol == '>')
            {
                placements.Doors.Add(new Placement<DoorSpec>(DoorSpec.Relative(symbol == '<' ? -1 : 1), column, row));
            }
            else if (symbol >= '0' && symbol <= '9')
            {
                // Digit = unlocked door to that room index (hub-and-spoke navigation).
                placements.Doors.Add(new Placement<DoorSpec>(DoorSpec.ToRoom(symbol - '0', false), column, row));
            }
            else if (LockedDoorSymbols.TryGetValue(symbol, out int lockedTarget))
            {
                // Dedicated locked-door letters map to room indices; they need a key.
                placements.Doors.Add(new Placement<DoorSpec>(DoorSpec.ToRoom(lockedTarget, true), column, row));
            }
            else if (BlockSymbols.TryGetValue(symbol, out BlockKind block))
            {
                placements.Blocks.Add(new Placement<BlockKind>(block, column, row));
                AddCrateDrop(symbol, column, row, placements.CrateDrops);
            }
            else if (ItemSymbols.TryGetValue(symbol, out ItemKind item))
            {
                placements.Items.Add(new Placement<ItemKind>(item, column, row));
            }
            else if (EnemySymbols.TryGetValue(symbol, out EnemyKind enemy))
            {
                placements.Enemies.Add(new Placement<EnemyKind>(enemy, column, row));
            }
            else if (ObjectSymbols.TryGetValue(symbol, out ObjectKind levelObject))
            {
                placements.Objects.Add(new Placement<ObjectKind>(levelObject, column, row));
            }
            else if (placements.UnknownTiles.TryGetValue(symbol, out UnknownTile seen))
            {
                seen.Count++;
            }
            else
            {
                placements.UnknownTiles[symbol] = new UnknownTile(rowLine, column + 1);
            }
        }

        private static void ReportUnknownSymbols(Dictionary<char, UnknownTile> unknownTiles, List<string> warnings)
        {
            foreach (KeyValuePair<char, UnknownTile> entry in unknownTiles)
            {
                UnknownTile tile = entry.Value;
                warnings.Add($"line {tile.Line} column {tile.Column}: unknown tile '{entry.Key}' ({tile.Count} total); treated as empty");
            }
        }

        private static void ValidatePlayerSpawns(List<string> spawnLocations, List<string> errors)
        {
            if (spawnLocations.Count == 0)
            {
                errors.Add($"no player spawn: put exactly one '{PlayerSymbol}' in the {TilesSection} grid");
            }
            else if (spawnLocations.Count > 1)
            {
                errors.Add($"found {spawnLocations.Count} player spawns ({string.Join("; ", spawnLocations)}); a room needs exactly one '{PlayerSymbol}'");
            }
        }

        private static LevelParseResult BuildLevelParseResult(
            HeaderData header,
            int columns,
            GridPlacements placements,
            List<string> errors,
            List<string> warnings)
        {
            string name = header.Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                warnings.Add($"no 'name:' header; using '{DefaultName}'");
                name = DefaultName;
            }

            LevelData data = new LevelData(name, columns, header.Rows.Count, placements.PlayerColumn, placements.PlayerRow,
                placements.Blocks, placements.Items, placements.Enemies, placements.Doors, header.Gravity)
            {
                Objects = placements.Objects,
                CrateDrops = placements.CrateDrops
            };
            return new LevelParseResult(data, errors, warnings);
        }

        private static LevelParseResult CreateFailedResult(List<string> errors, List<string> warnings)
        {
            return new LevelParseResult(null, errors, warnings);
        }

        private static void AddCrateDrop(char symbol, int column, int row, List<Placement<ItemKind>> drops)
        {
            if (CrateSymbols.TryGetValue(symbol, out ItemKind kind))
            {
                drops.Add(new Placement<ItemKind>(kind, column, row));
            }
        }

        private static void ParseHeaderLine(string line, int lineNumber, ref string name, ref float gravity, List<string> warnings, List<string> errors)
        {
            int colon = line.IndexOf(':', StringComparison.Ordinal);
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
            else if (string.Equals(key, "gravity", StringComparison.OrdinalIgnoreCase))
            {
                if (!float.TryParse(value, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out float parsed)
                    || !float.IsFinite(parsed) || parsed < 600 || parsed > 2200)
                {
                    errors.Add($"line {lineNumber}: gravity must be between 600 and 2200 pixels/second squared");
                }
                else gravity = parsed;
            }
            else
            {
                warnings.Add($"line {lineNumber}: unknown header '{key}' ignored");
            }
        }

        private sealed class HeaderData
        {
            public List<string> Rows { get; } = new List<string>();

            public List<int> RowLineNumbers { get; } = new List<int>();

            public string Name { get; set; }

            public float Gravity { get; set; }
        }

        private sealed class GridPlacements
        {
            public List<Placement<BlockKind>> Blocks { get; } = new List<Placement<BlockKind>>();

            public List<Placement<ItemKind>> Items { get; } = new List<Placement<ItemKind>>();

            public List<Placement<ItemKind>> CrateDrops { get; } = new List<Placement<ItemKind>>();

            public List<Placement<EnemyKind>> Enemies { get; } = new List<Placement<EnemyKind>>();

            public List<Placement<DoorSpec>> Doors { get; } = new List<Placement<DoorSpec>>();

            public List<Placement<ObjectKind>> Objects { get; } = new List<Placement<ObjectKind>>();

            public List<string> SpawnLocations { get; } = new List<string>();

            public Dictionary<char, UnknownTile> UnknownTiles { get; } = new Dictionary<char, UnknownTile>();

            public int PlayerColumn { get; set; }

            public int PlayerRow { get; set; }
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
