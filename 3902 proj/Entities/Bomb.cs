using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    public class Bomb : IProjectile
    {
        private const double FuseTime = 1200;
        private const double ExplosionTime = 320;

        private ISprite sprite;
        private double timer;
        private bool hasExploded;

        public Bomb(Vector2 position, ISprite sprite)
        {
            Position = position;
            this.sprite = sprite;
            timer = FuseTime;
            hasExploded = false;
            IsActive = true;
        }

        public Vector2 Position { get; set; }

        public bool IsActive { get; private set; }

        public void Update(GameTime gameTime)
        {
            timer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer <= 0 && !hasExploded)
            {
                Explode();
            }
            else if (timer <= 0)
            {
                IsActive = false;
            }
            sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch, Position);
        }

        private void Explode()
        {
            ISprite explosion = ProjectileSpriteFactory.Instance.CreateExplosionSprite();
            float x = Position.X + (sprite.Width - explosion.Width) / 2f;
            float y = Position.Y + sprite.Height - explosion.Height;
            Position = new Vector2(x, y);
            sprite = explosion;
            timer = ExplosionTime;
            hasExploded = true;
        }
    }
}
