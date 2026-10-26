using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Collisions;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;
using TransformersGame.States;

// Searches real movement trajectories in either playable form; no combat/timed-platform simulation.
internal static class ParkourCheck
{
    public static bool CanReachExit(LevelData data)
    {
        var level = new Level(data);
        var player = new Player(level.PlayerStart, level.Blocks, new ProjectileManager(), data.Gravity);
        if (new RouteSearch(data, player.Width, player.Height, RobotState.RobotSpeed, player.Position).Run())
            return true;
        // Transform preserves the feet position and uses the current vehicle sprite and speed.
        player.Transform();
        return new RouteSearch(data, player.Width, player.Height, new VehicleState(player).Speed, player.Position).Run();
    }

    public static Rectangle ExitBounds(LevelData data) => RouteSearch.ExitBounds(data);
    public static int ExitRow(LevelData data) => RouteSearch.ExitRow(data);

    private sealed class RouteSearch
    {
        private readonly int Width;
        private readonly int Height;
        private readonly float speed;
        private readonly LevelData data;
        private readonly Vector2 initialPosition;

        public RouteSearch(LevelData data, int width, int height, float speed, Vector2 position)
        {
            this.data = data;
            Width = width;
            Height = height;
            this.speed = speed;
            initialPosition = position;
        }
        private const float JumpSpeed = 650;
        private const int ScreenWidth = 960;
        private const int ScreenHeight = 540;
        private static readonly GameTime Step = new(TimeSpan.Zero, TimeSpan.FromSeconds(1.0 / 60));

        public bool Run()
        {
            // Breakable crates are assumed cleared first; hazards remain forbidden.
            IBlock[] solids = data.Blocks.Where(p => p.Kind != BlockKind.Hazard && p.Kind != BlockKind.Crate)
                .Select(p => (IBlock)new Block(Cell(p.Column, p.Row, data.Rows), new TileSprite(), true)).ToArray();
            Rectangle[] hazards = data.Blocks.Where(p => p.Kind == BlockKind.Hazard)
                .Select(p => new Rectangle((int)Cell(p.Column, p.Row, data.Rows).X,
                    (int)Cell(p.Column, p.Row, data.Rows).Y, BlockSpriteFactory.BlockSize, BlockSpriteFactory.BlockSize)).ToArray();
            Rectangle exit = ExitBounds(data);
            Vector2 start = initialPosition;
            var falling = new MovementProbe(solids, data.Gravity) { FootInset = CollisionManager.PlayerFootInset };
            for (int frame = 0; frame < 120 && !falling.IsOnGround; frame++)
                start = falling.Apply(start, Width, Height, Step);
            if (!falling.IsOnGround || hazards.Any(h => Bounds(start).Intersects(h))) return false;

            float inset = CollisionManager.PlayerFootInset;
            Vector2[] candidates = solids.SelectMany(b => new[]
                { b.Bounds.Center.X - Width / 2f, b.Bounds.Right - inset - 1, b.Bounds.Left - Width + inset + 1 }
                .Select(x => new Vector2(Math.Clamp(x, 0, ScreenWidth - Width), b.Bounds.Top - Height)))
                .Append(start).Distinct().Where(p => p.Y >= 0
                    && !solids.Any(b => Bounds(p).Intersects(b.Bounds))
                    && !hazards.Any(h => Bounds(p).Intersects(h))).ToArray();
            var visited = new HashSet<Vector2> { start };
            var queue = new Queue<Vector2>();
            queue.Enqueue(start);
            while (queue.TryDequeue(out Vector2 from))
            {
                if (Bounds(from).Intersects(exit)) return true;
                foreach (Vector2 target in candidates)
                {
                    if (visited.Contains(target) || from.Y - target.Y > JumpSpeed * JumpSpeed / (2 * data.Gravity)
                        || Math.Abs(target.X - from.X) > 260) continue;
                    if (TryMove(from, target, false, solids, hazards, data.Gravity)
                        || TryMove(from, target, true, solids, hazards, data.Gravity)
                        || TryMove(from, target, true, solids, hazards, data.Gravity, 10)
                        || TryMove(from, target, true, solids, hazards, data.Gravity, 20))
                    {
                        visited.Add(target);
                        queue.Enqueue(target);
                    }
                }
            }
            return false;
        }

        private bool TryMove(Vector2 position, Vector2 target, bool jump, IBlock[] solids, Rectangle[] hazards, float gravity, int movementDelay = 0)
        {
            var movement = new MovementProbe(solids, gravity) { FootInset = CollisionManager.PlayerFootInset };
            position = movement.Apply(position, Width, Height, Step);
            if (jump) movement.Jump(JumpSpeed);
            float distancePerFrame = speed * (float)Step.ElapsedGameTime.TotalSeconds;
            for (int frame = 0; frame < 150; frame++)
            {
                float distance = frame < movementDelay ? 0 : Math.Clamp(target.X - position.X, -distancePerFrame, distancePerFrame);
                position = movement.MoveHorizontally(position, Width, Height, distance, 0, ScreenWidth - Width);
                position = movement.Apply(position, Width, Height, Step);
                if (position.Y > ScreenHeight || hazards.Any(h => Bounds(position).Intersects(h))) return false;
                if (movement.IsOnGround && Vector2.DistanceSquared(position, target) < 1) return true;
            }
            return false;
        }

        public static Rectangle ExitBounds(LevelData data)
        {
            var door = data.Doors.SingleOrDefault(d => d.Kind == 1);
            if (door != null)
            {
                Vector2 point = Cell(door.Column, door.Row, data.Rows);
                return new Rectangle((int)point.X, (int)point.Y, 48, 64);
            }
            var goal = data.Objects.Single(o => o.Kind == ObjectKind.Goal);
            Vector2 goalPoint = Cell(goal.Column, goal.Row, data.Rows);
            return new Rectangle((int)goalPoint.X, (int)goalPoint.Y, 32, 64);
        }

        public static int ExitRow(LevelData data) =>
            data.Doors.SingleOrDefault(d => d.Kind == 1)?.Row ?? data.Objects.Single(o => o.Kind == ObjectKind.Goal).Row;

        private static Vector2 Cell(int column, int row, int rows) =>
            new(column * BlockSpriteFactory.BlockSize, ScreenHeight - (rows - row) * BlockSpriteFactory.BlockSize);
        private Rectangle Bounds(Vector2 position) => new((int)position.X, (int)position.Y, Width, Height);

        private sealed class TileSprite : ISprite
        {
            public int Width => BlockSpriteFactory.BlockSize;
            public int Height => BlockSpriteFactory.BlockSize;
            public void Update(GameTime gameTime) { }
            public void Draw(SpriteBatch spriteBatch, Vector2 position) { }
        }

    }
}
