using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    public class ProjectileManager : IProjectileManager
    {
        private readonly List<IProjectile> projectiles;

        public ProjectileManager()
        {
            projectiles = new List<IProjectile>();
        }

        public void Add(IProjectile projectile)
        {
            projectiles.Add(projectile);
        }

        public void Update(GameTime gameTime)
        {
            foreach (IProjectile projectile in projectiles)
            {
                projectile.Update(gameTime);
            }
            projectiles.RemoveAll(IsFinished);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (IProjectile projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }
        }

        private static bool IsFinished(IProjectile projectile)
        {
            return !projectile.IsActive;
        }
    }
}
