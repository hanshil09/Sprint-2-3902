using Microsoft.Xna.Framework;
using TransformersGame.Collisions;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

internal static class CrateDropChecks
{
    private static int passed;
    private static GameTime Step(double seconds) => new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(seconds));

    public static void Run()
    {
        LevelParseResult parsed = LevelParser.Parse(new[] { "name: Supply test", "[tiles]", "P.HEC", "GGGGG" });
        Check("supply symbols parse without warnings", parsed.Success && parsed.Warnings.Count == 0);
        Check("both supply symbols remain crate blocks", parsed.Data.Blocks.Count(b => b.Kind == BlockKind.Crate) == 3);
        Check("crate contents are separate from loose pickups", parsed.Data.CrateDrops.Count == 2 && parsed.Data.Items.Count == 0);
        Check("H contains Health Cell and E contains Energon", parsed.Data.CrateDrops[0].Kind == ItemKind.HealthCell
            && parsed.Data.CrateDrops[1].Kind == ItemKind.Energon);
        CheckShotDrop(parsed.Data, 0, ItemKind.HealthCell);
        CheckShotDrop(parsed.Data, 1, ItemKind.Energon);
        CheckBombDrop(parsed.Data);
        CheckOrdinaryBlocks(parsed.Data);
        Console.WriteLine($"{passed} crate-drop checks passed.");
    }

    private static void Check(string name, bool condition)
    {
        if (!condition) throw new InvalidOperationException("FAIL: " + name);
        passed++;
        Console.WriteLine("PASS: " + name);
    }

    private static void CheckShotDrop(LevelData data, int index, ItemKind kind)
    {
        var level = new Level(data);
        SupplyCrate crate = level.Blocks.OfType<SupplyCrate>().OrderBy(b => b.Position.X).ElementAt(index);
        level.Update(Step(0.1));
        Check(kind + " is hidden before destruction", level.Items.Count == 0 && crate.IsSolid);
        var shot = new EnergyProjectile(crate.Position - new Vector2(48, 0), new Vector2(520, 0), false, false, new StubSprite());
        shot.Update(Step(0.2));
        var collisions = new CollisionManager(960, 540);
        collisions.BuildTree(new List<IGameObject> { crate, shot });
        collisions.DetectCollision();
        Check(kind + " crate breaks through existing shot collision", crate.IsDestroyed && !crate.IsSolid && !shot.IsActive);
        level.Update(Step(0.1));
        Check(kind + " spawns one pickup at crate position", level.Items.Count == 1 && level.Items[0].Position == crate.Position);
        crate.Destroy();
        collisions.DetectCollision();
        level.Update(Step(0.1));
        Check(kind + " repeated destruction cannot duplicate pickup", level.Items.Count == 1);
        var player = new Player(crate.Position, level.Blocks, new ProjectileManager());
        player.TakeDamage();
        int health = player.Health;
        level.CollectItems(player);
        Check(kind + " pickup applies its actual effect", kind == ItemKind.HealthCell ? player.Health > health : player.EnergonCount == 1);
        Check(kind + " collected pickup leaves room", level.Items.Count == 0);
        level.Update(Step(1));
        Check(kind + " does not respawn after collection", level.Items.Count == 0);
    }

    private static void CheckBombDrop(LevelData data)
    {
        var level = new Level(data);
        SupplyCrate crate = level.Blocks.OfType<SupplyCrate>().First();
        var bomb = new Bomb(crate.Position, new StubSprite(), gravity: 0, createExplosion: () => new StubSprite());
        bomb.Update(Step(1.2));
        var collisions = new CollisionManager(960, 540);
        collisions.BuildTree(new List<IGameObject> { bomb, crate });
        collisions.DetectCollision();
        level.Update(Step(0.01));
        Check("bomb explosion releases supply", crate.IsDestroyed && level.Items.Count == 1);
        collisions.DetectCollision();
        level.Update(Step(0.01));
        Check("multi-frame explosion cannot duplicate supply", level.Items.Count == 1);
    }

    private static void CheckOrdinaryBlocks(LevelData data)
    {
        var level = new Level(data);
        Block plain = level.Blocks.OfType<Block>().First(b => b.Kind == BlockKind.Crate && b is not SupplyCrate);
        plain.Destroy();
        level.Update(Step(1));
        Check("plain C crate stays empty", level.Items.Count == 0);
        Block ground = level.Blocks.OfType<Block>().First(b => b.Kind == BlockKind.Ground);
        ground.Destroy();
        Check("ordinary floor remains indestructible", ground.IsSolid && !ground.IsDestroyed);
        var workshop = LevelParser.Parse(File.ReadAllLines("3902 proj/Content/Levels/crates-05.room"));
        Check("workshop contains both designated drop types", workshop.Success
            && workshop.Data.CrateDrops.Any(d => d.Kind == ItemKind.HealthCell)
            && workshop.Data.CrateDrops.Any(d => d.Kind == ItemKind.Energon));
    }
}
