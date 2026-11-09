using TransformersGame.Core;

if (args.Contains("--audio"))
{
    AudioChecks.Run();
    return;
}

if (args.Contains("--pickups"))
{
    PickupChecks.Run();
    return;
}

if (args.Contains("--crate-drops"))
{
    CrateDropChecks.Run();
    return;
}

if (args.Contains("--crumbling"))
{
    CrumblingChecks.Run();
    return;
}

// Default: small room-only suite for the presentation.
// The original full suite is still available with --extended.
if (args.Contains("--extended"))
{
    ExtendedChecks.Run();
    return;
}

int passed = 0;
void Check(string name, bool condition)
{
    if (!condition) throw new InvalidOperationException("FAIL: " + name);
    Console.WriteLine("PASS: " + name);
    passed++;
}

Console.WriteLine("BASIC ROOM CHECKS");
var roomFiles = Directory.GetFiles("3902 proj/Content/Levels", "*.room").Order().ToArray();
Check("seven room files", roomFiles.Length == 7);
foreach (string path in roomFiles)
{
    string room = Path.GetFileNameWithoutExtension(path);
    LevelParseResult result = LevelParser.Parse(File.ReadAllLines(path));
    Check(room + ": valid room and player spawn", result.Success);
    LevelData data = result.Data;

    // Room sizes vary, so scale expectations with the room instead of using fixed counts.
    // Variety: at least two enemies and at least two distinct kinds.
    Check(room + ": at least two enemy kinds",
        data.Enemies.Count >= 2 && data.Enemies.Select(e => e.Kind).Distinct().Count() >= 2);

    // Density: at least one pickup, and no more than roughly one screen's worth per 170 tiles.
    int tileArea = data.Columns * data.Rows;
    int maxPickups = Math.Max(3, tileArea / 170);
    Check(room + $": pickup count within 1..{maxPickups} for {data.Columns}x{data.Rows}",
        data.Items.Count >= 1 && data.Items.Count <= maxPickups);

    int returnDoors = data.Doors.Count(d => d.Kind == -1);
    int forwardDoors = data.Doors.Count(d => d.Kind == 1);
    int goals = data.Objects.Count(o => o.Kind == ObjectKind.Goal);
    Check(room + ": return door and one exit",
        returnDoors == 1 && ((forwardDoors == 1 && goals == 0) || (forwardDoors == 0 && goals == 1)));

    Check(room + ": exit above starting row", ParkourCheck.ExitRow(data) < data.PlayerRow);
    Check(room + ": exit reachable", ParkourCheck.CanReachExit(data));
}
Console.WriteLine($"{passed} basic room checks passed.");
