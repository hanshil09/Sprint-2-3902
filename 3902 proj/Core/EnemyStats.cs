namespace TransformersGame.Core
{
    public class EnemyStats
    {
        public EnemyStats(float movementSpeed, float patrolDistance, float sightRange, float actionSpeed, double cooldownSeconds)
        {
            MovementSpeed = movementSpeed;
            PatrolDistance = patrolDistance;
            SightRange = sightRange;
            ActionSpeed = actionSpeed;
            CooldownSeconds = cooldownSeconds;
        }

        public float MovementSpeed { get; private set; }

        public float PatrolDistance { get; private set; }

        public float SightRange { get; private set; }

        public float ActionSpeed { get; private set; }

        public double CooldownSeconds { get; private set; }
    }
}
