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
            enemy.Sprite = EnemySpriteFactory.Instance.CreateEnemySprite(enemy.Kind, Color.Gray, enemy.Facing < 0);
        }

        public int Facing
        {
            get
            {
                return 0;
            }
        }

        public void ChangeDirection()
        {
        }

        public void BeDestroyed()
        {
        }

        public void Update(GameTime gameTime)
        {
            enemy.ApplyGravity(gameTime);
        }
    }
}
