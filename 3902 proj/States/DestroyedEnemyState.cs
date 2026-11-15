using Microsoft.Xna.Framework;
using ArmadilloGame.Entities;
using ArmadilloGame.Factories;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.States
{
    internal class DestroyedEnemyState : IEnemyState
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
