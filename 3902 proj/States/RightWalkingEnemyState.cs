using Microsoft.Xna.Framework;
using ArmadilloGame.Core;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.States
{
    internal class RightWalkingEnemyState : IEnemyState
    {
        private Enemy enemy;

        public RightWalkingEnemyState(Enemy enemy)
        {
            this.enemy = enemy;
            enemy.Sprite = enemy.GetDirectionalSprite(Direction.Right);
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
