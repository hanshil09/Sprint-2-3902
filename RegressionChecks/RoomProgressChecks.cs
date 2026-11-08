using TransformersGame.Core;
using TransformersGame.Entities;

internal static class RoomProgressChecks
{
    private static int passed;

    public static void Run()
    {
        CheckCollectedItemsPersist();
        CheckOpenedCratesPersist();
        CheckBossProgressPersists();
        CheckNewGameResetsProgress();
        Console.WriteLine($"{passed} room-progress checks passed.");
    }

    private static void Check(string name, bool condition)
    {
        if (!condition) throw new InvalidOperationException("FAIL: " + name);
        passed++;
        Console.WriteLine("PASS: " + name);
    }

    private static LevelData CreateData() =>
        LevelParser.Parse(new[] { "[tiles]", "P.%Q", "GGGGG" }).Data;

    private static LevelData CreateCrateData() =>
        LevelParser.Parse(new[] { "[tiles]", "P...Q", "GGGGG" }).Data;

    private static void CheckCollectedItemsPersist()
    {
        LevelData data = CreateData();
        RoomProgress progress = new RoomProgress();
        Level firstVisit = new Level(data, roomProgress: progress);
        Player player = new Player(firstVisit.Items[0].Position, firstVisit.Blocks, new ProjectileManager());
        Check("placed Bomb Pack collects on first visit",
            firstVisit.CollectItems(player).Count == 1 && player.BombLevel == 1);

        Level revisit = new Level(data, roomProgress: progress);
        Check("collected placed item stays gone on revisit",
            revisit.Items.Count == 0 && revisit.Blocks.OfType<SupplyCrate>().Single().IsSolid);
    }

    private static void CheckOpenedCratesPersist()
    {
        LevelData data = CreateCrateData();
        RoomProgress progress = new RoomProgress();
        Level firstVisit = new Level(data, roomProgress: progress);
        SupplyCrate crate = firstVisit.Blocks.OfType<SupplyCrate>().Single();
        crate.Destroy();
        new Room(firstVisit, new ProjectileManager(), new GameObjectCycler()).Unload();
        Check("crate destroyed just before leaving is recorded as opened",
            progress.IsCrateOpened(crate.ProgressId));

        Level revisitBeforeCollection = new Level(data, roomProgress: progress);
        SupplyCrate openedCrate = revisitBeforeCollection.Blocks.OfType<SupplyCrate>().Single();
        Check("opened crate stays open and preserves its uncollected drop",
            openedCrate.IsDestroyed && !openedCrate.IsSolid && revisitBeforeCollection.Items.Count == 1);

        Player player = new Player(openedCrate.Position, revisitBeforeCollection.Blocks, new ProjectileManager());
        Check("preserved crate drop can still be collected",
            revisitBeforeCollection.CollectItems(player).Count == 1 && player.BombLevel == 1);

        Level revisitAfterCollection = new Level(data, roomProgress: progress);
        Check("collected crate drop does not return",
            revisitAfterCollection.Items.Count == 0
            && revisitAfterCollection.Blocks.OfType<SupplyCrate>().Single().IsDestroyed);
    }

    private static void CheckBossProgressPersists()
    {
        LevelData data = LevelParser.Parse(new[] { "[tiles]", "P...m", "GGGGG" }).Data;
        RoomProgress progress = new RoomProgress();
        progress.RecordBossDefeated();
        Level level = new Level(data, roomProgress: progress);
        var enemies = new GameObjectCycler();
        level.FillEnemyCycler(enemies, new Player(level.PlayerStart, level.Blocks, new ProjectileManager()),
            new ProjectileManager());
        Check("defeated boss is omitted when rebuilding room",
            enemies.Objects.Count == 0);
    }

    private static void CheckNewGameResetsProgress()
    {
        string roomPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..",
            "3902 proj", "Content", "Levels", "crates-05.room"));
        var factory = new RoomFactory(roomPath);
        RoomProgress oldProgress = factory.LoadLevel(0).Progress;
        oldProgress.RecordCollectedItem("placed:1:1");
        oldProgress.RecordCrateOpened("crate:2:2");
        factory.StartNewGame();
        RoomProgress newProgress = factory.LoadLevel(0).Progress;
        Check("new game gets clean item and crate progress",
            !newProgress.HasCollectedItem("placed:1:1") && !newProgress.IsCrateOpened("crate:2:2"));
    }
}
