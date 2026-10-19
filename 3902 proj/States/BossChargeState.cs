using Microsoft.Xna.Framework;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.States
{
    
    internal sealed class BossChargeState : IBossState
    {
        private const float ChargeSpeed = 380f;
        private const float EnragedChargeSpeed = 460f;

        private readonly Boss boss;
        private readonly Vector2 wall;

        public BossChargeState(Boss boss, int direction)
        {
            this.boss = boss;
            wall = new Vector2(direction > 0 ? Boss.MaximumX : Boss.MinimumX, Boss.FloorY);
        }

        public void Update(GameTime gameTime)
        {
            float speed = boss.Phase == 3 ? EnragedChargeSpeed : ChargeSpeed;
            if (boss.MoveToward(wall, speed, (float)gameTime.ElapsedGameTime.TotalSeconds))
            {
                boss.State = new BossRiseState(boss);
            }
        }
    }
}
