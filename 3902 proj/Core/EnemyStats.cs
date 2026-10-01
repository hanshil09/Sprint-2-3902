namespace TransformersGame.Core
{
    public class EnemyStats
    {
        public EnemyStats(float movementSpeed, float patrolDistance, double directionChangeSeconds, double hopSeconds,
            bool flies = false, float flightAmplitude = 0f, double flightPeriodSeconds = 2.0)
        {
            MovementSpeed = movementSpeed;
            PatrolDistance = patrolDistance;
            DirectionChangeSeconds = directionChangeSeconds;
            HopSeconds = hopSeconds;
            Flies = flies;
            FlightAmplitude = flightAmplitude;
            FlightPeriodSeconds = flightPeriodSeconds;
        }

        public float MovementSpeed { get; private set; }

        public float PatrolDistance { get; private set; }

        public double DirectionChangeSeconds { get; private set; }

        public double HopSeconds { get; private set; }

        public bool Flies { get; private set; }

        public float FlightAmplitude { get; private set; }

        public double FlightPeriodSeconds { get; private set; }
    }
}