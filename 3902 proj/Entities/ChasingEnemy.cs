using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities;

public sealed class ChasingEnemy : IEnemy
{
    private const float Speed = 95f;
    private readonly PlaceholderSpriteFactory spriteFactory;
    private ISprite sprite;
    private Vector2 movement;
    private double jumpClock;
    private double damageFlash;

    public ChasingEnemy(Vector2 position, PlaceholderSpriteFactory spriteFactory)
    {
        Position = position;
        this.spriteFactory = spriteFactory;
        sprite = spriteFactory.CreateEnemySprite(Vector2.UnitX);
        Health = 3;
    }

    public Vector2 Position { get; set; }
    public int Health { get; private set; }
    public bool IsDefeated => Health <= 0;
    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);

    public void Chase(Vector2 target)
    {
        if (IsDefeated) return;
        Vector2 difference = target - Position;
        movement = difference.LengthSquared() > 1f ? Vector2.Normalize(difference) : Vector2.Zero;
    }

    public void TakeDamage(int amount)
    {
        if (IsDefeated) return;
        Health = System.Math.Max(0, Health - amount);
        damageFlash = 0.16;
        sprite = IsDefeated
            ? spriteFactory.CreateDefeatedEnemySprite()
            : spriteFactory.CreateEnemySprite(movement, true);
    }

    public void Reset(Vector2 position)
    {
        Position = position;
        Health = 3;
        movement = Vector2.Zero;
        jumpClock = 0;
        damageFlash = 0;
        sprite = spriteFactory.CreateEnemySprite(Vector2.UnitX);
    }

    public void Update(GameTime gameTime)
    {
        if (IsDefeated) return;
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += movement * Speed * elapsed;
        jumpClock += elapsed;
        damageFlash -= elapsed;
        sprite = spriteFactory.CreateEnemySprite(movement, damageFlash > 0);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        float jumpOffset = IsDefeated ? 0f : (float)System.Math.Max(0, System.Math.Sin(jumpClock * 3.4)) * 12f;
        sprite.Draw(spriteBatch, Position - new Vector2(0, jumpOffset));
    }
}
