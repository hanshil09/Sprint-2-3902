using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;
using ArmadilloGame.States;
using ArmadilloGame.UI;

namespace ArmadilloGame.Entities
{
    internal sealed class Boss : IEnemy
    {
        private readonly BossConfiguration configuration;
        private readonly IProjectileManager projectiles;
        private ISprite sprite;
        private double hitCooldown;
        private double flashTimer;

        public Boss(Vector2 position, IPlayer target, IProjectileManager projectiles,
            BossConfiguration configuration = null)
        {
            this.configuration = configuration ?? BossConfiguration.Warlord;
            Position = position;
            Target = target;
            this.projectiles = projectiles;
            Health = this.configuration.MaximumHealth;
            SetPose(BossPose.Idle);
            State = new BossHoverState(this);
        }

        public BossConfiguration Configuration => configuration;

        public IBossState State { get; set; }

        public IPlayer Target { get; }

        public Vector2 Position { get; set; }

        public int Health { get; private set; }
        public EnemyKind Kind => configuration.Kind;

        public bool IsDefeated => Health <= 0;

        public bool IsDestroyed { get; private set; }

        // Phase 1 above two thirds health, phase 2 above one third, phase 3 (enraged) below that.
        public int Phase
        {
            get
            {
                if (Health * configuration.PhaseHealthDenominator
                    > configuration.MaximumHealth * configuration.PhaseOneHealthNumerator)
                {
                    return 1;
                }
                return Health * configuration.PhaseHealthDenominator
                    > configuration.MaximumHealth * configuration.PhaseTwoHealthNumerator ? 2 : 3;
            }
        }

        // A defeated boss has no hitbox while its destruction plays out.
        public Rectangle Bounds => IsDefeated ? Rectangle.Empty : new Rectangle(
            (int)Position.X + configuration.HitboxInset, (int)Position.Y + configuration.HitboxInset,
            configuration.Size - 2 * configuration.HitboxInset, configuration.Size - configuration.HitboxInset);

        public Vector2 Center => Position + new Vector2(configuration.Size / 2f);

        public Vector2 TargetCenter => Target.Position + new Vector2(Target.Width / 2f, Target.Height / 2f);

        public void SetPose(BossPose pose)
        {
            Color tint = pose == BossPose.Defeated ? Color.Gray : Color.White;
            sprite = configuration.CreateSprite(pose, tint, configuration.Size);
        }

        public void TakeDamage(int amount)
        {
            if (IsDefeated || hitCooldown > 0 || amount <= 0)
            {
                return;
            }
            Health = System.Math.Max(0, Health - amount);
            hitCooldown = configuration.HitCooldownSeconds;
            flashTimer = configuration.FlashSeconds;
            if (IsDefeated)
            {
                State = new BossDefeatedState(this);
            }
        }

        // Moves toward a point at a fixed speed; returns true once it arrives.
        public bool MoveToward(Vector2 destination, float speed, float elapsed)
        {
            Vector2 offset = destination - Position;
            float distance = offset.Length();
            float step = speed * elapsed;
            if (distance <= step)
            {
                Position = destination;
                return true;
            }
            Position += offset / distance * step;
            return false;
        }

        // Fires one fireball per angle, each rotated from the direction to the player.
        public void FireAtTarget(params float[] angleOffsetsDegrees)
        {
            Vector2 aim = TargetCenter - Center;
            float baseAngle = (float)System.Math.Atan2(aim.Y, aim.X);
            foreach (float offset in angleOffsetsDegrees)
            {
                FireInDirection(baseAngle + MathHelper.ToRadians(offset));
            }
        }

        public void FireRing(int count, float rotationDegrees)
        {
            for (int i = 0; i < count; i++)
            {
                FireInDirection(MathHelper.ToRadians(rotationDegrees) + i * MathHelper.TwoPi / count);
            }
        }

        public void Finish() => IsDestroyed = true;

        private readonly EnemyFreeze freeze = new EnemyFreeze();

        public void Freeze()
        {
            if (!IsDefeated)
            {
                freeze.Start();
            }
        }

        public void Update(GameTime gameTime)
        {
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
            hitCooldown -= elapsed;
            flashTimer -= elapsed;
            if (!IsDefeated && freeze.Update(gameTime))
            {
                return;
            }

            State.Update(gameTime);
            sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            bool flashHidden = flashTimer > 0 && (int)(flashTimer * 40) % 2 == 0;
            if (!flashHidden)
            {
                sprite.Draw(spriteBatch, Position);
            }
            Color fill = Phase == 3 ? configuration.EnragedHealthColor : configuration.NormalHealthColor;
            if (!IsDefeated)
            {
                freeze.Draw(spriteBatch, Bounds);
            }

            HeartMeter.Draw(spriteBatch, configuration.HealthHeartsPosition, Health,
                configuration.MaximumHealth, configuration.HealthHeartCount, fill);
        }

        private void FireInDirection(float radians)
        {
            Vector2 direction = new Vector2((float)System.Math.Cos(radians), (float)System.Math.Sin(radians));
            ISprite fireball = configuration.CreateProjectileSprite();
            projectiles.Add(new EnemyProjectile(Center, Center + direction * 100f, fireball,
                configuration.FireballSpeed));
        }
    }
}
