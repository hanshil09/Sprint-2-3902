using System;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    public sealed class RoomManager
    {
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
            ActiveRoom = null;
            ActiveRoomIndex = -1;
            previousRoom?.Unload();

            player.EnterRoom(nextRoom.Level.Blocks, nextRoom.Projectiles, nextRoom.Level.PlayerStart);
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
