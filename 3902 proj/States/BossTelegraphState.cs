using Microsoft.Xna.Framework;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.States
{

    internal sealed class BossTelegraphState : IBossState
    {
        private const double WarningSeconds = 0.7;
        private const float ShakeAmount = 3f;
        private const float SlamLeft = 288f;
        private const float SlamRight = 544f;

        private readonly Boss boss;
        private readonly Vector2 anchor;
        private double timer;

        public BossTelegraphState(Boss boss)
        {
            this.boss = boss;
            anchor = boss.Position;
            boss.SetPose(BossPose.Attack);
        }

        public void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;
            float shake = (int)(timer * 30) % 2 == 0 ? ShakeAmount : -ShakeAmount;
            boss.Position = anchor + new Vector2(shake, 0);
            if (timer >= WarningSeconds)
            {
                // Slams stay in the open middle of the arena so the boss never passes through the side ledges.
                float targetX = MathHelper.Clamp(boss.TargetCenter.X - boss.Center.X + boss.Position.X, SlamLeft, SlamRight);
                boss.State = new BossPlungeState(boss, targetX);
            }
        }
    }
}
