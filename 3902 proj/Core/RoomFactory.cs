using System;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    // Loads room files and builds Room objects (extracted from Game1 so it only coordinates the game).
    internal sealed class RoomFactory
    {
        private readonly string[] roomPaths;

        public RoomFactory(params string[] roomPaths)
        {
            this.roomPaths = roomPaths;
        }

        public int RoomCount => roomPaths.Length;

        public Level LoadLevel(int roomIndex)
        {
            LevelLoadResult loaded = LevelLoader.Load(roomPaths[roomIndex]);
            foreach (string message in loaded.Messages)
            {
                Console.WriteLine(message);
            }
            return new Level(loaded.Data);
        }

        public static Room CreateRoom(Level level, IPlayer player, IProjectileManager projectiles)
        {
            GameObjectCycler enemies = new GameObjectCycler();
            level.FillEnemyCycler(enemies, player, projectiles);
            return new Room(level, projectiles, enemies);
        }
    }
}
