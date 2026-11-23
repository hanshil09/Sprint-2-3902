using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities
{
    internal sealed class BoomerangProjectile : IPlayerWeapon
    {
        private const float LaunchSpeed = 560f;
        private const float Deceleration = 900f;
        private const float ReturnSpeed = 520f;
        private const float CatchDistance = 28f;
        private const double MaximumLifetime = 4000;
        private const int Damage = 3;

        private readonly Func<Vector2> ownerCenter;
        private readonly ISprite sprite;
        private readonly HashSet<IEnemy> hitThisPass = new HashSet<IEnemy>();
        // Unit vector the boomerang flies out along (one of the eight 45-degree directions).
        private readonly Vector2 direction;
        private float outwardSpeed;
        private double lifetime;

        public BoomerangProjectile(Vector2 position, Vector2 direction, Func<Vector2> ownerCenter, ISprite sprite)
        {
            Position = position;
            PreviousPosition = position;
            this.direction = direction.LengthSquared() > float.Epsilon ? Vector2.Normalize(direction) : new Vector2(1f, 0f);
            this.ownerCenter = ownerCenter;
            this.sprite = sprite;
            outwardSpeed = LaunchSpeed;
            IsActive = true;
        }

        public Vector2 Position { get; set; }

        public Vector2 PreviousPosition { get; private set; }

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);

        public bool IsActive { get; private set; }

        public bool IsReturning { get; private set; }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            PreviousPosition = Position;
            lifetime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (IsReturning)
            {
                FlyHome(elapsed);
            }
            else
            {
                FlyOut(elapsed);
            }
            IsActive &= lifetime < MaximumLifetime;
            sprite.Update(gameTime);
        }

        public void HitEnemy(IEnemy enemy)
        {
            if (!enemy.IsDestroyed && hitThisPass.Add(enemy))
            {
                enemy.TakeDamage(Damage);
            }
        }

        // Walls send it back early; on the way home it passes through blocks so it can always return.
        public void HitSolid(IBlock block)
        {
            if (!IsReturning)
            {
                StartReturn();
            }
        }

        public void Deactivate() => IsActive = false;

        public void Draw(SpriteBatch spriteBatch) => sprite.Draw(spriteBatch, Position);

        private void FlyOut(float elapsed)
        {
            Position += direction * outwardSpeed * elapsed;
            outwardSpeed -= Deceleration * elapsed;
            if (outwardSpeed <= 0)
            {
                StartReturn();
            }
        }

        private void FlyHome(float elapsed)
        {
            Vector2 center = Position + new Vector2(sprite.Width / 2f, sprite.Height / 2f);
            Vector2 toOwner = ownerCenter() - center;
            float distance = toOwner.Length();
            if (distance <= CatchDistance)
            {
                Deactivate();
                return;
            }
            Position += toOwner / distance * Math.Min(distance, ReturnSpeed * elapsed);
        }

        private void StartReturn()
        {
            IsReturning = true;
            hitThisPass.Clear();
        }
    }
}
