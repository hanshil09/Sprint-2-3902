using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TransformersGame.Interfaces
{
    internal interface IProjectileManager
    {
        IReadOnlyList<IProjectile> Projectiles { get; }

        void Add(IProjectile projectile);

        void Clear();

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }
}
