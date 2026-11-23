using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities
{
    internal class EnergyProjectile : IPlayerWeapon
    {
        private const double MaximumLifetime = 3000;
        private const double angleValue = 0.6632;       //38 degrees, in radians

        private ISprite sprite;
        private Vector2 velocity;
        private double lifetime;
        private bool isAngled;
        private readonly int damage;
        // Travel limit in pixels; 0 or less means unlimited (bounded only by lifetime/world edges).
        private readonly float maxRange;
        private float traveled;

        public EnergyProjectile(Vector2 position, Vector2 velocity, bool angled, bool isLeft, ISprite sprite,
            int damage = 1, float maxRange = 0f)
        {
            Position = position;
            PreviousPosition = position;
            isAngled = angled;
            this.sprite = sprite;
            this.damage = damage;
            this.maxRange = maxRange;
            lifetime = 0;
            IsActive = true;

            if(isAngled)
            {
                float angledShotX = (float)(velocity.X * Math.Cos(angleValue) + velocity.Y * Math.Sin(angleValue));
                float angledShotY = (float)(velocity.X * -1 * Math.Sin(angleValue) + velocity.Y * Math.Cos(angleValue));
                if(isLeft)
                {
                    angledShotY *= -1;
                }
                this.velocity = new Vector2(angledShotX, angledShotY);
            }
            else
            {
                this.velocity = velocity;
            }
        }

        public Vector2 Position { get; set; }

        public Vector2 PreviousPosition { get; private set; }

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);

        public bool IsActive { get; private set; }

        public void Update(GameTime gameTime)
        {
            PreviousPosition = Position;
            Vector2 step = velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += step;
            lifetime += gameTime.ElapsedGameTime.TotalMilliseconds;

            // Expire once the pellet has flown its maximum range (shotgun), or by lifetime otherwise.
            if (maxRange > 0f)
            {
                traveled += step.Length();
                if (traveled >= maxRange)
                {
                    IsActive = false;
                }
            }
            if (lifetime >= MaximumLifetime)
            {
                IsActive = false;
            }
            sprite.Update(gameTime);
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public virtual void HitEnemy(IEnemy enemy)
        {
            if (!enemy.IsDestroyed)
            {
                enemy.TakeDamage(damage);
            }
            Deactivate();
        }

        // Player shots break crates; every solid block stops the shot.
        public void HitSolid(IBlock block)
        {
            if (block is Block crate && crate.Kind == BlockKind.Crate)
            {
                crate.Destroy();
            }
            Deactivate();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, Position);
        }
    }
}
