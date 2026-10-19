using Microsoft.Xna.Framework;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.States
{
      internal sealed class BossHoverState : IBossState
    {
        private const float PatrolLeft = 96f;
        private const float PatrolRight = 768f;
        private static readonly float[] PatrolSpeeds = { 0f, 80f, 110f, 150f };
        private static readonly double[] FireIntervals = { 0, 1.8, 1.6, 1.3 };
        private static readonly double[] SlamIntervals = { 0, 0, 5.0, 3.2 };
        private const int RingShotCount = 10;

        private readonly Boss boss;
        private double fireTimer;
        private double slamTimer;
        private int patrolDirection = 1;
        private int volleyCount;

        public BossHoverState(Boss boss)
        {
            this.boss = boss;
            fireTimer = FireIntervals[boss.Phase];
            slamTimer = SlamIntervals[boss.Phase];
            boss.SetPose(boss.Phase == 3 ? BossPose.Enraged : BossPose.Idle);
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Patrol(elapsed);
            fireTimer -= elapsed;
            if (fireTimer <= 0)
            {
                FireVolley();
                fireTimer = FireIntervals[boss.Phase];
            }
            slamTimer -= elapsed;
            if (boss.Phase >= 2 && slamTimer <= 0)
            {
                boss.State = new BossTelegraphState(boss);
            }
        }

        private void Patrol(float elapsed)
        {
            if (boss.Position.X <= PatrolLeft)
            {
                patrolDirection = 1;
            }
            else if (boss.Position.X >= PatrolRight)
            {
                patrolDirection = -1;
            }
            float x = boss.Position.X + patrolDirection * PatrolSpeeds[boss.Phase] * elapsed;
            boss.MoveToward(new Vector2(x, Boss.HoverY), PatrolSpeeds[boss.Phase] * 2, elapsed);
        }

        // Aimed three-shot volleys; when enraged every other volley is a full ring.
        private void FireVolley()
        {
            volleyCount++;
            if (boss.Phase == 3 && volleyCount % 2 == 0)
            {
                boss.FireRing(RingShotCount, volleyCount * 9f);
            }
            else
            {
                boss.FireAtTarget(-14f, 0f, 14f);
            }
        }
    }
}
