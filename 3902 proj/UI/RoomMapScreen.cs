using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ArmadilloGame.Core;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.UI;

internal sealed class RoomMapScreen
{
    private const float MaximumPreviewScale = 0.72f;
    private const int PreviewLeft = 134;
    private const int PreviewTop = 54;
    private const int HeaderY = 13;
    private const int ShotButtonsY = 451;
    private const int ShotButtonWidth = 130;
    private const int ShotButtonHeight = 36;
    private const int ShotButtonGap = 10;
    private const int ShotIconGap = 4;
    private const float ShotLabelScale = 0.65f;
    private const int FooterY = 497;
    private static readonly Color Accent = new Color(255, 207, 113);

    private static readonly Rectangle PreviousButton = new Rectangle(28, 246, 48, 48);
    private static readonly Rectangle NextButton = new Rectangle(884, 246, 48, 48);
    private static readonly Color ButtonColor = new Color(77, 58, 41);

    private readonly SpriteFont font;
    private readonly WeaponSelection weapons;
    private int activeRoomIndex;
    private int previewIndex;
    private Level previewLevel;
    private ButtonState previousLeftButton;

    public RoomMapScreen(SpriteFont font, WeaponSelection weapons)
    {
        this.font = font;
        this.weapons = weapons;
    }

    public void Open(int roomIndex)
    {
        activeRoomIndex = roomIndex;
        previewIndex = roomIndex;
        previewLevel = null;
        previousLeftButton = Mouse.GetState().LeftButton;
    }

    public void UpdateNavigation(RoomFactory factory, IPlayer player)
    {
        MouseState mouse = Mouse.GetState();
        Point mousePosition = Core.ScreenScale.ToVirtual(mouse.Position);
        if (mouse.LeftButton == ButtonState.Pressed && previousLeftButton == ButtonState.Released)
        {
            if (PreviousButton.Contains(mousePosition) && previewIndex > 0)
            {
                ShowRoom(factory, previewIndex - 1);
            }
            else if (NextButton.Contains(mousePosition) && previewIndex < factory.RoomCount - 1)
            {
                ShowRoom(factory, previewIndex + 1);
            }
            else
            {
                SelectWeapon(mousePosition, player);
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
        Level level = previewLevel ?? rooms.ActiveRoom.Level;
        Rectangle world = level.WorldBounds;
        float scale = MathHelper.Min(viewport.Width * MaximumPreviewScale / world.Width,
            viewport.Height * MaximumPreviewScale / world.Height);
        int width = (int)(world.Width * scale);
        int height = (int)(world.Height * scale);
        Rectangle preview = new Rectangle((viewport.Width - width) / 2, PreviewTop, width, height);
        batch.Begin(samplerState: SamplerState.PointClamp);
        HudFrame.Fill(batch, preview, roomColor);
        string title = $"ROOM {previewIndex + 1}/{rooms.RoomCount}  {level.Name}";
        DrawArrow(batch, PreviousButton, "<", previewIndex > 0);
        DrawArrow(batch, NextButton, ">", previewIndex < rooms.RoomCount - 1);
        batch.DrawString(font, title, new Vector2(PreviewLeft, HeaderY), Accent);
        DrawWeaponButtons(batch, player);
        batch.DrawString(font, "Click to equip  |  1-4 equip  |  TAB resumes", new Vector2(PreviewLeft, FooterY), Color.White);
        batch.End();

        Matrix transform = Matrix.CreateTranslation(-world.X, -world.Y, 0f) *
            Matrix.CreateScale(scale, scale, 1f) *
            Matrix.CreateTranslation(preview.X, preview.Y, 0f);
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
    private void SelectWeapon(Point mousePosition, IPlayer player)
    {
        for (int i = 0; i < WeaponSelection.Count; i++)
        {
            if (WeaponButton(i).Contains(mousePosition))
            {
                weapons.Select(i, player);
                return;
            }
        }
    }

    private void DrawWeaponButtons(SpriteBatch batch, IPlayer player)
    {
        for (int i = 0; i < WeaponSelection.Count; i++)
        {
            Rectangle bounds = WeaponButton(i);
            WeaponKind kind = WeaponSelection.KindAt(i);
            bool unlocked = player.IsWeaponUnlocked(kind);
            Color background = ButtonColor;
            Color textColor = unlocked ? Color.White : Color.Gray;
            if (i == weapons.SelectedIndex)
            {
                background = Accent;
                textColor = Color.Black;
            }

            HudFrame.Fill(batch, bounds, background);
            string name = WeaponSelection.NameAt(i);
            if (!unlocked)
            {
                name += " (LOCKED)";
            }
            else if (!player.WeaponHasUnlimitedAmmo(kind))
            {
                name += $" x{player.WeaponAmmo(kind)}";
            }
            ISprite preview = weapons.PreviewAt(i);
            Vector2 textSize = font.MeasureString(name) * ShotLabelScale;
            float left = bounds.Center.X - (preview.Width + ShotIconGap + textSize.X) / 2f;
            preview.Draw(batch, new Vector2(left, bounds.Center.Y - preview.Height / 2f));
            Vector2 position = new Vector2(left + preview.Width + ShotIconGap, bounds.Center.Y - textSize.Y / 2f);
            batch.DrawString(font, name, position, textColor, 0f, Vector2.Zero, ShotLabelScale, SpriteEffects.None, 0f);
        }
    }

    private static Rectangle WeaponButton(int index) =>
        new Rectangle(PreviewLeft + index * (ShotButtonWidth + ShotButtonGap), ShotButtonsY, ShotButtonWidth, ShotButtonHeight);

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
