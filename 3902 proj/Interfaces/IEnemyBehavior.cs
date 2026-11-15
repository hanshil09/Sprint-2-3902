using Microsoft.Xna.Framework;
using ArmadilloGame.Entities;

namespace ArmadilloGame.Interfaces
{
    // A behavior controls how an enemy moves; IEnemyState separately controls its facing and life state.
    interface IEnemyBehavior
    {
        void Update(Enemy enemy, GameTime gameTime);
    }
}
