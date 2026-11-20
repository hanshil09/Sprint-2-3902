using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Core
{
    internal sealed class Room
    {
        // Only one key drops per room lifetime; this flags once that key has been handed out.
        private bool keyDropped;

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
            foreach (IGameObject enemy in enemies)
            {
                bool destroyed = enemy is IEnemy combatant && combatant.IsDestroyed;
                if (!destroyed)
                {
                    enemy.Update(gameTime);
                }
                else
                {
                    TryDropKey(enemy);
                }
                if (enemy is Boss boss && boss.IsDefeated)
                {
                    Level.Progress.RecordBossDefeated();
                }
            }
            Projectiles.Update(gameTime);
            Level.Goal?.SetUnlocked(Level.Progress.BossDefeated || !HasLiveBoss());
        }

        // Drops the single level key when the last living enemy is defeated. Only one key exists per
        // room, and it appears at the final enemy's position. The drop id keeps the key from
        // re-spawning once the player has collected it, even after leaving and returning to the room.
        private void TryDropKey(IGameObject enemy)
        {
            if (keyDropped || HasLiveEnemy())
            {
                return;
            }
            keyDropped = true;
            const string dropId = "room";
            if (Level.HasSpawnedEnemyDrop(dropId))
            {
                return;
            }
            Level.SpawnKeyDrop(enemy.Position, dropId);
        }

        private bool HasLiveEnemy()
        {
            foreach (IGameObject enemy in Enemies.Objects)
            {
                if (enemy is IEnemy combatant && !combatant.IsDestroyed)
                {
                    return true;
                }
            }
            return false;
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
