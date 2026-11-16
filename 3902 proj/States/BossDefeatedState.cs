using Microsoft.Xna.Framework;
using ArmadilloGame.Core;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.States
{
       internal sealed class BossDefeatedState : IBossState
       {
           private readonly Boss boss;
           private readonly BossConfiguration configuration;
           private double timer;

        public BossDefeatedState(Boss boss)
        {
            this.boss = boss;
            configuration = boss.Configuration;
            boss.SetPose(BossPose.Defeated);
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer += elapsed;
            boss.MoveToward(new Vector2(boss.Position.X, configuration.FloorY), configuration.SinkSpeed, elapsed);
            if (timer >= configuration.DestructionSeconds)
            {
                boss.Finish();
            }
        }
    }
}
