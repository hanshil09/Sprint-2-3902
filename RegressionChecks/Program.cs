using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TransformersGame.Collisions;
using TransformersGame.Controllers;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

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
    Check("exactly two enemies " + path, result.Data.Enemies.Count == 2);
    Check("one focused pickup " + path, result.Data.Items.Count == 1);
    Check("one elevated forward door " + path, result.Data.Doors.Count == 1 && result.Data.Doors[0].Kind == 1);
    Check("exit requires parkour " + path, result.Data.Doors.Single(d => d.Kind == 1).Row < result.Data.PlayerRow);
    Check("physics route reaches exit " + path, ParkourCheck.CanReachExit(result.Data));
}
Check("six distinct rooms", roomFiles.Length == 6 && roomData.Select(d => d.Name).Distinct().Count() == 6);
Check("reject missing spawn", !LevelParser.Parse(new[] { "[tiles]", "GGG" }).Success);
Check("reject duplicate spawn", !LevelParser.Parse(new[] { "[tiles]", "PP" }).Success);
var wall = new Block(new Vector2(100, 100), new StubSprite(), true);
var physics = new Physics(new[] { wall });
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
Check("seven enemy kinds across rooms", roomData.SelectMany(d => d.Enemies).Select(e => e.Kind).Distinct().Count() == 7);
Check("four pickup kinds across rooms", roomData.SelectMany(d => d.Items).Select(e => e.Kind).Distinct().Count() == 4);
Check("block variety across rooms", roomData.SelectMany(d => d.Blocks).Select(b => b.Kind).Distinct().Count() == 6);
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
Check("one door per room parsed", roomData.All(d => d.Doors.Count == 1));
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
resource.Collect(testPlayer); resource.Collect(testPlayer);
Check("resource pickup is applied once", testPlayer.ItemsAdded == 1);
Console.WriteLine($"{passed} regression checks passed.");

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
    public int DamageCount { get; private set; }
    public int ItemsAdded { get; private set; }
    public bool TakeDamage() { DamageCount++; return true; }
    public void AddItem(ItemKind kind) => ItemsAdded++;
    public bool UseMedkit() => false;
    public bool UseShield() => false;
    public void Move(Direction direction) { }
    public void Jump() { }
    public void Shoot(bool angled) { }
    public void UseItem(int itemNumber) { }
    public void Transform() { }
    public void EnterRoom(IReadOnlyList<IBlock> blocks, IProjectileManager projectiles, Vector2 position) => Position = position;
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
