using Microsoft.Xna.Framework;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.States
{
        internal sealed class BossRiseState : IBossState
        {
            private readonly Boss boss;
            private readonly BossConfiguration configuration;
            private readonly Vector2 floorPoint;
        private readonly Vector2 hoverPoint;
        private bool backInMiddle;

        public BossRiseState(Boss boss)
        {
            this.boss = boss;
            configuration = boss.Configuration;
            float x = MathHelper.Clamp(boss.Position.X, configuration.OpenLeft, configuration.OpenRight);
            floorPoint = new Vector2(x, configuration.FloorY);
            hoverPoint = new Vector2(x, configuration.HoverY);
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (!backInMiddle)
            {
                backInMiddle = boss.MoveToward(floorPoint, configuration.ReturnSpeed, elapsed);
            }
            else if (boss.MoveToward(hoverPoint, configuration.RiseSpeed, elapsed))
            {
                boss.FireAtTarget(0f);
                boss.State = new BossHoverState(boss);
            }
        }
    }
}
