using Microsoft.Xna.Framework;
using ArmadilloGame.Core;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.States
{
    internal class LeftWalkingEnemyState : IEnemyState
    {
        private Enemy enemy;

        public LeftWalkingEnemyState(Enemy enemy)
        {
            this.enemy = enemy;
            enemy.Sprite = enemy.GetDirectionalSprite(Direction.Left);
        }

        public int Facing
        {
            get
            {
                return -1;
            }
        }

        public void ChangeDirection()
        {
            enemy.State = new RightWalkingEnemyState(enemy);
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
