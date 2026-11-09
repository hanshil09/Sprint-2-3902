using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Sprites
{
    // The art can extend past the feet, especially when the selfie stick is raised.
    internal readonly record struct SelfiePose(ISprite Sprite, Point FootOffset)
    {
        public Rectangle BoundsAt(Point feet) => new Rectangle(feet.X - FootOffset.X,
            feet.Y - FootOffset.Y, Sprite.Width, Sprite.Height);

        public void Draw(SpriteBatch batch, Point feet, float opacity = 1f)
        {
            Rectangle bounds = BoundsAt(feet);
            Vector2 position = new Vector2(bounds.X, bounds.Y);
            if (Sprite is TextureRegionSprite region) region.Draw(batch, position, opacity);
            else Sprite.Draw(batch, position);
        }
    }
}
