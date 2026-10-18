using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    public sealed class Room
    {
        public Room(Level level, IProjectileManager projectiles, GameObjectCycler blocks, GameObjectCycler items, GameObjectCycler enemies)
        {
            Level = level;
            Projectiles = projectiles;
            Blocks = blocks;
            Items = items;
            Enemies = enemies;
        }

        public Level Level { get; }

        public IProjectileManager Projectiles { get; }

        public GameObjectCycler Blocks { get; }

        public GameObjectCycler Items { get; }

        public GameObjectCycler Enemies { get; }

        public void Update(GameTime gameTime)
        {
            Level.Update(gameTime);
            Blocks.Update(gameTime);
            Items.Update(gameTime);
            Enemies.Update(gameTime);
            Projectiles.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Level.Draw(spriteBatch);
            Blocks.Draw(spriteBatch);
            Items.Draw(spriteBatch);
            Enemies.Draw(spriteBatch);
            Projectiles.Draw(spriteBatch);
        }

        public void CollectItems(IPlayer player)
        {
            Level.CollectItems(player);
        }

        public List<IGameObject> GetCollisionObjects(IPlayer player)
        {
            List<IGameObject> objects = new List<IGameObject>(Level.GetOnScreenObjects())
            {
                player
            };
            if (Enemies.CurrentObject != null)
            {
                objects.Add(Enemies.CurrentObject);
            }
            foreach (IProjectile projectile in Projectiles.Projectiles)
            {
                if (projectile.IsActive)
                {
                    objects.Add(projectile);
                }
            }
            return objects;
        }

        public void Unload()
        {
            Projectiles.Clear();
        }
    }
}
