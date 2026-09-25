using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Factories;
using TransformersGame.Interfaces;
namespace TransformersGame.Entities;
public sealed class Player : IPlayer
{
    private const float RobotSpeed = 180f;
    private const float VehicleSpeed = 280f;
    private readonly PlaceholderSpriteFactory spriteFactory;
    private ISprite sprite;
    private Vector2 movement;
    private bool isVehicle;
    private double damageTimeRemaining;
    public Player(Vector2 position, PlaceholderSpriteFactory spriteFactory)
    {
        Position = position;
        this.spriteFactory = spriteFactory;
        Facing = Direction.Down;
        sprite = spriteFactory.CreateRobotSprite(Facing);
    }
    public Vector2 Position { get; set; }
    public Direction Facing { get; private set; }
    public int Width => sprite.Width;
    public int Height => sprite.Height;
    public void Move(Direction direction)
    {
        Facing = direction;
        movement = direction switch
        {
            Direction.Up => -Vector2.UnitY,
            Direction.Down => Vector2.UnitY,
            Direction.Left => -Vector2.UnitX,
            Direction.Right => Vector2.UnitX,
            _ => Vector2.Zero
        };
        RefreshSprite();
    }
    public void StopMoving() => movement = Vector2.Zero;
    public void Transform() { isVehicle = !isVehicle; RefreshSprite(); }
    public void TakeDamage() { damageTimeRemaining = 0.5; RefreshSprite(); }
    public void Reset(Vector2 position)
    {
        Position = position;
        Facing = Direction.Down;
        movement = Vector2.Zero;
        isVehicle = false;
        damageTimeRemaining = 0;
        RefreshSprite();
    }
    public void Update(GameTime gameTime)
    {
        Position += movement * (isVehicle ? VehicleSpeed : RobotSpeed) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (damageTimeRemaining > 0)
        {
            damageTimeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
            if (damageTimeRemaining <= 0) RefreshSprite();
        }
        sprite.Update(gameTime);
    }
    public void Draw(SpriteBatch spriteBatch) => sprite.Draw(spriteBatch, Position);
    private void RefreshSprite()
    {
        bool damaged = damageTimeRemaining > 0;
        sprite = isVehicle ? spriteFactory.CreateVehicleSprite(Facing, damaged) : spriteFactory.CreateRobotSprite(Facing, damaged);
    }
}
