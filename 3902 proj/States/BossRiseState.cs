using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.States
{
        internal sealed class BossRiseState : IBossState
    {
        private const float ReturnSpeed = 260f;
        private const float RiseSpeed = 220f;

        private readonly Boss boss;
        private readonly Vector2 floorPoint;
        private readonly Vector2 hoverPoint;
        private bool backInMiddle;

        public BossRiseState(Boss boss)
        {
            this.boss = boss;
            float x = MathHelper.Clamp(boss.Position.X, Boss.OpenLeft, Boss.OpenRight);
            floorPoint = new Vector2(x, Boss.FloorY);
            hoverPoint = new Vector2(x, Boss.HoverY);
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (!backInMiddle)
            {
                backInMiddle = boss.MoveToward(floorPoint, ReturnSpeed, elapsed);
            }
            else if (boss.MoveToward(hoverPoint, RiseSpeed, elapsed))
            {
                boss.FireAtTarget(0f);
                boss.State = new BossHoverState(boss);
            }
        }
    }
}
