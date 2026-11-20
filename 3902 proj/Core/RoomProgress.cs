using System.Collections.Generic;

namespace ArmadilloGame.Core
{
    internal sealed class RoomProgress
    {
        private readonly HashSet<string> collectedItemIds = new();
        private readonly HashSet<string> openedCrateIds = new();
        private readonly HashSet<string> collectedCrateDropIds = new();
        private readonly HashSet<int> unlockedDoorTargets = new();

        public bool BossDefeated { get; private set; }

        // Records that the door leading to the given room has been unlocked, so it stays open when
        // the room is rebuilt (e.g. returning to the hub).
        public bool IsDoorUnlocked(int targetRoomIndex) => unlockedDoorTargets.Contains(targetRoomIndex);

        public void RecordDoorUnlocked(int targetRoomIndex) => unlockedDoorTargets.Add(targetRoomIndex);

        public bool HasCollectedItem(string itemId) => collectedItemIds.Contains(itemId);

        public void RecordCollectedItem(string itemId) => collectedItemIds.Add(itemId);

        public bool IsCrateOpened(string crateId) => openedCrateIds.Contains(crateId);

        public void RecordCrateOpened(string crateId) => openedCrateIds.Add(crateId);

        public bool HasCollectedCrateDrop(string dropId) => collectedCrateDropIds.Contains(dropId);

        public void RecordCollectedCrateDrop(string dropId) => collectedCrateDropIds.Add(dropId);

        public void RecordBossDefeated() => BossDefeated = true;
    }
}
