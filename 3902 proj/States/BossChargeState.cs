using Microsoft.Xna.Framework;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.States
{
    
    internal sealed class BossChargeState : IBossState
    {
        private readonly Boss boss;
        private readonly BossConfiguration configuration;
        private readonly Vector2 wall;

        public BossChargeState(Boss boss, int direction)
        {
            this.boss = boss;
            configuration = boss.Configuration;
            wall = new Vector2(direction > 0 ? configuration.MaximumX : configuration.MinimumX,
                configuration.FloorY);
        }

        public void Update(GameTime gameTime)
        {
            float speed = boss.Phase == 3 ? configuration.EnragedChargeSpeed : configuration.ChargeSpeed;
            if (boss.MoveToward(wall, speed, (float)gameTime.ElapsedGameTime.TotalSeconds))
            {
                boss.State = new BossRiseState(boss);
            }
        }
    }
}
