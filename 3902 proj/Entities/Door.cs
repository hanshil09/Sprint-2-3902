using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    internal sealed class Door : IGameObject
    {
        private readonly ISprite sprite;
        public Door(Vector2 position, int direction, ISprite sprite)
        {
            Position = position;
            Direction = direction;
            this.sprite = sprite;
        }
        public int Direction { get; }
        public Vector2 Position { get; set; }
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);
        public void Update(GameTime gameTime) => sprite.Update(gameTime);
        public void Draw(SpriteBatch spriteBatch) => sprite.Draw(spriteBatch, Position);
    }
}
