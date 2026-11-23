using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Collisions;
using ArmadilloGame.Core;
using ArmadilloGame.Factories;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities
{
    // Arcing throw that bursts into a short-lived fire pool on impact. The launch velocity comes from
    // the player's aim and charge, so holding longer throws it farther. Hitting a barrel widens the
    // burst. Enemies caught in the fire take damage once each.
    internal sealed class MolotovProjectile : IPlayerWeapon
    {
        private const double FireTime = 700;
        // Safety cap so a throw that never meets ground or a wall still bursts instead of flying off.
        private const double MaxFlightMs = 3000;
        private const int FireDamage = 15;
        private const int BaseBlastLevel = 1;
        private const int BarrelBlastLevel = 3;

        private readonly Physics physics;
        private readonly CollisionManager collisionManager;
        private readonly Action onBurst;
        private readonly HashSet<IEnemy> burned = new HashSet<IEnemy>();

        private ISprite sprite;
        private float horizontalVelocity;   // pixels/sec; gravity drives the vertical arc.
        private double timer;
        private double flightMs;
        private bool hasBurst;
        private bool hitBarrel;

        public MolotovProjectile(Vector2 position, Vector2 launchVelocity, ISprite sprite,
            IReadOnlyList<IBlock> blocks, float gravity, CollisionManager collisionManager, Action onBurst = null)
        {
            Position = position;
            PreviousPosition = position;
            this.sprite = sprite;
            horizontalVelocity = launchVelocity.X;
            this.collisionManager = collisionManager ?? new CollisionManager(960, 540);
            this.collisionManager.SetBlocks(this, blocks ?? Array.Empty<IBlock>());
            this.onBurst = onBurst;
            physics = new Physics(gravity);
            physics.VerticalVelocity = launchVelocity.Y;
            IsActive = true;
        }

        public Vector2 Position { get; set; }
        public Vector2 PreviousPosition { get; private set; }
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);
        public bool IsActive { get; private set; }
        public bool IsBurning => IsActive && hasBurst;

        public void Update(GameTime gameTime)
        {
            if (!IsActive)
            {
                return;
            }
            PreviousPosition = Position;

            if (!hasBurst)
            {
                Fly(gameTime);
                return;
            }

            timer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer <= 0)
            {
                IsActive = false;
            }
            sprite.Update(gameTime);
        }

        public void HitEnemy(IEnemy enemy)
        {
            if (IsBurning && !enemy.IsDestroyed && burned.Add(enemy))
            {
                enemy.TakeDamage(FireDamage);
            }
        }

        public void HitSolid(IBlock block)
        {
            if (hasBurst)
            {
                return;
            }
            if (block is IExplosiveBarrel barrel)
            {
                hitBarrel = true;
                barrel.Ignite();
            }
            Burst();
        }

        public void Deactivate() => IsActive = false;

        public void Draw(SpriteBatch spriteBatch) => sprite.Draw(spriteBatch, Position);

        private void Fly(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move horizontally against walls (HitSolid bursts it on a wall), then apply gravity with
            // vertical collision resolution so it settles on the ground and the ground state is set.
            Vector2 afterHorizontal = collisionManager.MoveHorizontally(this, Position, sprite.Width, sprite.Height,
                horizontalVelocity * elapsed, float.NegativeInfinity, float.PositiveInfinity);
            Position = afterHorizontal;
            Position = collisionManager.ApplyGravityAndResolveVerticalCollisions(this, physics, Position,
                sprite.Width, sprite.Height, gameTime);
            sprite.Update(gameTime);

            if (collisionManager.IsOnGround(this))
            {
                Burst();
                return;
            }

            // Burst anyway if the throw has been in the air too long (e.g. flew off a ledge).
            flightMs += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (flightMs >= MaxFlightMs)
            {
                Burst();
            }
        }

        private void Burst()
        {
            int blastLevel = hitBarrel ? BarrelBlastLevel : BaseBlastLevel;
            ISprite fire = ProjectileSpriteFactory.Instance.CreateExplosionSprite(blastLevel);
            float x = Position.X + (sprite.Width - fire.Width) / 2f;
            float y = Position.Y + sprite.Height - fire.Height;
            Position = new Vector2(x, y);
            sprite = fire;
            timer = FireTime;
            hasBurst = true;
            onBurst?.Invoke();
        }
    }
}
