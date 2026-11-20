using System;
using ArmadilloGame.Interfaces;
using Microsoft.Xna.Framework;

namespace ArmadilloGame.Core
{
    internal sealed class RoomManager
    {
        private const int HubRoomIndex = 0;

        private readonly Func<int, Room> roomFactory;
        private readonly IPlayer player;

        public RoomManager(Room initialRoom, int roomCount, IPlayer player, Func<int, Room> roomFactory)
        {
            ArgumentNullException.ThrowIfNull(initialRoom);
            ArgumentNullException.ThrowIfNull(player);
            ArgumentNullException.ThrowIfNull(roomFactory);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(roomCount);

            ActiveRoom = initialRoom;
            ActiveRoomIndex = 0;
            RoomCount = roomCount;
            this.player = player;
            this.roomFactory = roomFactory;
        }

        public Room ActiveRoom { get; private set; }

        public int ActiveRoomIndex { get; private set; }

        public int RoomCount { get; }

        public void SwitchTo(int roomIndex)
        {
            if (roomIndex < 0 || roomIndex >= RoomCount)
            {
                throw new ArgumentOutOfRangeException(nameof(roomIndex));
            }
            if (roomIndex == ActiveRoomIndex)
            {
                return;
            }

            Room nextRoom = roomFactory(roomIndex);
            Room previousRoom = ActiveRoom;
            int previousIndex = ActiveRoomIndex;
            ActiveRoom = null;
            ActiveRoomIndex = -1;
            previousRoom?.Unload();

            // Arrive beside the door that leads back to the room just left, so re-entering a
            // room puts the player where they came in. Falls back to the level's start when there
            // is no matching door (e.g. the very first spawn).
            Vector2 spawn = nextRoom.Level.ArrivalFrom(previousIndex);

            // Entering a non-hub level locks the door back to the hub behind the player. The hub (room 0)
            // never locks, so the player can always leave a level once they hold a key.
            if (roomIndex != 0)
            {
                nextRoom.Level.LockReturnDoor(HubRoomIndex);
            }

            player.EnterRoom(nextRoom.Level.Blocks, nextRoom.Projectiles, spawn, nextRoom.Level.Gravity);
            ActiveRoom = nextRoom;
            ActiveRoomIndex = roomIndex;
        }

        public void SwitchNext()
        {
            SwitchTo((ActiveRoomIndex + 1) % RoomCount);
        }

        public void SwitchPrevious()
        {
            SwitchTo((ActiveRoomIndex - 1 + RoomCount) % RoomCount);
        }

        public void Unload()
        {
            Room previousRoom = ActiveRoom;
            ActiveRoom = null;
            ActiveRoomIndex = -1;
            previousRoom?.Unload();
        }
    }
}
