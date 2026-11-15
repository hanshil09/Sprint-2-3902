using Microsoft.Xna.Framework;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.States
{
        internal sealed class BossPlungeState : IBossState
    {
        private const float PlungeSpeed = 480f;

        private readonly Boss boss;
        private readonly Vector2 landing;

        public BossPlungeState(Boss boss, float targetX)
        {
            this.boss = boss;
            landing = new Vector2(targetX, Boss.FloorY);
        }

        public void Update(GameTime gameTime)
        {
            if (boss.MoveToward(landing, PlungeSpeed, (float)gameTime.ElapsedGameTime.TotalSeconds))
            {
                int direction = boss.TargetCenter.X >= boss.Center.X ? 1 : -1;
                boss.State = new BossChargeState(boss, direction);
            }
        }
    }
}
