using Microsoft.Xna.Framework;

namespace TransformersGame.Entities
{
     internal sealed class ItemBob
    {
        private const float Amplitude = 3f;
        private const double PeriodSeconds = 1.2;

        private double time;

        public Vector2 Offset => new Vector2(0, Amplitude * (float)System.Math.Sin(2 * System.Math.PI * time / PeriodSeconds));

        public void Update(GameTime gameTime)
        {
            time += gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
