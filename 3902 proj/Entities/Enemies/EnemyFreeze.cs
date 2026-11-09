using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Sprites;

namespace TransformersGame.Entities;

internal sealed class EnemyFreeze
{
    private const double DurationSeconds = 5;
    private const int BarHeight = 3;
    private const int BarGap = 4;
    private static readonly Color Tint = new Color(76, 180, 255, 90);

    private double remainingSeconds;

    public void Start() => remainingSeconds = DurationSeconds;

    public bool Update(GameTime gameTime)
    {
        remainingSeconds = Math.Max(0, remainingSeconds - gameTime.ElapsedGameTime.TotalSeconds);
        return remainingSeconds > 0;
    }

    public void Draw(SpriteBatch batch, Rectangle bounds)
    {
        if (remainingSeconds <= 0)
        {
            return;
        }

        int width = (int)(bounds.Width * remainingSeconds / DurationSeconds);
        Rectangle bar = new Rectangle(bounds.X, bounds.Y - BarGap - BarHeight, width, BarHeight);
        batch.Draw(SpriteAtlas.Texture, bounds, SpriteAtlas.WhitePixel, Tint);
        batch.Draw(SpriteAtlas.Texture, bar, SpriteAtlas.WhitePixel, Color.LightSkyBlue);
    }
}
