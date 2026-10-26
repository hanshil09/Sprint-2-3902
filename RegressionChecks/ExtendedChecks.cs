using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TransformersGame.Collisions;
using TransformersGame.Controllers;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

internal static class ExtendedChecks
{
    public static void Run()
    {
        int passed = 0;
        void Check(string name, bool condition)
        {
            if (!condition) throw new InvalidOperationException("FAIL: " + name);
            Console.WriteLine("PASS: " + name);
            passed++;
        }
        GameTime Step(double seconds) => new(TimeSpan.Zero, TimeSpan.FromSeconds(seconds));
        var roomFiles = Directory.GetFiles("3902 proj/Content/Levels", "*.room");
        var roomData = new List<LevelData>();
        foreach (string path in roomFiles)
        {
            LevelParseResult result = LevelParser.Parse(File.ReadAllLines(path));
            Check("valid room " + path, result.Success);
            roomData.Add(result.Data);
            BlockKind expectedFloor = result.Data.Name switch
            {
                "Ground Courtyard" => BlockKind.Ground,
                "Brick Stairway" => BlockKind.Brick,
                "Stone Switchbacks" => BlockKind.Stone,
                "Metal Towers" => BlockKind.Metal,
                "Crate Workshop" => BlockKind.Wood,
                "Hazard Crossing" => BlockKind.Caution,
                "Decepticon Fortress" => BlockKind.Alloy,
                _ => throw new InvalidOperationException("Unexpected room theme")
            };
            var baseFloor = result.Data.Blocks.Where(b => b.Row == result.Data.Rows - 1).ToArray();
            Check("complete themed base floor " + path, baseFloor.Length == 30 && baseFloor.All(b => b.Kind == expectedFloor));
            var platforms = result.Data.Blocks.Where(b => b.Row < result.Data.Rows - 1 && b.Kind != BlockKind.Crate && b.Kind != BlockKind.Hazard).ToArray();
            Check("parkour blocks match base floor " + path, platforms.Length > 0 && platforms.All(b => b.Kind == expectedFloor));
            var floorPhysics = new MovementProbe(new[] { new Block(new Vector2(100, 508), new StubSprite(), true, expectedFloor) }, result.Data.Gravity);
            Check("themed floor supports falling objects " + path,
                floorPhysics.Apply(new Vector2(100, 400), 10, 10, Step(.5)).Y == 498 && floorPhysics.IsOnGround);
            Check("exactly two enemies " + path, result.Data.Enemies.Count == 2);
            Check("one to three focused pickups " + path, result.Data.Items.Count is >= 1 and <= 3);
            bool hasForwardDoor = result.Data.Doors.Count(d => d.Kind == 1) == 1;
            bool hasGoal = result.Data.Objects.Count(o => o.Kind == ObjectKind.Goal) == 1;
            Check("return door plus one exit (forward door or beacon) " + path,
                result.Data.Doors.Count(d => d.Kind == -1) == 1 && hasForwardDoor != hasGoal);
            Check("exit requires parkour " + path, ParkourCheck.ExitRow(result.Data) < result.Data.PlayerRow);
            Check("physics route reaches exit " + path, ParkourCheck.CanReachExit(result.Data));
        }
        Check("seven distinct rooms", roomFiles.Length == 7 && roomData.Select(d => d.Name).Distinct().Count() == 7);
        Check("each room has two different enemy types", roomData.All(d => d.Enemies.Select(e => e.Kind).Distinct().Count() == 2));
        Check("no two rooms share an enemy pair", roomData.Select(d => string.Join(",", d.Enemies.Select(e => e.Kind).Order())).Distinct().Count() == 7);
        Check("seven distinct room block themes", roomData.Select(d => d.Blocks.Single(b => b.Row == d.Rows - 1 && b.Column == 0).Kind).Distinct().Count() == 7);
        Check("exactly one room has the boss and the beacon", roomData.Count(d => d.Enemies.Any(e => e.Kind == EnemyKind.Warlord)) == 1
            && roomData.Single(d => d.Enemies.Any(e => e.Kind == EnemyKind.Warlord)).Objects.Any(o => o.Kind == ObjectKind.Goal));
        Check("reject missing spawn", !LevelParser.Parse(new[] { "[tiles]", "GGG" }).Success);
        Check("reject duplicate spawn", !LevelParser.Parse(new[] { "[tiles]", "PP" }).Success);
        var wall = new Block(new Vector2(100, 100), new StubSprite(), true);
        var physics = new MovementProbe(new[] { wall });
        Check("horizontal wall crossing", physics.MoveHorizontally(new Vector2(70, 100), 10, 10, 100, 0, 960).X == 90);
        Check("upward ceiling crossing", physics.MoveVertically(new Vector2(100, 130), 10, 10, -100, 0, 540).Y == 110);
        Check("landing", physics.Apply(new Vector2(100, 80), 10, 10, Step(.2)).Y == 90 && physics.IsOnGround);
        var enemy = new StubEnemy { Position = new Vector2(100, 100) };
        var shot = new EnergyProjectile(new Vector2(0, 100), new Vector2(1000, 0), false, false, new StubSprite());
        shot.Update(Step(.2));
        var collisions = new CollisionManager(960, 540);
        collisions.BuildTree(new List<IGameObject> { shot, enemy });
        collisions.DetectCollision();
        Check("swept shot hits crossed enemy", enemy.Hits == 1 && !shot.IsActive);
        shot = new EnergyProjectile(new Vector2(80, 100), new Vector2(100, 0), false, false, new StubSprite());
        shot.Update(Step(.3));
        collisions.BuildTree(new List<IGameObject> { wall, shot });
        collisions.DetectCollision();
        Check("shot stops at wall", !shot.IsActive);
        shot = new EnergyProjectile(new Vector2(950, 20), new Vector2(100, 0), false, false, new StubSprite());
        shot.Update(Step(.3));
        collisions.BuildTree(new List<IGameObject> { shot });
        collisions.DetectCollision();
        Check("immediate boundary cleanup", !shot.IsActive);
        var previous = new CounterCommand();
        var next = new CounterCommand();
        MouseState mouse = default;
        var controller = new MouseController(previous, next, () => mouse);
        MouseState Buttons(ButtonState left, ButtonState right) => new(0, 0, 0, left, ButtonState.Released, right, ButtonState.Released, ButtonState.Released);
        mouse = Buttons(ButtonState.Pressed, ButtonState.Released);
        controller.Update(); controller.Update();
        Check("hold left triggers once", previous.Count == 1 && next.Count == 0);
        mouse = default; controller.Update();
        mouse = Buttons(ButtonState.Released, ButtonState.Pressed);
        controller.Update(); controller.Update();
        Check("hold right triggers once", next.Count == 1);
        var rebuilt = new MouseController(previous, next, () => mouse);
        rebuilt.Update();
        Check("controller rebuild does not repeat held click", next.Count == 1);
        Check("every enemy kind appears in a room", roomData.SelectMany(d => d.Enemies).Select(e => e.Kind).Distinct().Count() == Enum.GetValues<EnemyKind>().Length);
        Check("every pickup kind appears in a room", roomData.SelectMany(d => d.Items).Select(e => e.Kind).Distinct().Count() == Enum.GetValues<ItemKind>().Length);
        Check("every interactive object kind appears in a room", roomData.SelectMany(d => d.Objects).Select(o => o.Kind).Distinct().Count() == Enum.GetValues<ObjectKind>().Length);
        Check("block variety across rooms", roomData.SelectMany(d => d.Blocks).Select(b => b.Kind).Distinct().Order().SequenceEqual(Enum.GetValues<BlockKind>().Order()));
        var crate = new Block(new Vector2(100, 100), new StubSprite(), true, BlockKind.Crate);
        shot = new EnergyProjectile(new Vector2(80, 100), new Vector2(100, 0), false, false, new StubSprite());
        shot.Update(Step(.3));
        collisions.BuildTree(new List<IGameObject> { crate, shot });
        collisions.DetectCollision();
        Check("crate breaks on shot impact", crate.IsDestroyed && !crate.IsSolid && !shot.IsActive);
        var piercingShot = new PiercingProjectile(new Vector2(100, 100), Vector2.Zero, new StubSprite());
        var first = new StubEnemy { Position = new Vector2(100, 100) };
        var second = new StubEnemy { Position = new Vector2(100, 100) };
        collisions.BuildTree(new List<IGameObject> { piercingShot, first, second });
        collisions.DetectCollision();
        Check("piercing shot hits multiple enemies", first.Hits == 1 && second.Hits == 1 && piercingShot.IsActive);
        collisions.DetectCollision();
        Check("piercing shot does not repeat damage", first.Hits == 1 && second.Hits == 1);
        wall = new Block(new Vector2(100, 100), new StubSprite(), true);
        enemy = new StubEnemy { Position = new Vector2(150, 100) };
        shot = new EnergyProjectile(new Vector2(0, 100), new Vector2(1000, 0), false, false, new StubSprite());
        shot.Update(Step(.2));
        collisions.BuildTree(new List<IGameObject> { shot, enemy, wall });
        collisions.DetectCollision();
        Check("nearest wall shields enemy independent of object order", !shot.IsActive && enemy.Hits == 0);
        Check("room doors parsed", roomData.All(d => d.Doors.Count == (d.Objects.Any(o => o.Kind == ObjectKind.Goal) ? 1 : 2)));
        var testPlayer = new StubPlayer { Position = new Vector2(100, 100) };
        int doorDirection = 0;
        collisions = new CollisionManager(960, 540, null, direction => doorDirection = direction);
        collisions.BuildTree(new List<IGameObject> { testPlayer, new Door(new Vector2(100, 100), -1, new StubSprite()) });
        collisions.DetectCollision();
        Check("door overlap requests previous room", doorDirection == -1);
        var enemyShot = new EnemyProjectile(new Vector2(105, 105), new Vector2(105, 105), new StubSprite());
        collisions.BuildTree(new List<IGameObject> { testPlayer, enemyShot });
        collisions.DetectCollision();
        Check("enemy projectile damages and deactivates", testPlayer.DamageCount == 1 && !enemyShot.IsActive);
        var hazard = new Block(new Vector2(100, 100), new StubSprite(), false, BlockKind.Hazard);
        collisions.BuildTree(new List<IGameObject> { hazard, testPlayer });
        collisions.DetectCollision();
        Check("non-solid hazard damages player", testPlayer.DamageCount == 2 && !hazard.IsSolid);
        var resource = new ResourcePickup(Vector2.Zero, new StubSprite(), ItemKind.HealthCell);
        resource.TryCollect(testPlayer); resource.TryCollect(testPlayer);
        Check("resource pickup is applied once", testPlayer.ItemsAdded == 1);
        Check("seven distinct room gravity settings", roomData.Select(d => d.Gravity).Distinct().Count() == 7);
        Check("legacy rooms retain default gravity", LevelParser.Parse(new[] { "[tiles]", "P" }).Data.Gravity == Physics.DefaultGravity);
        foreach (string invalidGravity in new[] { "NaN", "Infinity", "-1", "0", "2201", "heavy" })
            Check("reject invalid gravity " + invalidGravity,
                !LevelParser.Parse(new[] { "gravity: " + invalidGravity, "[tiles]", "P" }).Success);
        var lightBomb = new Bomb(Vector2.Zero, new StubSprite(), gravity: 1200);
        var heavyBomb = new Bomb(Vector2.Zero, new StubSprite(), gravity: 1550);
        lightBomb.Update(Step(.1)); heavyBomb.Update(Step(.1));
        Check("bomb falls faster in heavier gravity", heavyBomb.Position.Y > lightBomb.Position.Y && lightBomb.Position.Y > 0);
        Check("bomb records previous position", lightBomb.PreviousPosition == Vector2.Zero);
        foreach (float gravity in roomData.Select(d => d.Gravity))
        {
            var floor = new Block(new Vector2(100, 508), new StubSprite(), true);
            var fallingBomb = new Bomb(new Vector2(100, 400), new StubSprite(), new[] { floor }, gravity);
            for (int frame = 0; frame < 60; frame++) fallingBomb.Update(Step(1.0 / 60));
            Check("bomb rests on base floor at gravity " + gravity, fallingBomb.Position.Y == 498 && fallingBomb.IsActive && !fallingBomb.IsExploding);
            fallingBomb.Update(Step(.05));
            Check("bomb does not jitter on floor at gravity " + gravity, fallingBomb.Position.Y == 498);
        }
        var platform = new Block(new Vector2(100, 200), new StubSprite(), true);
        var platformBomb = new Bomb(new Vector2(100, 100), new StubSprite(), new[] { platform });
        platformBomb.Update(Step(.5));
        Check("bomb lands on elevated platform without tunneling", platformBomb.Position.Y == 190);
        var crateSupport = new Block(new Vector2(100, 200), new StubSprite(), true, BlockKind.Crate);
        var crateBomb = new Bomb(new Vector2(100, 100), new StubSprite(), new[] { crateSupport });
        crateBomb.Update(Step(.5));
        Check("bomb rests on breakable crate", crateBomb.Position.Y == 190);
        crateSupport.Destroy();
        crateBomb.Update(Step(.1));
        Check("bomb falls when supporting crate is destroyed", crateBomb.Position.Y > 190);
        var hazardSupport = new Block(new Vector2(100, 200), new StubSprite(), false, BlockKind.Hazard);
        var hazardBomb = new Bomb(new Vector2(100, 100), new StubSprite(), new[] { hazardSupport });
        hazardBomb.Update(Step(.5));
        Check("non-solid hazard does not suspend bomb", hazardBomb.Position.Y > 210);
        var timedBomb = new Bomb(new Vector2(100, 100), new StubSprite(), new[] { platform }, createExplosion: () => new StubSprite());
        timedBomb.Update(Step(1.2));
        Check("bomb still explodes after original fuse", timedBomb.IsExploding && timedBomb.Position.Y == 190);
        timedBomb.Update(Step(.1));
        Check("explosion stays on supporting surface", timedBomb.Position.Y == 190 && timedBomb.IsExploding);
        var bombTarget = new StubEnemy();
        timedBomb.DamageEnemy(bombTarget); timedBomb.DamageEnemy(bombTarget);
        Check("explosion damages an enemy only once", bombTarget.Hits == 1);
        timedBomb.Update(Step(.23));
        Check("explosion expires", !timedBomb.IsActive);
        Vector2 finishedPosition = timedBomb.Position;
        timedBomb.Update(Step(1));
        Check("inactive bomb does not move", timedBomb.Position == finishedPosition);
        var ledge = new Block(new Vector2(100, 200), new StubSprite(), true);
        var edgePhysics = new MovementProbe(new[] { ledge }) { FootInset = CollisionManager.PlayerFootInset };
        Vector2 onlyEdgeOver = edgePhysics.Apply(new Vector2(100 - 68 + Player.FootInset - 2, 150), 68, 50, Step(.2));
        Check("player falls when only the empty edge of its box is over a ledge", !edgePhysics.IsOnGround && onlyEdgeOver.Y > 150);
        edgePhysics = new MovementProbe(new[] { ledge }) { FootInset = CollisionManager.PlayerFootInset };
        edgePhysics.Apply(new Vector2(100 - 68 + Player.FootInset + 2, 149.5f), 68, 50, Step(.2));
        Check("player stands when its feet are over a ledge", edgePhysics.IsOnGround);
        var slider = new MovingPlatform(new Vector2(200, 400), 3, new Vector2(100, 0), 60f, new StubSprite());
        var pushedPlayer = new Player(new Vector2(225, 380), new IBlock[] { slider }, new ProjectileManager());
        slider.Update(Step(1.0 / 60));
        pushedPlayer.Update(Step(1.0 / 60));
        Check("moving platform pushes the player aside instead of overlapping", pushedPlayer.Position.X >= slider.Bounds.Right);
        var overhead = new MovingPlatform(new Vector2(200, 300), 3, new Vector2(100, 0), 60f, new StubSprite());
        var jumper = new MovementProbe(new IBlock[] { overhead });
        jumper.Apply(new Vector2(205, 330), 10, 10, Step(1.0 / 60));
        jumper.Launch(600);
        Vector2 jumped = jumper.Apply(new Vector2(205, 330), 10, 10, Step(.1));
        Check("jumping into a moving platform from below is stopped by it", jumped.Y >= overhead.Bounds.Bottom);
        Console.WriteLine($"{passed} regression checks passed.");
    }
}

sealed class CounterCommand : ICommand { public int Count { get; private set; } public void Execute() => Count++; }
sealed class StubPlayer : IPlayer
{
    public Vector2 Position { get; set; }
    public int Width => 10;
    public int Height => 10;
    public Direction Facing => Direction.Right;
    public int Health => 100;
    public int MaxHealth => 100;
    public int MedkitCount => 0;
    public int ShieldCount => 0;
    public bool IsShieldActive => false;
    public int BombLevel => 0;
    public int EnergonCount => 0;
    public bool IsInvincible { get; set; }
    public float SpeedScale { get; set; } = 1f;
    public float LastBounce { get; private set; }
    public void Bounce(float speed) => LastBounce = speed;
    public int DamageCount { get; private set; }
    public int ItemsAdded { get; private set; }
    public bool TakeDamage() { DamageCount++; return true; }
    public bool TryAddItem(ItemKind kind) { ItemsAdded++; return true; }
    public bool UseMedkit() => false;
    public bool UseShield() => false;
    public void Move(Direction direction) { }
    public void Jump() { }
    public void Shoot(bool angled) { }
    public void UseItem(int itemNumber) { }
    public void Transform() { }
    public void EnterRoom(IReadOnlyList<IBlock> blocks, IProjectileManager projectiles, Vector2 position, float gravity = Physics.DefaultGravity) => Position = position;
    public void Update(GameTime gameTime) { }
    public void Draw(SpriteBatch spriteBatch) { }
}
sealed class StubSprite : ISprite
{
    public int Width => 10;
    public int Height => 10;
    public void Update(GameTime time) { }
    public void Draw(SpriteBatch batch, Vector2 position) { }
}
sealed class StubEnemy : IEnemy
{
    public Vector2 Position { get; set; }
    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, 10, 10);
    public bool IsDestroyed => Hits > 0;
    public int Hits { get; private set; }
    public void TakeDamage(int amount) => Hits += amount;
    public void Update(GameTime time) { }
    public void Draw(SpriteBatch batch) { }
}
