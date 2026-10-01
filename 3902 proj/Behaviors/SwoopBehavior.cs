using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.Behaviors
{
    public class SwoopBehavior : IEnemyBehavior
    {
        private const float BobAmplitude = 14f;
        private const double BobPeriodSeconds = 1.6;
        private const double WindupSeconds = 0.45;
        private const double InitialCooldownSeconds = 1.5;
        private const float VerticalReach = 320f;
        private const float RetreatSpeedFactor = 2.5f;

        private enum Phase { Hover, Windup, Dive, Retreat }

        private Phase phase = Phase.Hover;
        private double timer;
        private double cooldown = InitialCooldownSeconds;
        private int patrolDirection = -1;
        private Vector2 diveTarget;
        private Vector2 retreatTarget;

        public void Update(Enemy enemy, GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (phase)
            {
                case Phase.Hover:
                    Hover(enemy, elapsed);
                    break;
                case Phase.Windup:
                    Windup(enemy, elapsed);
                    break;
                case Phase.Dive:
                    Dive(enemy, elapsed);
                    break;
                default:
                    Retreat(enemy, elapsed);
                    break;
            }
        }

        private void Hover(Enemy enemy, float elapsed)
        {
            timer += elapsed;
            cooldown -= elapsed;
            float left = enemy.Home.X - enemy.Stats.PatrolDistance;
            float right = enemy.Home.X + enemy.Stats.PatrolDistance;
            if (enemy.Position.X <= left)
            {
                patrolDirection = 1;
            }
            else if (enemy.Position.X >= right)
            {
                patrolDirection = -1;
            }
            enemy.Face(patrolDirection);
            enemy.MoveBy(patrolDirection * enemy.Stats.MovementSpeed * elapsed);
            double angle = 2 * System.Math.PI * timer / BobPeriodSeconds;
            enemy.Position = new Vector2(enemy.Position.X, enemy.Home.Y + BobAmplitude * (float)System.Math.Sin(angle));
            if (cooldown <= 0 && enemy.CanSeeTarget(enemy.Stats.SightRange, VerticalReach))
            {
                timer = 0;
                phase = Phase.Windup;
            }
        }

        private void Windup(Enemy enemy, float elapsed)
        {
            timer += elapsed;
            enemy.Face(enemy.DirectionToTarget);
            if (timer >= WindupSeconds)
            {
                diveTarget = enemy.TargetCenter - new Vector2(enemy.Width / 2f, enemy.Height / 2f);
                phase = Phase.Dive;
            }
        }

        private void Dive(Enemy enemy, float elapsed)
        {
            Vector2 toTarget = diveTarget - enemy.Position;
            float distance = toTarget.Length();
            float step = enemy.Stats.ActionSpeed * elapsed;
            if (distance <= step)
            {
                enemy.Position = diveTarget;
                BeginRetreat(enemy);
            }
            else
            {
                enemy.Face(toTarget.X >= 0 ? 1 : -1);
                enemy.Position += toTarget / distance * step;
            }
        }

        private void BeginRetreat(Enemy enemy)
        {
            float left = enemy.Home.X - enemy.Stats.PatrolDistance;
            float right = enemy.Home.X + enemy.Stats.PatrolDistance;
            retreatTarget = new Vector2(MathHelper.Clamp(enemy.Position.X, left, right), enemy.Home.Y);
            phase = Phase.Retreat;
        }

        private void Retreat(Enemy enemy, float elapsed)
        {
            Vector2 toTarget = retreatTarget - enemy.Position;
            float distance = toTarget.Length();
            float step = enemy.Stats.MovementSpeed * RetreatSpeedFactor * elapsed;
            if (distance <= step)
            {
                enemy.Position = retreatTarget;
                timer = 0;
                cooldown = enemy.Stats.CooldownSeconds;
                phase = Phase.Hover;
            }
            else
            {
                enemy.Position += toTarget / distance * step;
            }
        }
    }
}
