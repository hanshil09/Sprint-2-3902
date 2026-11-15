using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ArmadilloGame.Interfaces
{
    interface ISprite
    {
        int Width { get; }

        int Height { get; }

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch, Vector2 location);
    }
}
