using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities;

public sealed class EnergyProjectile : IProjectile
{
    private const float Speed = 420f;
    private const float MaximumLifetime = 1.5f;
    private readonly ISprite sprite;
    private readonly Vector2 velocity;
    private float lifetime;

    public EnergyProjectile(Vector2 position, Vector2 direction, ISprite sprite)
    {
        Position = position;
        velocity = direction * Speed;
        this.sprite = sprite;
        IsActive = true;
    }

    public Vector2 Position { get; set; }
    public bool IsActive { get; private set; }
    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);
    public void Deactivate() => IsActive = false;

    public void Update(GameTime gameTime)
    {
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += velocity * elapsed;
        lifetime += elapsed;
        if (lifetime >= MaximumLifetime) IsActive = false;
        sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsActive) sprite.Draw(spriteBatch, Position);
    }
}
