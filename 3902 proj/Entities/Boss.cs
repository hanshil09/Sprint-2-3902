using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Factories;
using TransformersGame.Interfaces;
using TransformersGame.States;
using TransformersGame.UI;

namespace TransformersGame.Entities
{
        internal sealed class Boss : IEnemy
    {
        public const int MaximumHealth = 30;
        public const float HoverY = 96f;
        public const float FloorY = 540 - BlockSpriteFactory.BlockSize - EnemySpriteFactory.BossSize;
        public const float MinimumX = 16f;
        public const float MaximumX = 960 - EnemySpriteFactory.BossSize - 16f;
        // The open middle of the arena between the side ledges; the boss only rises or dives here.
        public const float OpenLeft = 208f;
        public const float OpenRight = 592f;
        private const int HitboxInset = 14;
        private const double HitCooldownSeconds = 0.08;
        private const double FlashSeconds = 0.12;
        private const float FireballSpeed = 260f;
        private static readonly Rectangle HealthBarArea = new Rectangle(280, 512, 400, 22);

        private readonly IProjectileManager projectiles;
        private ISprite sprite;
        private double hitCooldown;
        private double flashTimer;

        public Boss(Vector2 position, IPlayer target, IProjectileManager projectiles)
        {
            Position = position;
            Target = target;
            this.projectiles = projectiles;
            Health = MaximumHealth;
            SetPose(BossPose.Idle);
            State = new BossHoverState(this);
        }

        public IBossState State { get; set; }

        public IPlayer Target { get; }

        public Vector2 Position { get; set; }

        public int Health { get; private set; }

        public bool IsDefeated => Health <= 0;

        public bool IsDestroyed { get; private set; }

        // Phase 1 above two thirds health, phase 2 above one third, phase 3 (enraged) below that.
        public int Phase => Health * 3 > MaximumHealth * 2 ? 1 : Health * 3 > MaximumHealth ? 2 : 3;

        // A defeated boss has no hitbox while its destruction plays out.
        public Rectangle Bounds => IsDefeated ? Rectangle.Empty : new Rectangle(
            (int)Position.X + HitboxInset, (int)Position.Y + HitboxInset,
            EnemySpriteFactory.BossSize - 2 * HitboxInset, EnemySpriteFactory.BossSize - HitboxInset);

        public Vector2 Center => Position + new Vector2(EnemySpriteFactory.BossSize / 2f);

        public Vector2 TargetCenter => Target.Position + new Vector2(Target.Width / 2f, Target.Height / 2f);

        public void SetPose(BossPose pose)
        {
            Color tint = pose == BossPose.Defeated ? Color.Gray : Color.White;
            sprite = EnemySpriteFactory.Instance.CreateBossSprite(pose, tint);
        }

        public void TakeDamage(int amount)
        {
            if (IsDefeated || hitCooldown > 0 || amount <= 0)
            {
                return;
            }
            Health = System.Math.Max(0, Health - amount);
            hitCooldown = HitCooldownSeconds;
            flashTimer = FlashSeconds;
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

        public void Update(GameTime gameTime)
        {
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
            hitCooldown -= elapsed;
            flashTimer -= elapsed;
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
            Color fill = Phase == 3 ? Color.OrangeRed : Color.Gold;
            HealthBar.Draw(spriteBatch, HealthBarArea, Health / (float)MaximumHealth, fill);
        }

        private void FireInDirection(float radians)
        {
            Vector2 direction = new Vector2((float)System.Math.Cos(radians), (float)System.Math.Sin(radians));
            ISprite fireball = ProjectileSpriteFactory.Instance.CreateBossFireballSprite();
            projectiles.Add(new EnemyProjectile(Center, Center + direction * 100f, fireball, FireballSpeed));
        }
    }
}
