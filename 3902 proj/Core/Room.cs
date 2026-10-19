using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    internal sealed class Room
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
            foreach (IGameObject enemy in Enemies.Objects)
            {
                if (enemy is not IEnemy combatant || !combatant.IsDestroyed)
                {
                    enemy.Update(gameTime);
                }
            }
            Projectiles.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Level.Draw(spriteBatch);
            Blocks.Draw(spriteBatch);
            Items.Draw(spriteBatch);
            foreach (IGameObject enemy in Enemies.Objects)
            {
                if (enemy is not IEnemy combatant || !combatant.IsDestroyed)
                {
                    enemy.Draw(spriteBatch);
                }
            }
            Projectiles.Draw(spriteBatch);
        }

        public void CollectItems(IPlayer player)
        {
            Level.CollectItems(player);
        }

        public IReadOnlyList<IGameObject> GetCollisionObjects(IPlayer player)
        {
            List<IGameObject> objects = new List<IGameObject>(Level.GetOnScreenObjects())
            {
                player
            };
            foreach (IGameObject enemy in Enemies.Objects)
            {
                objects.Add(enemy);
            }
            foreach (IProjectile projectile in Projectiles.Projectiles)
            {
                if (projectile.IsActive)
                {
                    objects.Add(projectile);
                }
            }
            return objects.AsReadOnly();
        }

        public void Unload()
        {
            Projectiles.Clear();
        }
    }
}
