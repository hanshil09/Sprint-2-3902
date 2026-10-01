using Microsoft.Xna.Framework;
using TransformersGame.Entities;

namespace TransformersGame.Interfaces
{
    public interface IEnemyBehavior
    {
        void Update(Enemy enemy, GameTime gameTime);
    }
}