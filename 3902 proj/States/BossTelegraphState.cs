using Microsoft.Xna.Framework;
using ArmadilloGame.Core;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.States
{

    internal sealed class BossTelegraphState : IBossState
    {
        private readonly Boss boss;
        private readonly BossConfiguration configuration;
        private readonly Vector2 anchor;
        private double timer;

        public BossTelegraphState(Boss boss)
        {
            this.boss = boss;
            configuration = boss.Configuration;
            anchor = boss.Position;
            boss.SetPose(BossPose.Attack);
        }

        public void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;
            float shake = (int)(timer * 30) % 2 == 0 ? configuration.ShakeAmount : -configuration.ShakeAmount;
            boss.Position = anchor + new Vector2(shake, 0);
            if (timer >= configuration.WarningSeconds)
            {
                // Slams stay in the open middle of the arena so the boss never passes through the side ledges.
                float targetX = MathHelper.Clamp(boss.TargetCenter.X - boss.Center.X + boss.Position.X,
                    configuration.SlamLeft, configuration.SlamRight);
                boss.State = new BossPlungeState(boss, targetX);
            }
        }
    }
}
