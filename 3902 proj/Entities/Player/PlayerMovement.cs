using Microsoft.Xna.Framework;
using ArmadilloGame.Collisions;
using ArmadilloGame.Core;

namespace ArmadilloGame.Entities;

internal sealed class PlayerMovement
{
    private const float JumpSpeed = 650f;
    private const float KnockbackSpeed = 250f;
    // Walking opposite the way the player faces (backpedaling) is slower than walking forward.
    private const float BackpedalSpeedScale = 0.55f;

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

    // Where the player looks (driven by aim); controls the sprite flip.
    public Direction Facing { get; private set; } = Direction.Right;

    // Where the player is walking this frame (driven by input); controls travel.
    public Direction MoveDirection { get; private set; } = Direction.Right;

    public bool IsMoving { get; private set; }
    public float Gravity => physics.Gravity;

    // Records the walk input for this frame. Returns true when the animation-relevant state changed
    // so Player can refresh its sprite before collision checks.
    public bool Move(Direction direction)
    {
        bool horizontal = direction == Direction.Left || direction == Direction.Right;
        bool changed = IsMoving != horizontal || (horizontal && MoveDirection != direction);
        movedThisFrame = true;
        if (horizontal)
        {
            MoveDirection = direction;
        }
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
            float direction = MoveDirection == Direction.Left ? -1f : 1f;
            // Slower when walking against the facing direction (backpedaling).
            float facingScale = MoveDirection == Facing ? 1f : BackpedalSpeedScale;
            distance = direction * speed * speedScale * facingScale * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        collisions.ResolveFrameMovement(player, player.Width, player.Height, physics, gameTime, distance);
    }

    public bool TryJump() => collisions.TryGroundedJump(player, physics, JumpSpeed);
    public void Bounce(float speed) => collisions.Launch(player, physics, speed);
    public void ApplyKnockback() => collisions.TryGroundedJump(player, physics, KnockbackSpeed);
    public void SetGravity(float gravity) => physics.SetGravity(gravity);

    // Turns the player to face a horizontal direction without moving (used to face the aim).
    public bool FaceToward(Direction direction)
    {
        if (direction != Direction.Left && direction != Direction.Right)
        {
            return false;
        }
        if (Facing == direction)
        {
            return false;
        }
        Facing = direction;
        return true;
    }
}
