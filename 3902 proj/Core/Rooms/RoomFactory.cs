using System;
using ArmadilloGame.Collisions;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Core
{
    // Loads room files and builds Room objects (extracted from Game1 so it only coordinates the game).
    internal sealed class RoomFactory
    {
        private readonly string[] roomPaths;
        private readonly Random random = new Random();
        private readonly int[] roomSeeds;
        private RoomProgress[] roomProgress;

        public RoomFactory(params string[] roomPaths)
        {
            this.roomPaths = roomPaths;
            roomSeeds = new int[roomPaths.Length];
            StartNewGame();
        }

        public int RoomCount => roomPaths.Length;

        public void StartNewGame()
        {
            roomProgress = new RoomProgress[roomPaths.Length];
            for (int i = 0; i < roomSeeds.Length; i++)
            {
#pragma warning disable CA5394 // Gameplay layouts use repeatable seeds and have no security purpose.
                roomSeeds[i] = random.Next();
                roomProgress[i] = new RoomProgress();
#pragma warning restore CA5394
            }
        }

        public Level LoadLevel(int roomIndex)
        {
            LevelLoadResult loaded = LevelLoader.Load(roomPaths[roomIndex]);
            foreach (string message in loaded.Messages)
            {
                Console.WriteLine(message);
            }
            // Difficulty ramps from 0 in the first room to 1 in the last.
            float difficultyTier = RoomCount > 1 ? (float)roomIndex / (RoomCount - 1) : 0f;
            return new Level(loaded.Data, new Random(roomSeeds[roomIndex]), roomProgress[roomIndex], difficultyTier);
        }

        public static Room CreateRoom(Level level, IPlayer player, IProjectileManager projectiles,
            CollisionManager collisionManager = null)
        {
            GameObjectCycler enemies = new GameObjectCycler();
            level.FillEnemyCycler(enemies, player, projectiles, collisionManager);
            return new Room(level, projectiles, enemies);
        }
    }
}
