using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    // Groups the values that define an enemy type so Enemy does not need a long parameter list.
    public sealed class EnemyConfiguration
    {
        public EnemyConfiguration(EnemyKind kind, EnemyStats stats, IEnemyBehavior behavior)
        {
            Kind = kind;
            Stats = stats;
            Behavior = behavior;
        }

        public EnemyKind Kind { get; }

        public EnemyStats Stats { get; }

        public IEnemyBehavior Behavior { get; }
    }
}
