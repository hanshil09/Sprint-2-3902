using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ArmadilloGame.Core;

internal static class LevelSpawnSearch
{
    // Nearest free position to the requested one, searched on a 16-pixel grid.
    public static Vector2 Find(Vector2 requested, int width, int height, Rectangle roomBounds,
        IReadOnlyList<Rectangle> occupied)
    {
        const int searchStep = 16;
        float bestDistance = float.MaxValue;
        Vector2? best = null;
        for (int y = roomBounds.Top; y <= roomBounds.Bottom - height; y += searchStep)
        {
            for (int x = roomBounds.Left; x <= roomBounds.Right - width; x += searchStep)
            {
                Rectangle candidate = new Rectangle(x, y, width, height);
                float distance = Vector2.DistanceSquared(new Vector2(x, y), requested);
                if (distance < bestDistance && !Overlaps(candidate, occupied))
                {
                    bestDistance = distance;
                    best = new Vector2(x, y);
                }
            }
        }
        if (!best.HasValue)
        {
            throw new InvalidOperationException("The level has no non-overlapping spawn position for an entity.");
        }

        return best.Value;
    }

    private static bool Overlaps(Rectangle candidate, IReadOnlyList<Rectangle> occupied)
    {
        foreach (Rectangle bounds in occupied)
        {
            if (candidate.Intersects(bounds))
            {
                return true;
            }
        }

        return false;
    }
}
