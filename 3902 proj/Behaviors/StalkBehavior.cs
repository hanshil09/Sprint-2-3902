using Microsoft.Xna.Framework;
using TransformersGame.Entities;

namespace TransformersGame.Behaviors
{
    public class StalkBehavior : GroundBehavior
    {
        private const float SameLevelTolerance = 60f;
        private const float StopDistance = 24f;

        private bool isChasing;
        private double lostSightTimer;

        public override void Update(Enemy enemy, GameTime gameTime)
        {
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
            if (enemy.CanSeeTarget(enemy.Stats.SightRange, SameLevelTolerance))
            {
                isChasing = true;
                lostSightTimer = 0;
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
