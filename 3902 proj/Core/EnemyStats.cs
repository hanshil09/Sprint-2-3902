namespace TransformersGame.Core
{
    public class EnemyStats
    {
        public EnemyStats(float movementSpeed, float patrolDistance, double directionChangeSeconds, double hopSeconds)
        {
            MovementSpeed = movementSpeed;
            PatrolDistance = patrolDistance;
            DirectionChangeSeconds = directionChangeSeconds;
            HopSeconds = hopSeconds;
        }

        public float MovementSpeed { get; private set; }

        public float PatrolDistance { get; private set; }

        public double DirectionChangeSeconds { get; private set; }

        public double HopSeconds { get; private set; }
    }
}
