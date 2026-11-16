using Microsoft.Xna.Framework;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.States
{
        internal sealed class BossPlungeState : IBossState
        {
            private readonly Boss boss;
            private readonly BossConfiguration configuration;
            private readonly Vector2 landing;

        public BossPlungeState(Boss boss, float targetX)
        {
            this.boss = boss;
            configuration = boss.Configuration;
            landing = new Vector2(targetX, configuration.FloorY);
        }

        public void Update(GameTime gameTime)
        {
            if (boss.MoveToward(landing, configuration.PlungeSpeed,
                (float)gameTime.ElapsedGameTime.TotalSeconds))
            {
                int direction = boss.TargetCenter.X >= boss.Center.X ? 1 : -1;
                boss.State = new BossChargeState(boss, direction);
            }
        }
    }
}
