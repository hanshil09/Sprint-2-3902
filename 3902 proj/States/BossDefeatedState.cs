using Microsoft.Xna.Framework;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

namespace TransformersGame.States
{
       internal sealed class BossDefeatedState : IBossState
    {
        private const double DestructionSeconds = 1.8;
        private const float SinkSpeed = 40f;

        private readonly Boss boss;
        private double timer;

        public BossDefeatedState(Boss boss)
        {
            this.boss = boss;
            boss.SetPose(BossPose.Defeated);
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer += elapsed;
            boss.MoveToward(new Vector2(boss.Position.X, Boss.FloorY), SinkSpeed, elapsed);
            if (timer >= DestructionSeconds)
            {
                boss.Finish();
            }
        }
    }
}
