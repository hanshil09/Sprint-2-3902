
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;
using ArmadilloGame.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ArmadilloGame.UI;

internal sealed class PlayerHud
{
    private const int PanelTop = 8;

    // Player status panel.
    private const int StatusLeft = 12;
    private const int StatusWidth = 210;
    private const int StatusHeight = 76;

    private const int PortraitX = 24;
    private const int PortraitY = 16;
    private const int PortraitSize = 52;

    private static readonly Rectangle PortraitSource =
        new Rectangle(878, 78, 78, 78);

    private const int HealthLeft = 88;
    private const int HeartsTop = 16;
    private const int KeyTop = 39;
    private const int HealthHeartCount = 5;

    // Horizontal equipment panel.
    private const int EquipmentLeft = 730;
    private const int EquipmentWidth = 207;
    private const int EquipmentHeight = 68;

    private const int SlotSize = 44;
    private const int SlotGap = 5;
    private const int SlotStartX = EquipmentLeft + 8;
    private const int SlotTop = PanelTop + 12;

    private const float FontSupersample = 20f / 48f;

    private static readonly Color PanelColor =
        new Color(0, 0, 0, 220);

    private static readonly Color HeadingColor =
        new Color(255, 225, 177);

    private static readonly Color TextColor =
        new Color(226, 201, 164);

    // Muted red used for HUD borders.
    private static readonly Color AccentColor =
        new Color(170, 55, 60);

    // Brighter red for the selected weapon.
    private static readonly Color SelectedColor =
        new Color(220, 75, 80);

    private static readonly Color LockedColor =
        new Color(100, 88, 77);

    private static readonly Color KeyOffColor =
        new Color(75, 75, 75);

    private static readonly Color KeyOnColor =
        new Color(235, 181, 65);

    private static readonly string[] KeyIcon =
    {
        ".XXXX......",
        "XX....XXXXXX",
        "XX....XXXXXX",
        ".XXXX......"
    };

    private readonly SpriteFont font;
    private readonly Texture2D portraitTexture;

    public PlayerHud(SpriteFont font, Texture2D portraitTexture)
    {
        this.font = font;
        this.portraitTexture = portraitTexture;
    }

    public void Draw(
        SpriteBatch spriteBatch,
        IPlayer player,
        WeaponSelection weapons)
    {
        DrawPlayerStatus(spriteBatch, player);
        DrawEquipment(spriteBatch, player, weapons);
    }

    private void DrawPlayerStatus(
        SpriteBatch spriteBatch,
        IPlayer player)
    {
        Rectangle panel = new Rectangle(
            StatusLeft,
            PanelTop,
            StatusWidth,
            StatusHeight);

        DrawPanel(spriteBatch, panel);

        // Portrait frame.
        Rectangle portrait = new Rectangle(
            PortraitX,
            PortraitY,
            PortraitSize,
            PortraitSize);

        Fill(
            spriteBatch,
            new Rectangle(
                portrait.X - 2,
                portrait.Y - 2,
                portrait.Width + 4,
                portrait.Height + 4),
            Color.Black);

        Fill(
            spriteBatch,
            new Rectangle(
                portrait.X - 1,
                portrait.Y - 1,
                portrait.Width + 2,
                portrait.Height + 2),
            AccentColor);

        // Draw the original portrait sprite from its spritesheet.
        spriteBatch.Draw(
            portraitTexture,
            portrait,
            PortraitSource,
            Color.White);

        // Health to the right of the portrait.
  

        HeartMeter.Draw(
            spriteBatch,
            new Vector2(HealthLeft, HeartsTop),
            player.Health,
            player.MaxHealth,
            HealthHeartCount,
            Color.Crimson);

        // Key silhouette: empty when not owned, gold when owned.
        bool hasKey = player.KeyCount > 0;

        DrawKeyIcon(
            spriteBatch,
            new Point(HealthLeft, KeyTop),
            hasKey ? KeyOnColor : KeyOffColor);
    }

    private void DrawEquipment(
        SpriteBatch spriteBatch,
        IPlayer player,
        WeaponSelection weapons)
    {
        Rectangle panel = new Rectangle(
            EquipmentLeft,
            PanelTop,
            EquipmentWidth,
            EquipmentHeight);

        DrawPanel(spriteBatch, panel);


        for (int i = 0; i < WeaponSelection.Count; i++)
        {
            Rectangle slot = new Rectangle(
                SlotStartX + i * (SlotSize + SlotGap),
                SlotTop,
                SlotSize,
                SlotSize);

            bool selected = weapons.SelectedIndex == i;

            bool unlocked = player.IsWeaponUnlocked(
                WeaponSelection.KindAt(i));

            Color background = selected
                ? new Color(45, 45, 45)
                : new Color(15, 15, 15);

            Color border = selected
                ? SelectedColor
                : new Color(100, 85, 70);

            DrawSlot(
                spriteBatch,
                slot,
                background,
                border,
                selected);

            if (unlocked)
            {
                ISprite preview = weapons.PreviewAt(i);

                int previewX = slot.X +
                    (slot.Width - preview.Width) / 2;

                int previewY = slot.Y +
                    (slot.Height - preview.Height) / 2;

                preview.Draw(
                    spriteBatch,
                    new Vector2(previewX, previewY));
            }
            else
            {
                DrawCenteredText(
                    spriteBatch,
                    "LOCK",
                    slot,
                    LockedColor,
                    0.30f);
            }

            // Small slot number in the upper-right corner.
            DrawText(
                spriteBatch,
                (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture), //weird warning had to ask ai :(
                slot.Right - 9,
                slot.Y + 2,
                selected ? SelectedColor : TextColor,
                0.22f);
        }
    }

    private static void DrawKeyIcon(
        SpriteBatch spriteBatch,
        Point position,
        Color color)
    {
        const int pixelSize = 2;

        for (int row = 0; row < KeyIcon.Length; row++)
        {
            for (int column = 0; column < KeyIcon[row].Length; column++)
            {
                if (KeyIcon[row][column] != 'X')
                    continue;

                Rectangle pixel = new Rectangle(
                    position.X + column * pixelSize,
                    position.Y + row * pixelSize,
                    pixelSize,
                    pixelSize);

                Fill(spriteBatch, pixel, color);
            }
        }
    }

    private static void DrawPanel(
    SpriteBatch spriteBatch,
    Rectangle bounds)
    {
        const int cornerRadius = 5;
        const int borderThickness = 2;

        FillRoundedRectangle(
            spriteBatch,
            bounds,
            AccentColor,
            cornerRadius);

        Rectangle innerBounds = new Rectangle(
            bounds.X + borderThickness,
            bounds.Y + borderThickness,
            bounds.Width - borderThickness * 2,
            bounds.Height - borderThickness * 2);

        FillRoundedRectangle(
            spriteBatch,
            innerBounds,
            PanelColor,
            cornerRadius - borderThickness);
    }

    private static void DrawSlot(
    SpriteBatch spriteBatch,
    Rectangle bounds,
    Color background,
    Color border,
    bool selected)
    {
        int thickness = selected ? 2 : 1;
        const int cornerRadius = 4;

        FillRoundedRectangle(
            spriteBatch,
            bounds,
            border,
            cornerRadius);

        Rectangle innerBounds = new Rectangle(
            bounds.X + thickness,
            bounds.Y + thickness,
            bounds.Width - thickness * 2,
            bounds.Height - thickness * 2);

        FillRoundedRectangle(
            spriteBatch,
            innerBounds,
            background,
            Math.Max(1, cornerRadius - thickness));
    }

    private void DrawText(
        SpriteBatch spriteBatch,
        string text,
        int x,
        int y,
        Color color,
        float scale)
    {
        spriteBatch.DrawString(
            font,
            text,
            new Vector2(x, y),
            color,
            0f,
            Vector2.Zero,
            scale * FontSupersample,
            SpriteEffects.None,
            0f);
    }

    private void DrawCenteredText(
        SpriteBatch spriteBatch,
        string text,
        Rectangle bounds,
        Color color,
        float scale)
    {
        Vector2 size = font.MeasureString(text) *
            (scale * FontSupersample);

        int x = (int)(bounds.X + (bounds.Width - size.X) / 2f);
        int y = (int)(bounds.Y + (bounds.Height - size.Y) / 2f);

        DrawText(spriteBatch, text, x, y, color, scale);
    }

    private static void FillRoundedRectangle(
    SpriteBatch spriteBatch,
    Rectangle bounds,
    Color color,
    int radius)
    {
        radius = Math.Clamp(
            radius,
            0,
            Math.Min(bounds.Width, bounds.Height) / 2);

        for (int y = 0; y < bounds.Height; y++)
        {
            int distanceFromCorner = Math.Min(
                y,
                bounds.Height - 1 - y);

            int inset = 0;

            if (distanceFromCorner < radius)
            {
                float dy = radius - distanceFromCorner - 0.5f;

                inset = radius - (int)Math.Floor(
                    Math.Sqrt(radius * radius - dy * dy));
            }

            Fill(
                spriteBatch,
                new Rectangle(
                    bounds.X + inset,
                    bounds.Y + y,
                    bounds.Width - inset * 2,
                    1),
                color);
        }
    }
    private static void Fill(
        SpriteBatch spriteBatch,
        Rectangle bounds,
        Color color)
    {
        spriteBatch.Draw(
            SpriteAtlas.Texture,
            bounds,
            SpriteAtlas.WhitePixel,
            color);
    }
}
