using System.Linq;
using Microsoft.Xna.Framework;
using ArmadilloGame.Core;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.States
{
    internal sealed class BossHoverState : IBossState
    {
        private readonly Boss boss;
        private readonly BossConfiguration configuration;
        private double fireTimer;
        private double slamTimer;
        private int patrolDirection = 1;
        private int volleyCount;

        public BossHoverState(Boss boss)
        {
            this.boss = boss;
            configuration = boss.Configuration;
            fireTimer = configuration.FireIntervals[boss.Phase];
            slamTimer = configuration.SlamIntervals[boss.Phase];
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
                fireTimer = configuration.FireIntervals[boss.Phase];
            }
            slamTimer -= elapsed;
            if (boss.Phase >= 2 && slamTimer <= 0)
            {
                boss.State = new BossTelegraphState(boss);
            }
        }

        private void Patrol(float elapsed)
        {
            if (boss.Position.X <= configuration.PatrolLeft)
            {
                patrolDirection = 1;
            }
            else if (boss.Position.X >= configuration.PatrolRight)
            {
                patrolDirection = -1;
            }
            float speed = configuration.PatrolSpeeds[boss.Phase];
            float x = boss.Position.X + patrolDirection * speed * elapsed;
            boss.MoveToward(new Vector2(x, configuration.HoverY), speed * 2, elapsed);
        }

        // Aimed three-shot volleys; when enraged every other volley is a full ring.
        private void FireVolley()
        {
            volleyCount++;
            if (boss.Phase == 3 && volleyCount % 2 == 0)
            {
                boss.FireRing(configuration.RingShotCount, volleyCount * configuration.RingRotationPerVolley);
            }
            else
            {
                boss.FireAtTarget(configuration.FireAngleOffsets.ToArray());
            }
        }
    }
}
