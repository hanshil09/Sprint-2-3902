using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.Behaviors
{
    internal sealed class GuardBehavior : IEnemyBehavior
    {
        private readonly bool airborne;
        private double shotTimer;
        private double movementTimer;
        private int burstRemaining = 3;

        public GuardBehavior(bool airborne) => this.airborne = airborne;

        public void Update(Enemy enemy, GameTime gameTime)
        {
            double elapsed = gameTime.ElapsedGameTime.TotalSeconds;
            enemy.Face(enemy.DirectionToTarget);
            if (airborne)
            {
                movementTimer += elapsed;
                float targetY = enemy.Home.Y + (float)System.Math.Sin(movementTimer * 2) * 48;
                enemy.FlyVertically(targetY - enemy.Position.Y);
            }
            else
            {
                enemy.ApplyGravity(gameTime);
            }
            shotTimer -= elapsed;
            if (shotTimer <= 0 && enemy.CanSeeTarget(enemy.Stats.SightRange, 540))
            {
                enemy.LaunchProjectile();
                burstRemaining--;
                shotTimer = burstRemaining > 0 ? .25 : 2.5;
                if (burstRemaining == 0) burstRemaining = 3;
            }
        }
    }
}
