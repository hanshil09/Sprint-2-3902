using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.States
{
    public class LeftWalkingEnemyState : IEnemyState
    {
        private Enemy enemy;

        public LeftWalkingEnemyState(Enemy enemy)
        {
            this.enemy = enemy;
            enemy.Sprite = EnemySpriteFactory.Instance.CreateEnemySprite(enemy.Kind, enemy.Tint);
        }

        public void ChangeDirection()
        {
            enemy.State = new RightWalkingEnemyState(enemy);
        }

        public void Hop()
        {
            enemy.Hop();
        }

        public void BeDestroyed()
        {
            enemy.State = new DestroyedEnemyState(enemy);
        }

        public void Update(GameTime gameTime)
        {
            enemy.Walk(-1, gameTime);
        }
    }
}
