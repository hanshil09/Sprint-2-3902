using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.States
{
    public class RightWalkingEnemyState : IEnemyState
    {
        private Enemy enemy;

        public RightWalkingEnemyState(Enemy enemy)
        {
            this.enemy = enemy;
            enemy.Sprite = enemy.RightSprite;
        }

        public int Facing
        {
            get
            {
                return 1;
            }
        }

        public void ChangeDirection()
        {
            enemy.State = new LeftWalkingEnemyState(enemy);
        }

        public void BeDestroyed()
        {
            enemy.State = new DestroyedEnemyState(enemy);
        }

        public void Update(GameTime gameTime)
        {
            enemy.Behavior.Update(enemy, gameTime);
        }
    }
}
