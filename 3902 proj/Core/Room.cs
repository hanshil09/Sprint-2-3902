using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    internal sealed class Room
    {
        public Room(Level level, IProjectileManager projectiles, GameObjectCycler enemies)
        {
            Level = level;
            Projectiles = projectiles;
            Enemies = enemies;
        }

        public Level Level { get; }

        public IProjectileManager Projectiles { get; }

        public GameObjectCycler Enemies { get; }

        public bool IsGoalReached => Level.Goal != null && Level.Goal.IsReached;

        public string Objective
        {
            get
            {
                if (Level.Goal == null)
                {
                    return "GOLD door: next room    CYAN door: return    T/Y U/I O/P: object showcase";
                }
                return Level.Goal.IsUnlocked
                    ? "The beacon is online! Touch it to complete the mission."
                    : "Defeat the DECEPTICON WARLORD to power the beacon.";
            }
        }

        public void Update(GameTime gameTime)
        {
            Level.Update(gameTime);
            foreach (IGameObject enemy in Enemies.Objects)
            {
                if (enemy is not IEnemy combatant || !combatant.IsDestroyed)
                {
                    enemy.Update(gameTime);
                }
            }
            Projectiles.Update(gameTime);
            Level.Goal?.SetUnlocked(!HasLiveBoss());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Level.Draw(spriteBatch);
            foreach (IGameObject enemy in Enemies.Objects)
            {
                if (enemy is not IEnemy combatant || !combatant.IsDestroyed)
                {
                    enemy.Draw(spriteBatch);
                }
            }
            Projectiles.Draw(spriteBatch);
        }

        public IReadOnlyList<IItem> CollectItems(IPlayer player)
        {
            return Level.CollectItems(player);
        }

        public IReadOnlyList<IGameObject> GetCollisionObjects(IPlayer player)
        {
            List<IGameObject> objects = new List<IGameObject>(Level.GetOnScreenObjects())
            {
                player
            };
            objects.AddRange(Enemies.Objects);
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

        private bool HasLiveBoss()
        {
            foreach (IGameObject enemy in Enemies.Objects)
            {
                if (enemy is Boss boss && !boss.IsDestroyed)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
