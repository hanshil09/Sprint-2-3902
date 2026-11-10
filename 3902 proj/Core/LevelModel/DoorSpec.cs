namespace TransformersGame.Core
{
    // Describes a door placed in a room. A door either moves relative to the current room
    // (legacy '<' = previous, '>' = next) or targets a specific room index (hub-and-spoke).
    // Locked doors require the player to spend a key before they open.
    internal readonly struct DoorSpec
    {
        private DoorSpec(int direction, int? targetRoomIndex, bool locked)
        {
            Direction = direction;
            TargetRoomIndex = targetRoomIndex;
            IsLocked = locked;
        }

        // -1 previous, +1 next; 0 when the door uses TargetRoomIndex instead.
        public int Direction { get; }

        // Absolute destination room index, or null for a legacy relative door.
        public int? TargetRoomIndex { get; }

        public bool IsLocked { get; }

        public bool IsTargeted => TargetRoomIndex.HasValue;

        // Legacy relative door from the '<' / '>' symbols.
        public static DoorSpec Relative(int direction) => new DoorSpec(direction, null, false);

        // Targeted door (grid digit for unlocked, letter for locked) to a specific room index.
        public static DoorSpec ToRoom(int roomIndex, bool locked) => new DoorSpec(0, roomIndex, locked);
    }
}
