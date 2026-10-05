using Microsoft.Xna.Framework;
using TransformersGame.Entities;

namespace TransformersGame.Behaviors
{
    public class ChargeBehavior : GroundBehavior
    {
        private const double WindupSeconds = 0.6;
        private const float MaximumChargeDistance = 360f;
        private const float SameLevelTolerance = 50f;

        private enum Phase { Patrol, Windup, Charge, Rest }

        private Phase phase = Phase.Patrol;
        private double timer;
        private int chargeDirection;
        private float distanceCharged;

        public override void Update(Enemy enemy, GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (phase)
            {
                case Phase.Patrol:
                    Patrol(enemy, gameTime);
                    if (enemy.IsOnGround && enemy.CanSeeTarget(enemy.Stats.SightRange, SameLevelTolerance))
                    {
                        chargeDirection = enemy.DirectionToTarget;
                        enemy.Face(chargeDirection);
                        timer = 0;
                        phase = Phase.Windup;
                    }
                    break;
                case Phase.Windup:
                    timer += elapsed;
                    if (timer >= WindupSeconds)
                    {
                        distanceCharged = 0;
                        phase = Phase.Charge;
                    }
                    break;
                case Phase.Charge:
                    Charge(enemy, elapsed);
                    break;
                default:
                    timer += elapsed;
                    if (timer >= enemy.Stats.CooldownSeconds)
                    {
                        phase = Phase.Patrol;
                    }
                    break;
            }
            enemy.ApplyGravity(gameTime);
        }

        private void Charge(Enemy enemy, float elapsed)
        {
            float step = enemy.Stats.ActionSpeed * elapsed;
            if (distanceCharged < MaximumChargeDistance && enemy.HasGroundAhead(chargeDirection, step))
            {
                enemy.MoveBy(chargeDirection * step);
                distanceCharged += step;
            }
            else
            {
                timer = 0;
                phase = Phase.Rest;
            }
        }
    }
}
