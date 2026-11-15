using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using TransformersGame.Collisions;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

internal static class CrateDropChecks
{
    private static int passed;

    private static GameTime Step(double seconds) =>
        new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(seconds));

    public static void Run()
    {
        CheckReinforcedCrate();
        CheckReinforcedLayout();
        CheckBazookaBarrel();
        Console.WriteLine($"{passed} crate-drop checks passed.");
    }

    private static void Check(string name, bool condition)
    {
        if (!condition) throw new InvalidOperationException("FAIL: " + name);
        passed++;
        Console.WriteLine("PASS: " + name);
    }

    private static void CheckBazookaBarrel()
    {
        LevelParseResult parsed = LevelParser.Parse(new[] { "name: barrel", "[tiles]", "P...j", "GGGGG" });
        Check("red barrel parses as a bazooka call container",
            parsed.Success && parsed.Data.CrateDrops.Single().Kind == ItemKind.BazookaCall);
        var progress = new RoomProgress();
        var level = new Level(parsed.Data, roomProgress: progress);
        SupplyCrate barrel = level.Blocks.OfType<SupplyCrate>().Single();
        var shot = new EnergyProjectile(barrel.Position - new Vector2(48, 0), new Vector2(520, 0),
            false, false, new StubSprite());
        shot.Update(Step(0.2));
        var collisions = new CollisionManager(960, 540);
        collisions.BuildTree(new List<IGameObject> { barrel, shot });
        collisions.DetectCollision();
        level.Update(Step(0.01));
        Check("shooting the barrel releases exactly one call", barrel.IsDestroyed && level.Items.Count == 1);
        var player = new Player(barrel.Position, level.Blocks, new ProjectileManager());
        Check("player collects the call and cannot carry a duplicate",
            level.CollectItems(player).Count == 1 && !player.TryAddItem(ItemKind.BazookaCall));
        var revisit = new Level(parsed.Data, roomProgress: progress);
        Check("collected barrel call stays gone after revisiting",
            revisit.Items.Count == 0 && revisit.Blocks.OfType<SupplyCrate>().Single().IsDestroyed);
    }

    private static void CheckReinforcedCrate()
    {
        LevelData data = LevelParser.Parse(new[] { "[tiles]", "....P.R", "GGGGGGG" }).Data;
        Level level = new Level(data);
        SupplyCrate crate = level.Blocks.OfType<SupplyCrate>().Single();
        var bomb = new Bomb(crate.Position, new StubSprite(), gravity: 0, createExplosion: () => new StubSprite());
        bomb.Update(Step(1.2));
        var collisions = new CollisionManager(960, 540);
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
        Check("fixed crate rewards one Energon", player.Inventory.EnergonCount == 1);

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

        LevelData lowCeilingData = LevelParser.Parse(new[]
        {
            "[tiles]",
            "....W...",
            "........",
            "........",
            "P.GGGGGG"
        }).Data;
        level = new Level(lowCeilingData);
        player = new Player(new Vector2(128, 472), level.Blocks, new ProjectileManager());
        player.Transform();
        player.Position = new Vector2(128, 472);
        Vector2 originalPosition = player.Position;
        player.Transform();
        Check("low ceiling prevents transformation without changing position", player.IsInBallForm
            && player.Position == originalPosition);
        player.Position = new Vector2(0, player.Position.Y);
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
