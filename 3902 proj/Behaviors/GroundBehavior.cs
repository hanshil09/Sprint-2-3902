using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.Behaviors
{
    public abstract class GroundBehavior : IEnemyBehavior
    {
        private int patrolDirection = -1;

        public abstract void Update(Enemy enemy, GameTime gameTime);

        protected void Patrol(Enemy enemy, GameTime gameTime)
        {
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
            if (!WalkSafely(enemy, patrolDirection, enemy.Stats.MovementSpeed, gameTime))
            {
                // Reverse at a ledge instead of allowing the preview enemy to leave its platform.
                patrolDirection = -patrolDirection;
            }
        }

        protected bool WalkSafely(Enemy enemy, int direction, float speed, GameTime gameTime)
        {
            if (!enemy.IsOnGround)
            {
                return true;
            }
            float step = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            enemy.Face(direction);
            if (!enemy.HasGroundAhead(direction, step))
            {
                return false;
            }
            enemy.MoveBy(direction * step);
            return true;
        }
    }
}
