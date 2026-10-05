using Microsoft.Xna.Framework;
using TransformersGame.Entities;

namespace TransformersGame.Behaviors
{
    public class StalkBehavior : GroundBehavior
    {
        private const float SameLevelTolerance = 60f;
        private const float StopDistance = 24f;
        private const double FireCooldownSeconds = 2.0;

        private bool isChasing;
        private double lostSightTimer;
        private double fireCooldown;

        public override void Update(Enemy enemy, GameTime gameTime)
        {
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
            if (enemy.CanSeeTarget(enemy.Stats.SightRange, SameLevelTolerance))
            {
                isChasing = true;
                lostSightTimer = 0;
                fireCooldown -= elapsed;
                if (fireCooldown <= 0)
                {
                    enemy.FireProjectile();
                    fireCooldown = FireCooldownSeconds;
                }
            }
            else if (isChasing)
            {
                lostSightTimer += elapsed;
                if (lostSightTimer >= enemy.Stats.CooldownSeconds)
                {
                    isChasing = false;
                }
            }

            if (isChasing)
            {
                if (enemy.HorizontalDistanceToTarget > StopDistance)
                {
                    WalkSafely(enemy, enemy.DirectionToTarget, enemy.Stats.ActionSpeed, gameTime);
                }
            }
            else
            {
                Patrol(enemy, gameTime);
            }
            enemy.ApplyGravity(gameTime);
        }
    }
}
