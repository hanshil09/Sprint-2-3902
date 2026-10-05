using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TransformersGame.Interfaces
{
    public interface IProjectileManager
    {
        void Add(IProjectile projectile);

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }
}
