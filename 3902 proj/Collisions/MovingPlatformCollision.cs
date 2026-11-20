using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Collisions;

internal static class MovingPlatformCollision
{
    public static Vector2 ResolvePlayerPosition(Vector2 position, Point size, Rectangle room,
        IReadOnlyList<IBlock> blocks, IBlock support)
    {
        if (support is MovingPlatform riderPlatform && riderPlatform.Delta != Vector2.Zero)
        {
            Vector2 carry = riderPlatform.Delta;
            if (carry.Y != 0)
            {
                carry.Y = riderPlatform.Bounds.Top - size.Y - position.Y;
            }

            position = ApplyMovement(position, size, room, blocks, riderPlatform, carry);
        }

        foreach (IBlock block in blocks)
        {
            if (block is MovingPlatform platform && platform != support && platform.Delta != Vector2.Zero
                && PlayerBounds(position, size).Intersects(platform.Bounds))
            {
                position = ApplyMovement(position, size, room, blocks, platform, PushDistance(position, size, platform));
            }
        }

        return position;
    }

    private static Vector2 ApplyMovement(Vector2 position, Point size, Rectangle room,
        IReadOnlyList<IBlock> blocks, MovingPlatform platform, Vector2 distance)
    {
        Vector2 moved = BlockCollisionOperations.MoveHorizontally(position, size.X, size.Y,
            distance.X, room.Left, room.Right - size.X, blocks);
        moved = BlockCollisionOperations.MoveVertically(moved, size.X, size.Y,
            distance.Y, room.Top, room.Bottom - size.Y, blocks);

        if (PlayerBounds(moved, size).Intersects(platform.Bounds))
        {
            platform.UndoMovement();
            return position;
        }

        return moved;
    }

    private static Vector2 PushDistance(Vector2 position, Point size, MovingPlatform platform)
    {
        Rectangle bounds = platform.Bounds;
        if (platform.Delta.X > 0)
        {
            return new Vector2(bounds.Right - position.X, 0);
        }
        if (platform.Delta.X < 0)
        {
            return new Vector2(bounds.Left - size.X - position.X, 0);
        }
        if (platform.Delta.Y > 0)
        {
            return new Vector2(0, bounds.Bottom - position.Y);
        }

        return new Vector2(0, bounds.Top - size.Y - position.Y);
    }

    private static Rectangle PlayerBounds(Vector2 position, Point size) =>
        new Rectangle((int)position.X, (int)position.Y, size.X, size.Y);
}
