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
        CheckReinforcedCrates();
        CheckReinforcedLayout();
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

    private static void CheckReinforcedCrates()
    {
        LevelData data = LevelParser.Parse(new[] { "name: Roll test", "[tiles]", "P...R.....", "GGGGGGGGGG" }).Data;
        var level = new Level(data);
        SupplyCrate crate = level.Blocks.OfType<SupplyCrate>().Single();
        var collisions = new CollisionManager(960, 540);
        var shot = new EnergyProjectile(crate.Position - new Vector2(48, 0), new Vector2(520, 0), false, false, new StubSprite());
        shot.Update(Step(0.2));
        collisions.BuildTree(new List<IGameObject> { crate, shot });
        collisions.DetectCollision();
        Check("reinforced crate stops a shot without breaking", crate.IsSolid && !shot.IsActive);
        level = new Level(data);
        crate = level.Blocks.OfType<SupplyCrate>().Single();
        var bomb = new Bomb(crate.Position, new StubSprite(), gravity: 0, createExplosion: () => new StubSprite());
        bomb.Update(Step(1.2));
        collisions.BuildTree(new List<IGameObject> { crate, bomb });
        collisions.DetectCollision();
        Check("reinforced crate survives a bomb explosion", crate.IsSolid);

        level = new Level(data);
        crate = level.Blocks.OfType<SupplyCrate>().Single();
        var player = new Player(Vector2.Zero, level.Blocks, new ProjectileManager());
        var sounds = new List<GameSound>();
        player.SoundRequested += sounds.Add;
        player.Position = new Vector2(crate.Bounds.Left - player.Width - 1, crate.Bounds.Bottom - player.Height);
        player.Move(Direction.Right);
        player.Update(Step(1.0 / 60));
        Check("robot movement stops at the reinforced crate", crate.IsSolid
            && player.Position.X + player.Width == crate.Bounds.Left);
        level = new Level(data);
        crate = level.Blocks.OfType<SupplyCrate>().Single();
        player = new Player(Vector2.Zero, level.Blocks, new ProjectileManager());
        player.Transform();
        player.Position = new Vector2(crate.Bounds.Left - player.Width, crate.Bounds.Bottom - player.Height);
        player.Update(Step(1.0 / 60));
        Check("stationary ball contact does not break the crate", crate.IsSolid);
        level = new Level(data);
        crate = level.Blocks.OfType<SupplyCrate>().Single();
        player = new Player(Vector2.Zero, level.Blocks, new ProjectileManager());
        sounds = new List<GameSound>();
        player.SoundRequested += sounds.Add;
        player.Transform();
        player.Position = new Vector2(crate.Bounds.Left - player.Width, crate.Bounds.Bottom - player.Height);
        player.Move(Direction.Right);
        player.Update(Step(1.0 / 60));
        Check("moving ball breaks the crate and moves through it", crate.IsDestroyed && !crate.IsSolid
            && player.Position.X + player.Width > crate.Bounds.Left);
        level.Update(Step(0.01));
        level.Update(Step(0.01));
        Check("reinforced crate makes one crunch and releases one pickup", level.Items.Count == 1
            && sounds.Count(sound => sound == GameSound.CrateBreak) == 1);
        player.Position = crate.Position;
        level.CollectItems(player);
        Check("fixed crate rewards one Energon", player.EnergonCount == 1);

        level = new Level(data);
        crate = level.Blocks.OfType<SupplyCrate>().Single();
        var wall = new Block(crate.Position - new Vector2(32, 0), new StubSprite(), true);
        var blockedPlayer = new Player(Vector2.Zero, new IBlock[] { crate, wall }, new ProjectileManager(), gravity: 0);
        blockedPlayer.Transform();
        blockedPlayer.Position = new Vector2(wall.Bounds.Left - blockedPlayer.Width, crate.Position.Y);
        blockedPlayer.Move(Direction.Right);
        blockedPlayer.Update(Step(0.25));
        Check("closer solid block shields the reinforced crate", crate.IsSolid
            && blockedPlayer.Position.X + blockedPlayer.Width == wall.Bounds.Left);
    }

    private static void CheckReinforcedLayout()
    {
        LevelData first = LevelParser.Parse(File.ReadAllLines("3902 proj/Content/Levels/desert-01.room")).Data;
        var level = new Level(first, new Random(0));
        var player = new Player(level.PlayerStart, level.Blocks, new ProjectileManager(), level.Gravity);
        for (int frame = 0; frame < 30; frame++) player.Update(Step(1.0 / 60));
        player.Transform();
        for (int frame = 0; frame < 30; frame++)
        {
            player.Move(Direction.Right);
            player.Update(Step(1.0 / 60));
        }
        Check("the fixed Level 1 crate can be broken by rolling right from spawn", !level.HasIntactReinforcedCrate
            && level.Blocks.OfType<SupplyCrate>().Count(crate => crate.IsDestroyed) == 1);

        level = new Level(first, new Random(0));
        player = new Player(level.PlayerStart, level.Blocks, new ProjectileManager(), level.Gravity);
        for (int frame = 0; frame < 30; frame++) player.Update(Step(1.0 / 60));
        player.Transform();
        for (int frame = 0; frame < 16; frame++)
        {
            player.Move(Direction.Right);
            player.Update(Step(1.0 / 60));
        }
        var originalPosition = player.Position;
        player.Transform();
        Check("low ceiling prevents transformation without changing position", player.IsInBallForm
            && player.Position == originalPosition);
        player.Position = new Vector2(100, player.Position.Y);
        player.Transform();
        Check("robot transformation succeeds again in a clear area", !player.IsInBallForm);

        LevelData workshop = LevelParser.Parse(File.ReadAllLines("3902 proj/Content/Levels/crates-05.room")).Data;
        static Vector2 ReinforcedPosition(Level level) =>
            level.Blocks.OfType<SupplyCrate>().Single(crate => crate.Kind == BlockKind.ReinforcedCrate).Position;
        var selections = new HashSet<Vector2>();
        for (int seed = 0; seed < 8; seed++)
        {
            var seededLevel = new Level(workshop, new Random(seed));
            Vector2 cratePosition = ReinforcedPosition(seededLevel);
            selections.Add(cratePosition);
            Rectangle supportProbe = new Rectangle((int)cratePosition.X, (int)cratePosition.Y + 32, 32, 1);
            Check("reinforced crate has permanent support for seed " + seed,
                seededLevel.Blocks.Any(block => block.IsSolid && block is not CrumblingPlatform
                    && block.Bounds.Intersects(supportProbe)));
        }
        Check("workshop randomizes between two existing supported crate spots", selections.Count == 2);
        Check("same room seed picks the same reinforced crate on revisit",
            ReinforcedPosition(new Level(workshop, new Random(42))) == ReinforcedPosition(new Level(workshop, new Random(42))));
    }
}
