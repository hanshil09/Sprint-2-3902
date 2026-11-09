using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.UI;

internal sealed class RoomMapScreen
{
    private const float PreviewScale = 0.8f;
    private const int PreviewLeft = 96;
    private const int PreviewTop = 54;
    private const int HeaderY = 13;
    private const int FooterY = 497;
    private static readonly Color Accent = new Color(255, 207, 113);

    private static readonly Rectangle PreviousButton = new Rectangle(28, 246, 48, 48);
    private static readonly Rectangle NextButton = new Rectangle(884, 246, 48, 48);
    private static readonly Color ButtonColor = new Color(77, 58, 41);

    private readonly SpriteFont font;
    private int activeRoomIndex;
    private int previewIndex;
    private Level previewLevel;
    private ButtonState previousLeftButton;

    public RoomMapScreen(SpriteFont font) => this.font = font;

    public void Open(int roomIndex)
    {
        activeRoomIndex = roomIndex;
        previewIndex = roomIndex;
        previewLevel = null;
        previousLeftButton = Mouse.GetState().LeftButton;
    }

    public void UpdateNavigation(RoomFactory factory)
    {
        MouseState mouse = Mouse.GetState();
        if (mouse.LeftButton == ButtonState.Pressed && previousLeftButton == ButtonState.Released)
        {
            if (PreviousButton.Contains(mouse.Position) && previewIndex > 0)
            {
                ShowRoom(factory, previewIndex - 1);
            }
            else if (NextButton.Contains(mouse.Position) && previewIndex < factory.RoomCount - 1)
            {
                ShowRoom(factory, previewIndex + 1);
            }
        }

        previousLeftButton = mouse.LeftButton;
    }

    private void ShowRoom(RoomFactory factory, int roomIndex)
    {
        previewLevel = null;
        if (roomIndex != activeRoomIndex)
        {
            previewLevel = factory.LoadLevel(roomIndex);
        }

        previewIndex = roomIndex;
    }

    // Owns its sprite batches so only the room artwork is scaled.
    public void Draw(SpriteBatch batch, RoomManager rooms, IPlayer player, Color roomColor)
    {
        Viewport viewport = batch.GraphicsDevice.Viewport;
        Rectangle preview = new Rectangle(PreviewLeft, PreviewTop,
            (int)(viewport.Width * PreviewScale), (int)(viewport.Height * PreviewScale));
        batch.Begin(samplerState: SamplerState.PointClamp);
        HudFrame.Fill(batch, preview, roomColor);
        Level level = previewLevel ?? rooms.ActiveRoom.Level;
        string title = $"ROOM {previewIndex + 1}/{rooms.RoomCount}  {level.Name}";
        DrawArrow(batch, PreviousButton, "<", previewIndex > 0);
        DrawArrow(batch, NextButton, ">", previewIndex < rooms.RoomCount - 1);
        batch.DrawString(font, title, new Vector2(PreviewLeft, HeaderY), Accent);
        batch.DrawString(font, "TAB / ESC  Back to game", new Vector2(PreviewLeft, FooterY), Color.White);
        batch.End();

        Matrix transform = Matrix.CreateScale(PreviewScale, PreviewScale, 1f) *
            Matrix.CreateTranslation(PreviewLeft, PreviewTop, 0f);
        batch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: transform);
        if (previewLevel == null)
        {
            rooms.ActiveRoom.Draw(batch);
            player.Draw(batch);
        }
        else
        {
            previewLevel.Draw(batch);
        }
        batch.End();
    }
    private void DrawArrow(SpriteBatch batch, Rectangle bounds, string arrow, bool enabled)
    {
        if (enabled)
        {
            HudFrame.Fill(batch, bounds, ButtonColor);
            Vector2 position = bounds.Center.ToVector2() - font.MeasureString(arrow) / 2f;
            batch.DrawString(font, arrow, position, Accent);
        }
    }
}
