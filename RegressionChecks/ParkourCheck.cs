using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

// Verifies geometry with the actual movement resolver, robot size, speed, and jump impulse.
// Enemies are excluded: this checks a navigable route, not combat difficulty.
internal static class ParkourCheck
{
    private const int Width = 68;
    private const int Height = 56;
    private static readonly GameTime Step = new(TimeSpan.Zero, TimeSpan.FromSeconds(1.0 / 60));

    public static bool CanReachExit(LevelData data)
    {
        // Crates are removed after shooting them; their destruction is covered by the combat check.
        var solids = data.Blocks.Where(p => p.Kind != BlockKind.Hazard && p.Kind != BlockKind.Crate)
            .Select(p => (IBlock)new Block(Cell(p.Column, p.Row), new TileSprite(), true)).ToArray();
        var hazards = data.Blocks.Where(p => p.Kind == BlockKind.Hazard)
            .Select(p => new Rectangle(p.Column * 32, (int)Cell(p.Column, p.Row).Y, 32, 32)).ToArray();
        var exit = data.Doors.Single(d => d.Kind == 1);
        var exitBounds = new Rectangle(exit.Column * 32, (int)Cell(exit.Column, exit.Row).Y, 48, 64);
        Vector2 start = Cell(data.PlayerColumn, data.PlayerRow);
        var initialPhysics = new Physics(solids, data.Gravity);
        for (int frame = 0; frame < 120 && !initialPhysics.IsOnGround; frame++)
            start = initialPhysics.Apply(start, Width, Height, Step);
        var candidates = solids.Select(b => new Vector2(Math.Clamp(b.Bounds.Center.X - Width / 2, 0, 960 - Width), b.Bounds.Top - Height))
            .Append(start).Distinct().Where(p => p.Y >= 0 && !solids.Any(b => Bounds(p).Intersects(b.Bounds))
                && !hazards.Any(h => Bounds(p).Intersects(h))).ToArray();
        var visited = new HashSet<Vector2> { start };
        var queue = new Queue<Vector2>();
        queue.Enqueue(start);
        while (queue.TryDequeue(out Vector2 from))
        {
            if (Bounds(from).Intersects(exitBounds)) return true;
            foreach (Vector2 target in candidates)
            {
                if (visited.Contains(target) || from.Y - target.Y > 650 * 650 / (2 * data.Gravity) || Math.Abs(target.X - from.X) > 260) continue;
                if (TryMove(from, target, false, solids, hazards, data.Gravity) || TryMove(from, target, true, solids, hazards, data.Gravity))
                {
                    visited.Add(target);
                    queue.Enqueue(target);
                }
            }
        }
        return false;
    }

    private static bool TryMove(Vector2 position, Vector2 target, bool jump, IBlock[] solids, Rectangle[] hazards, float gravity)
    {
        var physics = new Physics(solids, gravity);
        position = physics.Apply(position, Width, Height, Step);
        if (jump) physics.Jump(650);
        for (int frame = 0; frame < 150; frame++)
        {
            float movement = Math.Clamp(target.X - position.X, -3, 3);
            position = physics.MoveHorizontally(position, Width, Height, movement, 0, 960 - Width);
            position = physics.Apply(position, Width, Height, Step);
            if (hazards.Any(h => Bounds(position).Intersects(h)) || position.Y > 540) return false;
            if (physics.IsOnGround && Vector2.DistanceSquared(position, target) < 1) return true;
        }
        return false;
    }

    private static Vector2 Cell(int column, int row) => new(column * 32, 540 - (17 - row) * 32);
    private static Rectangle Bounds(Vector2 position) => new((int)position.X, (int)position.Y, Width, Height);
    private sealed class TileSprite : ISprite
    {
        public int Width => 32;
        public int Height => 32;
        public void Update(GameTime gameTime) { }
        public void Draw(SpriteBatch spriteBatch, Vector2 location) { }
    }
}
