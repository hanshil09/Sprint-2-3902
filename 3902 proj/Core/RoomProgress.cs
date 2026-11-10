using System.Collections.Generic;

namespace TransformersGame.Core
{
    internal sealed class RoomProgress
    {
        private readonly HashSet<string> collectedItemIds = new();
        private readonly HashSet<string> openedCrateIds = new();
        private readonly HashSet<string> collectedCrateDropIds = new();

        public bool BossDefeated { get; private set; }

        public bool HasCollectedItem(string itemId) => collectedItemIds.Contains(itemId);

        public void RecordCollectedItem(string itemId) => collectedItemIds.Add(itemId);

        public bool IsCrateOpened(string crateId) => openedCrateIds.Contains(crateId);

        public void RecordCrateOpened(string crateId) => openedCrateIds.Add(crateId);

        public bool HasCollectedCrateDrop(string dropId) => collectedCrateDropIds.Contains(dropId);

        public void RecordCollectedCrateDrop(string dropId) => collectedCrateDropIds.Add(dropId);

        public void RecordBossDefeated() => BossDefeated = true;
    }
}
