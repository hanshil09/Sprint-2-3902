using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.UI
{
    // One animated preview in the object showcase (a block, item or enemy sprite with its name).
    internal sealed class ShowcaseEntry : IGameObject
    {
        private readonly ISprite sprite;

        public ShowcaseEntry(string name, ISprite sprite)
        {
            Name = name;
            this.sprite = sprite;
        }

        public string Name { get; }

        public int Width => sprite.Width;

        public int Height => sprite.Height;

        public Vector2 Position { get; set; }

        public void Update(GameTime gameTime) => sprite.Update(gameTime);

        public void Draw(SpriteBatch spriteBatch) => sprite.Draw(spriteBatch, Position);
    }
}
