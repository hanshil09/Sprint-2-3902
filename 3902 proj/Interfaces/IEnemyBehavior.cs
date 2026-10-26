using Microsoft.Xna.Framework;
using TransformersGame.Entities;

namespace TransformersGame.Interfaces
{
    // A behavior controls how an enemy moves; IEnemyState separately controls its facing and life state.
    interface IEnemyBehavior
    {
        void Update(Enemy enemy, GameTime gameTime);
    }
}
