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
foreach (string path in Directory.GetFiles("3902 proj/Content/Levels", "*.room"))
{
    LevelParseResult result = LevelParser.Parse(File.ReadAllLines(path));
    Check("valid room " + path, result.Success);
    Check("all enemy types " + path, result.Data.Enemies.Select(e => e.Kind).Distinct().Count() == 5);
    Check("both pickup types " + path, result.Data.Items.Count == 2);
}
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
Console.WriteLine($"{passed} regression checks passed.");

sealed class CounterCommand : ICommand { public int Count { get; private set; } public void Execute() => Count++; }
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
