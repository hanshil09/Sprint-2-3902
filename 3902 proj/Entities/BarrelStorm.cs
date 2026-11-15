
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    internal sealed class BarrelStorm : IDisposable
    {
        private const int ScreenWidth = 960;
        private const int ScreenHeight = 540;
        private const int BarrelCount = 9;
        private const float SpawnInterval = 0.42f;
        private const float StormDuration = 5.2f;
        private const float CooldownDuration = 14f;

        // Standard barrel sprite from additionalItemSprites.png.
        private static readonly Rectangle BarrelSource =
            new Rectangle(71, 15, 16, 17);

        private readonly Texture2D pixel;
        private readonly Texture2D barrelTexture;
        private readonly Action<GameSound> playSound;

        private readonly List<BarrelProjectile> barrels =
            new List<BarrelProjectile>();

        private readonly List<DustParticle> particles =
            new List<DustParticle>();

        private IProjectileManager projectiles;
        private float cooldownRemaining;
        private float stormElapsed;
        private float spawnElapsed;
        private bool isActive;
        private int spawnedBarrels;
        private Rectangle visibleBounds;
        private Rectangle roomBounds;

        public BarrelStorm(
            GraphicsDevice graphicsDevice,
            Action<GameSound> playSound,
            Texture2D barrelTexture)
        {
            ArgumentNullException.ThrowIfNull(graphicsDevice);
            ArgumentNullException.ThrowIfNull(playSound);
            ArgumentNullException.ThrowIfNull(barrelTexture);

            this.playSound = playSound;
            this.barrelTexture = barrelTexture;

            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public bool IsActive => isActive;

        public float CooldownRemaining => cooldownRemaining;

        public bool TryActivate(
            IProjectileManager roomProjectiles,
            Rectangle visibleWorldBounds,
            Rectangle roomWorldBounds)
        {
            ArgumentNullException.ThrowIfNull(roomProjectiles);

            if (isActive || cooldownRemaining > 0)
            {
                return false;
            }

            projectiles = roomProjectiles;
            visibleBounds = visibleWorldBounds;
            roomBounds = roomWorldBounds;

            cooldownRemaining = CooldownDuration;
            stormElapsed = 0;
            spawnElapsed = 0;
            spawnedBarrels = 0;
            isActive = true;

            SpawnBarrel();
            playSound(GameSound.Explosion);

            return true;
        }

        public void Update(GameTime gameTime, Rectangle currentViewBounds)
        {
            float elapsed =
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            cooldownRemaining = Math.Max(0, cooldownRemaining - elapsed);

            if (isActive)
            {
                visibleBounds = currentViewBounds;
                stormElapsed += elapsed;
                spawnElapsed += elapsed;

                while (spawnedBarrels < BarrelCount &&
                       spawnElapsed >= SpawnInterval)
                {
                    spawnElapsed -= SpawnInterval;
                    SpawnBarrel();
                }

                if (stormElapsed >= StormDuration)
                {
                    isActive = false;

                    foreach (BarrelProjectile barrel in barrels)
                    {
                        barrel.Deactivate();
                    }

                    barrels.Clear();
                    projectiles = null;
                }
            }

            foreach (BarrelProjectile barrel in barrels)
            {
                if (barrel.IsActive)
                {
                    barrel.EmitDust();
                }
            }

            foreach (DustParticle particle in particles)
            {
                particle.Update(elapsed);
            }

            particles.RemoveAll(particle => particle.IsExpired);
            barrels.RemoveAll(barrel => !barrel.IsActive);
        }

        public void DrawOverlay(SpriteBatch spriteBatch)
        {
            if (!isActive)
            {
                return;
            }

            float fadeIn =
                MathHelper.Clamp(stormElapsed / 0.35f, 0, 1);

            float fadeOut =
                MathHelper.Clamp(
                    (StormDuration - stormElapsed) / 0.9f,
                    0,
                    1);

            byte alpha = (byte)(105 * Math.Min(fadeIn, fadeOut));

            spriteBatch.Draw(
                pixel,
                new Rectangle(0, 0, ScreenWidth, ScreenHeight / 2),
                new Color(20, 23, 30, (int)alpha));
        }

        public void DrawParticles(SpriteBatch spriteBatch)
        {
            foreach (DustParticle particle in particles)
            {
                particle.Draw(spriteBatch, pixel);
            }
        }

        public void CancelRoomEffect()
        {
            isActive = false;

            foreach (BarrelProjectile barrel in barrels)
            {
                barrel.Deactivate();
            }

            barrels.Clear();
            particles.Clear();
            projectiles = null;
        }

        public void Reset()
        {
            CancelRoomEffect();

            cooldownRemaining = 0;
            stormElapsed = 0;
            spawnElapsed = 0;
            spawnedBarrels = 0;
        }

        public void Dispose()
        {
            CancelRoomEffect();
            pixel.Dispose();
        }

        private void SpawnBarrel()
        {
            if (spawnedBarrels >= BarrelCount)
            {
                return;
            }

            int minimumX = Math.Max(
                visibleBounds.Left,
                roomBounds.Left);

            int maximumX = Math.Min(
                visibleBounds.Right - BarrelProjectile.Width,
                roomBounds.Right - BarrelProjectile.Width);

            if (maximumX < minimumX)
            {
                minimumX = MathHelper.Clamp(
                    visibleBounds.Center.X - BarrelProjectile.Width / 2,
                    roomBounds.Left,
                    roomBounds.Right - BarrelProjectile.Width);

                maximumX = minimumX;
            }

            int x = maximumX == minimumX
                ? minimumX
                : RandomNumberGenerator.GetInt32(
                    minimumX,
                    maximumX + 1);

            float horizontalSpeed = NextRange(-50, 50);

            var barrel = new BarrelProjectile(
                new Vector2(
                    x,
                    visibleBounds.Top - BarrelProjectile.Height),
                horizontalSpeed,
                (position, count) => SpawnDust(position, count),
                () => playSound(GameSound.CrateBreak),
                barrelTexture,
                BarrelSource);

            projectiles.Add(barrel);
            barrels.Add(barrel);
            spawnedBarrels++;
        }

        private void SpawnDust(Vector2 position, int count)
        {
            for (int i = 0; i < count; i++)
            {
                float horizontalSpeed = NextRange(-45, 45);
                float verticalSpeed = NextRange(-80, -15);
                float size = NextRange(2, 5);

                particles.Add(
                    new DustParticle(
                        position,
                        new Vector2(horizontalSpeed, verticalSpeed),
                        size));
            }
        }

        private static float NextRange(float minimum, float maximum) =>
            minimum +
            RandomNumberGenerator.GetInt32(10_000) / 10_000f *
            (maximum - minimum);
    }

    internal sealed class BarrelProjectile : IPlayerWeapon
    {
        internal const int Width = 32;
        internal const int Height = 40;

        private const float Gravity = 650f;

        private readonly float horizontalSpeed;
        private readonly Action<Vector2, int> emitDust;
        private readonly Action playImpactSound;
        private readonly Texture2D barrelTexture;
        private readonly Rectangle barrelSource;

        private float verticalSpeed = 40f;
        private float dustTimer;
        private bool impactHandled;

        public BarrelProjectile(
            Vector2 position,
            float horizontalSpeed,
            Action<Vector2, int> emitDust,
            Action playImpactSound,
            Texture2D barrelTexture,
            Rectangle barrelSource)
        {
            Position = position;
            PreviousPosition = position;

            this.horizontalSpeed = horizontalSpeed;
            this.emitDust = emitDust;
            this.playImpactSound = playImpactSound;
            this.barrelTexture = barrelTexture;
            this.barrelSource = barrelSource;

            IsActive = true;
        }

        public Vector2 Position { get; set; }

        public Vector2 PreviousPosition { get; private set; }

        public Rectangle Bounds =>
            new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                Width,
                Height);

        public bool IsActive { get; private set; }

        public void Update(GameTime gameTime)
        {
            if (!IsActive)
            {
                return;
            }

            float elapsed =
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            PreviousPosition = Position;

            verticalSpeed = Math.Min(
                verticalSpeed + Gravity * elapsed,
                560f);

            Position +=
                new Vector2(horizontalSpeed, verticalSpeed) * elapsed;

            dustTimer += elapsed;

            while (dustTimer >= 0.1f)
            {
                dustTimer -= 0.1f;

                emitDust(
                    new Vector2(
                        Position.X + Width / 2f,
                        Position.Y + Height),
                    2);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                barrelTexture,
                Bounds,
                barrelSource,
                Color.White);
        }

        public void Deactivate() => IsActive = false;

        public void HitEnemy(IEnemy enemy)
        {
            if (impactHandled || !IsActive)
            {
                return;
            }

            impactHandled = true;

            if (!enemy.IsDestroyed)
            {
                enemy.TakeDamage(2);
            }

            Impact();
        }

        public void HitSolid(IBlock block)
        {
            if (!impactHandled && IsActive)
            {
                impactHandled = true;
                Impact();
            }
        }

        public void EmitDust()
        {
            if (IsActive)
            {
                emitDust(
                    new Vector2(
                        Position.X + Width / 2f,
                        Position.Y + Height),
                    1);
            }
        }

        private void Impact()
        {
            emitDust(
                new Vector2(
                    Position.X + Width / 2f,
                    Position.Y + Height - 4),
                9);

            playImpactSound();
            Deactivate();
        }
    }

    internal sealed class DustParticle
    {
        private static readonly Color[] Colors =
        {
            new Color(195, 153, 94, 210),
            new Color(224, 188, 127, 190),
            new Color(156, 112, 67, 180)
        };

        private Vector2 position;
        private readonly Vector2 velocity;
        private readonly float size;
        private readonly Color color;
        private float lifetime = 0.55f;

        public DustParticle(
            Vector2 position,
            Vector2 velocity,
            float size)
        {
            this.position = position;
            this.velocity = velocity;
            this.size = size;

            color = Colors[
                RandomNumberGenerator.GetInt32(Colors.Length)];
        }

        public bool IsExpired => lifetime <= 0;

        public void Update(float elapsed)
        {
            position += velocity * elapsed;
            lifetime -= elapsed;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            byte alpha = (byte)(
                MathHelper.Clamp(lifetime / 0.55f, 0, 1) *
                color.A);

            spriteBatch.Draw(
                pixel,
                new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    Math.Max(1, (int)size),
                    Math.Max(1, (int)size)),
                new Color(
                    color.R,
                    color.G,
                    color.B,
                    alpha));
        }
    }
}
