using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TransformersGame.Interfaces;

namespace TransformersGame.Collisions
{
    internal readonly record struct SelfieMarks(Point PlayerFeet, Point ClickFeet, Point ClickStart);

    // Finds a clear, supported stretch for the visual actors without moving any game object.
    internal sealed class SelfieStage
    {
        private const int SearchStep = 8;
        private const int ClickSpacing = 65;
        private const int PreferredRunDistance = 224;
        private const int MinimumRunDistance = 56;
        private const int RunDistanceStep = 24;
        private readonly List<Rectangle> solid = new();
        private readonly List<Rectangle> obstacles = new();
        private readonly HashSet<int> floorRows = new();
        private readonly Rectangle viewport;

        public SelfieStage(IReadOnlyList<IBlock> blocks, Rectangle viewport)
        {
            this.viewport = viewport;
            foreach (IBlock block in blocks)
            {
                if (BlockCollisionHelper.TryGetBounds(block, out Rectangle bounds)) obstacles.Add(bounds);
                if (!block.IsSolid) continue;

                solid.Add(block.Bounds);
                if (block.Bounds.Top > viewport.Top && block.Bounds.Top <= viewport.Bottom)
                {
                    floorRows.Add(block.Bounds.Top);
                }
            }
            solid.Sort((first, second) => first.Left.CompareTo(second.Left));
        }

        public bool TryFind(Point origin, Rectangle playerSpace, Rectangle clickSpace, out SelfieMarks marks)
        {
            marks = default;
            float bestDistance = float.PositiveInfinity;
            foreach (Point feet in CandidateFeet(origin.X))
            {
                float distance = Vector2.DistanceSquared(feet.ToVector2(), origin.ToVector2());
                if (distance >= bestDistance || !IsClearAndSupported(At(playerSpace, feet))) continue;

                int preferredSide = feet.X < viewport.Center.X ? 1 : -1;
                if (TryRunway(feet, clickSpace, preferredSide, out SelfieMarks candidate))
                {
                    marks = candidate;
                    bestDistance = distance;
                }
            }
            return !float.IsPositiveInfinity(bestDistance);
        }

        private bool TryRunway(Point feet, Rectangle space, int preferredSide, out SelfieMarks marks)
        {
            for (int distance = PreferredRunDistance; distance >= MinimumRunDistance; distance -= RunDistanceStep)
            {
                if (TryWalkway(feet, space, preferredSide, distance, out marks)
                    || TryWalkway(feet, space, -preferredSide, distance, out marks)) return true;
            }
            marks = default;
            return false;
        }

        private IEnumerable<Point> CandidateFeet(int preferredX)
        {
            foreach (int y in floorRows)
            {
                yield return new Point(preferredX, y);
                for (int x = viewport.Left; x < viewport.Right; x += SearchStep)
                {
                    yield return new Point(x, y);
                }
            }
        }

        private bool TryWalkway(Point feet, Rectangle clickSpace, int side, int distance, out SelfieMarks marks)
        {
            Point clickFeet = new Point(feet.X + side * ClickSpacing, feet.Y);
            Point clickStart = new Point(clickFeet.X + side * distance, feet.Y);
            marks = new SelfieMarks(feet, clickFeet, clickStart);
            Rectangle walkway = Rectangle.Union(At(clickSpace, clickFeet), At(clickSpace, clickStart));
            return IsClearAndSupported(walkway);
        }

        private bool IsClearAndSupported(Rectangle area)
        {
            if (!viewport.Contains(area) || obstacles.Exists(area.Intersects)) return false;

            int supportedTo = area.Left;
            foreach (Rectangle block in solid)
            {
                if (block.Top != area.Bottom || block.Left > supportedTo) continue;
                supportedTo = Math.Max(supportedTo, block.Right);
                if (supportedTo >= area.Right) return true;
            }
            return false;
        }

        private static Rectangle At(Rectangle area, Point feet)
        {
            area.Offset(feet);
            return area;
        }
    }
}
