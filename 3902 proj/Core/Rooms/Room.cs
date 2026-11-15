using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    internal sealed class Room
    {
        // Enemies already credited with a key drop this room lifetime, keyed by their stable spawn order.
        private readonly HashSet<int> droppedEnemies = new();

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
            IReadOnlyList<IGameObject> enemies = Enemies.Objects;
            for (int index = 0; index < enemies.Count; index++)
            {
                IGameObject enemy = enemies[index];
                bool destroyed = enemy is IEnemy combatant && combatant.IsDestroyed;
                if (!destroyed)
                {
                    enemy.Update(gameTime);
                }
                else
                {
                    TryDropKey(index, enemy);
                }
                if (enemy is Boss boss && boss.IsDefeated)
                {
                    Level.Progress.RecordBossDefeated();
                }
            }
            Projectiles.Update(gameTime);
            Level.Goal?.SetUnlocked(Level.Progress.BossDefeated || !HasLiveBoss());
        }

        // Drops a key from a defeated enemy the first time it is seen destroyed. The spawn order index
        // gives each enemy a stable id so a key the player already picked up is not spawned again.
        private void TryDropKey(int index, IGameObject enemy)
        {
            if (!droppedEnemies.Add(index))
            {
                return;
            }
            string dropId = index.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (Level.HasSpawnedEnemyDrop(dropId))
            {
                return;
            }
            Level.SpawnKeyDrop(enemy.Position, dropId);
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
            List<IGameObject> objects = new List<IGameObject>(Level.GetCollisionObjects())
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
            Level.RecordPendingCrateDrops();
            foreach (IGameObject enemy in Enemies.Objects)
            {
                if (enemy is Boss boss && boss.IsDefeated)
                {
                    Level.Progress.RecordBossDefeated();
                }
            }
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
