using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Core
{
    /// <summary>
    /// Coordinates runtime activity for one room, including enemy and projectile updates,
    /// objective and boss progress, collision objects, and unloading. <see cref="Level"/> owns the
    /// room's content and state, such as blocks, items, doors, triggers, gravity, and player placement.
    /// <see cref="RoomManager"/> selects the active room and unloads it when switching rooms.
    /// </summary>
    internal sealed class Room
    {
        // Only one key drops per room lifetime; this flags once that key has been handed out.
        private bool keyDropped;

        /// <summary>Creates a room around its level and runtime enemy and projectile managers.</summary>
        /// <param name="level">The content and state for this room.</param>
        /// <param name="projectiles">The room's projectile manager.</param>
        /// <param name="enemies">The room's enemy cycler.</param>
        public Room(Level level, IProjectileManager projectiles, GameObjectCycler enemies)
        {
            Level = level;
            Projectiles = projectiles;
            Enemies = enemies;
        }

        /// <summary>Gets the room content and state.</summary>
        public Level Level { get; }

        /// <summary>Gets the manager for projectiles active in this room.</summary>
        public IProjectileManager Projectiles { get; }

        /// <summary>Gets the cycler for enemies active in this room.</summary>
        public GameObjectCycler Enemies { get; }

        /// <summary>Gets whether this room's goal has been reached.</summary>
        public bool IsGoalReached => Level.Goal != null && Level.Goal.IsReached;

        /// <summary>Gets the current objective text for this room.</summary>
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

        /// <summary>Updates the level, enemies, projectiles, boss progress, and goal state for this frame.</summary>
        /// <param name="gameTime">The elapsed game time.</param>
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

        /// <summary>Draws the level and the active enemies and projectiles.</summary>
        /// <param name="spriteBatch">The sprite batch used for drawing.</param>
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

        /// <summary>Collects items from the level for the player.</summary>
        /// <param name="player">The player collecting items.</param>
        /// <returns>The items collected during this call.</returns>
        public IReadOnlyList<IItem> CollectItems(IPlayer player)
        {
            return Level.CollectItems(player);
        }

        /// <summary>Combines level, player, enemy, and active projectile objects for collision checks.</summary>
        /// <param name="player">The player to include in the collision objects.</param>
        /// <returns>A read-only list of objects used for collision checks.</returns>
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

        /// <summary>Records pending crate and boss progress, then clears projectiles before leaving.</summary>
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
