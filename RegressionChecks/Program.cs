using TransformersGame.Core;

if (args.Contains("--selfie"))
{
    SelfieChecks.Run();
    return;
}

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
Check("eight room files (hub + seven spokes)", roomFiles.Length == 8);
foreach (string path in roomFiles)
{
    string room = Path.GetFileNameWithoutExtension(path);
    LevelParseResult result = LevelParser.Parse(File.ReadAllLines(path));
    Check(room + ": valid room and player spawn", result.Success);
    RoomChecks.Validate(result.Data, room, Check);
}
Console.WriteLine($"{passed} basic room checks passed.");
