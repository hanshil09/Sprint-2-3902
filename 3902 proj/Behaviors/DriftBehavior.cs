using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.Behaviors
{
    public class DriftBehavior : IEnemyBehavior
    {
        private const float WaveAmplitude = 45f;
        private const double WavePeriodSeconds = 1.8;
        private const float SwaySpeed = 60f;
        private const float HoverAbovePlayer = 120f;
        private const float MinimumAltitude = 24f;
        private const float MaximumAltitude = 380f;
        private const float AltitudeEasing = 2.5f;
        private const float SightHeight = 1000f;

        private double time;
        private float altitude;
        private bool isInitialized;

        public void Update(Enemy enemy, GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (!isInitialized)
            {
                altitude = enemy.Position.Y;
                isInitialized = true;
            }
            time += elapsed;

            float goalX;
            float goalAltitude;
            float speed;
            if (enemy.CanSeeTarget(enemy.Stats.SightRange, SightHeight))
            {
                goalX = enemy.TargetCenter.X - enemy.Width / 2f;
                goalAltitude = MathHelper.Clamp(enemy.TargetCenter.Y - HoverAbovePlayer - enemy.Height / 2f, MinimumAltitude, MaximumAltitude);
                speed = enemy.Stats.ActionSpeed;
            }
            else
            {
                goalX = enemy.Home.X;
                goalAltitude = enemy.Home.Y;
                speed = enemy.Stats.MovementSpeed;
            }

            float maximumStep = speed * elapsed;
            float driftX = MathHelper.Clamp(goalX - enemy.Position.X, -maximumStep, maximumStep);
            altitude += (goalAltitude - altitude) * MathHelper.Clamp(AltitudeEasing * elapsed, 0f, 1f);

            double angle = 2 * System.Math.PI * time / WavePeriodSeconds;
            float sway = SwaySpeed * (float)System.Math.Cos(angle) * elapsed;
            if (driftX != 0)
            {
                enemy.Face(System.Math.Sign(driftX));
            }
            enemy.MoveBy(driftX + sway);
            enemy.Position = new Vector2(enemy.Position.X, altitude + WaveAmplitude * (float)System.Math.Sin(angle));
        }
    }
}
