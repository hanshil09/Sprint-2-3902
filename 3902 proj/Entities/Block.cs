using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;
using TransformersGame.Core;

namespace TransformersGame.Entities
{
    public class Block : IBlock
    {
        private ISprite sprite;

        public Block(Vector2 position, ISprite sprite, bool isSolid, BlockKind kind = BlockKind.Ground)
        {
            Position = position;
            this.sprite = sprite;
            IsSolid = isSolid;
            Kind = kind;
        }

        public BlockKind Kind { get; }
        public bool IsDestroyed { get; private set; }
        public bool IsHazard => Kind == BlockKind.Hazard;
        public void Destroy()
        {
            if (Kind == BlockKind.Crate)
            {
                IsDestroyed = true;
                IsSolid = false;
            }
        }

        public Vector2 Position { get; set; }

        public bool IsSolid { get; private set; }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);
            }
        }

        public void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsDestroyed) sprite.Draw(spriteBatch, Position);
        }
    }
}
