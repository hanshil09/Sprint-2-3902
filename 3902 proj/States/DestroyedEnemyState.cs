using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.States
{
    public class DestroyedEnemyState : IEnemyState
    {
        private Enemy enemy;

        public DestroyedEnemyState(Enemy enemy)
        {
            this.enemy = enemy;
            enemy.Sprite = EnemySpriteFactory.Instance.CreateDestroyedEnemySprite(enemy.Tint);
        }

        public void ChangeDirection()
        {
        }

        public void Hop()
        {
        }

        public void BeDestroyed()
        {
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
