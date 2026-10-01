using Microsoft.Xna.Framework;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.Behaviors
{
    public class LeapBehavior : IEnemyBehavior
    {
        private const float LeapSpeed = 560f;
        private const float IdleHopSpeed = 380f;
        private const float LeapAirSeconds = 0.75f;
        private const float VerticalReach = 200f;

        private double cooldown;
        private float airVelocity;
        private int idleDirection = 1;

        public void Update(Enemy enemy, GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (enemy.IsOnGround)
            {
                airVelocity = 0;
                cooldown -= elapsed;
                if (cooldown <= 0)
                {
                    Leap(enemy);
                    cooldown = enemy.Stats.CooldownSeconds;
                }
            }
            else
            {
                enemy.MoveBy(airVelocity * elapsed);
            }
            enemy.ApplyGravity(gameTime);
        }

        private void Leap(Enemy enemy)
        {
            EnemyStats stats = enemy.Stats;
            if (enemy.CanSeeTarget(stats.SightRange, VerticalReach))
            {
                int direction = enemy.DirectionToTarget;
                float speed = System.Math.Min(stats.ActionSpeed, enemy.HorizontalDistanceToTarget / LeapAirSeconds);
                enemy.Face(direction);
                airVelocity = direction * speed;
                enemy.Jump(LeapSpeed);
            }
            else
            {
                float offset = enemy.Position.X - enemy.Home.X;
                if (offset > stats.PatrolDistance)
                {
                    idleDirection = -1;
                }
                else if (offset < -stats.PatrolDistance)
                {
                    idleDirection = 1;
                }
                else
                {
                    idleDirection = -idleDirection;
                }
                enemy.Face(idleDirection);
                airVelocity = idleDirection * stats.MovementSpeed;
                enemy.Jump(IdleHopSpeed);
            }
        }
    }
}
