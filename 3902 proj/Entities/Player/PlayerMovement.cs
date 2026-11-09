using Microsoft.Xna.Framework;
using TransformersGame.Collisions;
using TransformersGame.Core;

namespace TransformersGame.Entities;

internal sealed class PlayerMovement
{
    private const float JumpSpeed = 650f;
    private const float KnockbackSpeed = 300f;

    private readonly Player player;
    private readonly CollisionManager collisions;
    private readonly Physics physics;
    private bool movedThisFrame;

    public PlayerMovement(Player player, CollisionManager collisions, float gravity)
    {
        this.player = player;
        this.collisions = collisions;
        physics = new Physics(gravity);
    }

    public Direction Facing { get; private set; } = Direction.Right;
    public bool IsMoving { get; private set; }
    public float Gravity => physics.Gravity;

    // Report changes so Player can refresh its sprite before collision checks.
    public bool Move(Direction direction)
    {
        bool horizontal = direction == Direction.Left || direction == Direction.Right;
        bool changed = Facing != direction || IsMoving != horizontal;
        movedThisFrame = true;
        Facing = direction;
        IsMoving = horizontal;
        return changed;
    }

    public bool BeginFrame()
    {
        bool stopped = !movedThisFrame && IsMoving;
        if (stopped)
        {
            IsMoving = false;
        }

        movedThisFrame = false;
        return stopped;
    }

    public void Update(GameTime gameTime, float speed, float speedScale)
    {
        float distance = 0f;
        if (IsMoving)
        {
            float direction = 1f;
            if (Facing == Direction.Left)
            {
                direction = -1f;
            }

            distance = direction * speed * speedScale * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        collisions.UpdatePlayerMovement(player, physics, gameTime, distance);
    }

    public bool TryJump() => collisions.TryJump(player, physics, JumpSpeed);
    public void Bounce(float speed) => collisions.Launch(player, physics, speed);
    public void ApplyKnockback() => collisions.TryJump(player, physics, KnockbackSpeed);
    public void SetGravity(float gravity) => physics.SetGravity(gravity);
}
