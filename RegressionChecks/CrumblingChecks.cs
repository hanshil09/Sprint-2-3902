using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;
using TransformersGame.States;

internal static class CrumblingChecks
{
    private static int passed;
    private static GameTime Step(double seconds) => new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(seconds));

    public static void Run()
    {
        foreach (CrumbleKind kind in Enum.GetValues<CrumbleKind>()) CheckLifecycle(kind);
        CheckStandingContact();
        CheckRoomLayouts();
        Console.WriteLine($"{passed} crumbling-platform checks passed.");
    }

    private static void Check(string name, bool condition)
    {
        if (!condition) throw new InvalidOperationException("FAIL: " + name);
        passed++;
        Console.WriteLine("PASS: " + name);
    }

    private static void CheckLifecycle(CrumbleKind kind)
    {
        double delay = kind == CrumbleKind.Fast ? 0.9 : kind == CrumbleKind.Medium ? 1.5 : 2.3;
        var state = new CrumblingPlatformStateMachine(kind);
        state.Update(Step(10), true);
        Check(kind + " stays ready without standing contact", state.Phase == CrumblingPhase.Ready);
        state.OnStand();
        state.Update(Step(delay - 0.01), true);
        Check(kind + " warns while still solid", state.IsSolid && state.Phase == CrumblingPhase.Cracking);
        state.OnStand();
        state.Update(Step(0.02), true);
        Check(kind + " repeated contact does not reset countdown", !state.IsSolid && state.Phase == CrumblingPhase.Missing);
        state.Update(Step(4), false);
        Check(kind + " waits for occupant to leave", state.Phase == CrumblingPhase.Missing);
        state.Update(Step(0.01), true);
        Check(kind + " reform preview is non-solid", state.Phase == CrumblingPhase.Reforming && !state.IsSolid);
        state.Update(Step(1), false);
        Check(kind + " cannot become solid inside occupant", !state.IsSolid);
        state.Update(Step(0.01), true);
        Check(kind + " restores when clear", state.Phase == CrumblingPhase.Ready && state.IsSolid);
        state.OnStand();
        Check(kind + " can crumble again", state.Phase == CrumblingPhase.Cracking);
    }

    private static void CheckStandingContact()
    {
        var platform = new CrumblingPlatform(new Vector2(100, 200), 3, CrumbleKind.Fast);
        var player = new Player(new Vector2(110, 100), new List<IBlock> { platform }, new ProjectileManager());
        player.Update(Step(0.3));
        float standingY = platform.Bounds.Top - player.Height;
        Check("player lands on crumble platform", player.IsOnGround && player.Position.Y == standingY);
        platform.Update(Step(0.91));
        player.Update(Step(0.1));
        Check("player contact starts timer and falls after platform vanishes", !player.IsOnGround && player.Position.Y > standingY);
        platform.SetPlayerBounds(new Rectangle(110, 200, player.Width, player.Height));
        platform.Update(Step(4));
        platform.Update(Step(1));
        Check("overlapping player prevents restoration", !platform.IsSolid);
        var untouched = new CrumblingPlatform(new Vector2(100, 200), 3, CrumbleKind.Fast);
        player = new Player(new Vector2(20, 200), new List<IBlock> { untouched }, new ProjectileManager());
        player.Move(Direction.Right);
        player.Update(Step(0.1));
        untouched.Update(Step(4));
        Check("side contact does not crumble platform", untouched.IsSolid);
    }

    private static void CheckRoomLayouts()
    {
        var paths = Directory.GetFiles("3902 proj/Content/Levels", "*.room").Order().ToArray();
        foreach (string path in paths)
        {
            LevelData data = LevelParser.Parse(File.ReadAllLines(path)).Data;
            var layouts = new HashSet<string>();
            bool valid = true;
            for (int seed = 0; seed < 24; seed++)
            {
                var level = new Level(data, new Random(seed));
                var crumbles = level.Blocks.OfType<CrumblingPlatform>().ToArray();
                valid &= crumbles.Length >= 2 && crumbles.Length <= 3;
                valid &= crumbles.Select(p => p.Kind).Distinct().Count() == crumbles.Length;
                valid &= crumbles.All(p => p.Bounds.Bottom <= 508);
                valid &= level.Blocks.Count(b => b.Bounds.Top == 508 && b.IsSolid) == 30;
                var spawnPlayer = new Player(level.PlayerStart, level.Blocks, new ProjectileManager());
                valid &= level.Blocks.All(b => !b.Bounds.Intersects(new Rectangle((int)level.PlayerStart.X, (int)level.PlayerStart.Y, spawnPlayer.Width, spawnPlayer.Height)));
                layouts.Add(Describe(level));
            }
            Check(Path.GetFileName(path) + " has 2-3 distinct variants, safe spawn and unchanged floor for 24 seeds", valid);
            Check(Path.GetFileName(path) + " has layout variation", layouts.Count > 1);
            Check(Path.GetFileName(path) + " same seed reproduces layout", Describe(new Level(data, new Random(4))) == Describe(new Level(data, new Random(4))));
        }
        var factory = new RoomFactory(paths.Select(Path.GetFullPath).ToArray());
        string initial = Describe(factory.LoadLevel(0));
        Check("room revisit keeps selection", initial == Describe(factory.LoadLevel(0)));
        var newGames = new HashSet<string> { initial };
        for (int i = 0; i < 12; i++)
        {
            factory.StartNewGame();
            newGames.Add(Describe(factory.LoadLevel(0)));
        }
        Check("new games reroll layouts", newGames.Count > 1);
    }

    private static string Describe(Level level)
    {
        return string.Join(";", level.Blocks.OfType<CrumblingPlatform>()
            .Select(p => $"{p.Position.X},{p.Position.Y},{p.Bounds.Width},{p.Kind}"));
    }
}
