using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Collisions;
using TransformersGame.Core;
using TransformersGame.Interfaces;

// Headless body using the same Physics + CollisionManager split as Player and Bomb.
internal sealed class MovementProbe : IGameObject
{
    private readonly Physics physics;
    private readonly CollisionManager collisions = new(960, 540);
    private readonly IReadOnlyList<IBlock> blocks;
    private float footInset;

    public MovementProbe(IReadOnlyList<IBlock> blocks, float gravity = Physics.DefaultGravity)
    {
        this.blocks = blocks;
        physics = new Physics(gravity);
        collisions.SetBlocks(this, blocks);
    }

    public Vector2 Position { get; set; }
    public bool IsOnGround => collisions.IsOnGround(this);
    public float FootInset
    {
        get => footInset;
        set
        {
            footInset = value;
            collisions.SetBlocks(this, blocks, value);
        }
    }

    public Vector2 Apply(Vector2 position, int width, int height, GameTime time)
    {
        Position = collisions.UpdateVerticalMovement(this, physics, position, width, height, time);
        return Position;
    }

    public void Jump(float speed) => collisions.TryJump(this, physics, speed);
    public void Launch(float speed) => collisions.Launch(this, physics, speed);

    public Vector2 MoveHorizontally(Vector2 position, int width, int height, float distance, float minimum, float maximum) =>
        collisions.MoveHorizontally(this, position, width, height, distance, minimum, maximum);

    public Vector2 MoveVertically(Vector2 position, int width, int height, float distance, float minimum, float maximum) =>
        collisions.MoveVertically(this, position, width, height, distance, minimum, maximum);

    public void Update(GameTime gameTime) { }
    public void Draw(SpriteBatch spriteBatch) { }
}
