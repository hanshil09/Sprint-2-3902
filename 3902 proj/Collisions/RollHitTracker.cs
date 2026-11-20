using System.Collections.Generic;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Collisions
{
    // Enforces the non-chainable roll rule: an enemy hit by a roll is immune until contact ends.
    internal sealed class RollHitTracker
    {
        private readonly HashSet<IEnemy> locked = new HashSet<IEnemy>();
        private readonly HashSet<IEnemy> touchedThisFrame = new HashSet<IEnemy>();

        public void BeginFrame() => touchedThisFrame.Clear();

        // True when the roll may hit this enemy now; a locked enemy still counts as touched.
        public bool TryRegisterHit(IEnemy enemy)
        {
            touchedThisFrame.Add(enemy);
            return locked.Add(enemy);
        }

        public void EndFrame() => locked.IntersectWith(touchedThisFrame);
    }
}
