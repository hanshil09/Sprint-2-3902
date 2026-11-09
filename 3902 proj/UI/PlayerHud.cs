using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.UI;

internal sealed class PlayerHud
{
    private const int PanelLeft = 12;
    private const int PanelTop = 8;
    private const int PanelWidth = 936;
    private const int PanelHeight = 52;

    private const int HealthHeartCount = 5;

    private const int ContentLeft = 80;
    private const int LabelY = 13;
    private const int ValueY = 33;
    private const int HeartsY = 33;
    private const int MedkitsLeft = 260;
    private const int ShieldsLeft = 390;
    private const int GemsLeft = 520;
    private const int BombPacksLeft = 650;
    private const int AmmoLeft = 790;

    private const int RoomStatusY = 66;
    private const int HelpStatusLeft = 440;
    private const int PowerStatusLeft = 790;

    private const int IconTextSpacing = 6;
    private const int DividerOffset = 15;
    private const int DividerInset = 7;

    private const float LabelScale = 0.52f;
    private const float CountScale = 0.65f;
    private const float RoomStatusScale = 0.42f;

    // Warm brown and brass palette.
    private static readonly Color HeadingColor = new Color(255, 225, 177);

    private static readonly Color TextColor = new Color(226, 201, 164);

    private static readonly Color PowerColor = new Color(231, 181, 76);

    private static readonly Color AccentColor = new Color(169, 115, 57);

    private static readonly Color DividerColor = new Color(210, 160, 100, 75);

    private static readonly Color ShieldColor = Color.Cyan;
    private static readonly Color GemColor = Color.HotPink;

    private readonly SpriteFont font;
    private readonly Texture2D portraitTexture;

    public PlayerHud(SpriteFont font, Texture2D portraitTexture)
    {
        this.font = font;
        this.portraitTexture = portraitTexture;
    }

    public void Draw(SpriteBatch spriteBatch, IPlayer player, string roomName, string roomStatus)
    {
        Rectangle panel = new Rectangle(PanelLeft, PanelTop, PanelWidth, PanelHeight);
        HudFrame.Draw(spriteBatch, panel, portraitTexture, AccentColor);
        DrawHealth(spriteBatch, player);
        DrawInventory(spriteBatch, player);
        DrawRoomStatus(spriteBatch, player, roomName, roomStatus);
    }

    private void DrawHealth(SpriteBatch spriteBatch, IPlayer player)
    {
        DrawText(spriteBatch, $"HEALTH {player.Health}/{player.MaxHealth}", ContentLeft, LabelY, HeadingColor, LabelScale);
        HeartMeter.Draw(spriteBatch, new Vector2(ContentLeft, HeartsY), player.Health,
            player.MaxHealth, HealthHeartCount, Color.Crimson);
    }

    private readonly record struct CounterStyle(string Label, int Left, Color LabelColor, Color IconColor, string[] Icon);

    private static readonly CounterStyle Medkits = new("MED KITS", MedkitsLeft, HeadingColor, AccentColor, HudIcon.MedkitIcon);
    private static readonly CounterStyle Shields = new("SHIELDS", ShieldsLeft, ShieldColor, ShieldColor, HudIcon.ShieldIcon);
    private static readonly CounterStyle Gems = new("PINK GEMS", GemsLeft, GemColor, GemColor, HudIcon.GemIcon);
    private static readonly CounterStyle BombPacks = new("BOMB PACKS", BombPacksLeft, PowerColor, PowerColor, HudIcon.BombIcon);
    private static readonly CounterStyle Ammo = new("FIREBALL AMMO", AmmoLeft, PowerColor, PowerColor, HudIcon.FireballIcon);

    private void DrawInventory(SpriteBatch spriteBatch, IPlayer player)
    {
        DrawCounter(spriteBatch, Medkits, player.MedkitCount.ToString(CultureInfo.InvariantCulture));
        string shieldStatus = player.ShieldCount.ToString(CultureInfo.InvariantCulture);
        Color shieldStatusColor = TextColor;
        if (player.IsShieldActive)
        {
            shieldStatus += " ACTIVE";
            shieldStatusColor = ShieldColor;
        }

        DrawCounter(spriteBatch, Shields, shieldStatus, shieldStatusColor);
        DrawCounter(spriteBatch, Gems, player.EnergonCount.ToString(CultureInfo.InvariantCulture));
        DrawCounter(spriteBatch, BombPacks, $"{player.BombLevel}/{PlayerInventory.MaximumBombLevel}");
        DrawCounter(spriteBatch, Ammo, "Unlimited");
    }

    private void DrawCounter(SpriteBatch spriteBatch, CounterStyle style, string value, Color? valueColor = null)
    {
        HudFrame.Fill(spriteBatch, new Rectangle(style.Left - DividerOffset, PanelTop + DividerInset,
            1, PanelHeight - 2 * DividerInset), DividerColor);
        DrawText(spriteBatch, style.Label, style.Left, LabelY, style.LabelColor, LabelScale);
        HudIcon.Draw(spriteBatch, style.Icon, new Point(style.Left, ValueY + 1), style.IconColor);

        Point iconSize = HudIcon.Size(style.Icon);
        float textHeight = font.MeasureString(value).Y * CountScale;
        int textY = ValueY + (int)Math.Round((iconSize.Y - textHeight) / 2f);
        DrawText(spriteBatch, value, style.Left + iconSize.X + IconTextSpacing, textY,
            valueColor ?? TextColor, CountScale);
    }

    private void DrawRoomStatus(SpriteBatch spriteBatch, IPlayer player, string roomName, string roomStatus)
    {
        DrawText(spriteBatch, $"{roomStatus}  {roomName}", ContentLeft, RoomStatusY, TextColor, RoomStatusScale);
        DrawText(spriteBatch, "TAB MAP  |  F1 / H HELP", HelpStatusLeft, RoomStatusY, AccentColor, RoomStatusScale);

        string powerStatus = string.Empty;
        if (player.IsInvincible)
        {
            powerStatus = "INVINCIBLE ";
        }

        if (player.SpeedScale > 1f)
        {
            powerStatus += "TURBO ";
        }
        if (powerStatus.Length > 0)
        {
            DrawText(spriteBatch, powerStatus, PowerStatusLeft, RoomStatusY, PowerColor, RoomStatusScale);
        }
    }

    // Draw a second copy one pixel to the right to give HUD text a heavier weight.
    private void DrawText(SpriteBatch spriteBatch, string text, int x, int y, Color color, float scale)
    {
        Vector2 position = new Vector2(x, y);
        spriteBatch.DrawString(font, text, position, color,
            0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        spriteBatch.DrawString(font, text, position + Vector2.UnitX, color,
            0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }
}
