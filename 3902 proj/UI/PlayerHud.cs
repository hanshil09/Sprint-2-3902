using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;

namespace TransformersGame.UI
{
    // Reads live player state; inventory and combat remain owned by the player.
    internal sealed class PlayerHud
    {
        private const int PanelLeft = 12;
        private const int PanelTop = 8;
        private const int PanelWidth = 936;
        private const int PanelHeight = 64;
        private const int HealthHeartCount = 5;
        private const int MedkitsLeft = 220;
        private const int ShieldsLeft = 350;
        private const int GemsLeft = 490;
        private const int AmmoLeft = 780;
        private const int BombPacksLeft = 630;
        private const int ContentLeft = 26;
        private const int LabelY = 14;
        private const int ValueY = 39;
        private const int HeartsY = 40;
        private const int RoomStatusY = 78;
        private const int PowerStatusLeft = 700;
        private const int ShadowOffset = 5;
        private const int AccentWidth = 6;
        private const int BorderHeight = 2;
        private const float LabelScale = 0.58f;
        private const float CountScale = 0.65f;
        private const float RoomStatusScale = 0.42f;
        private static readonly Color BackgroundColor = new Color(10, 12, 20, 225);
        private static readonly Color HeadingColor = new Color(255, 232, 190);
        private static readonly Color TextColor = new Color(220, 194, 151);
        private static readonly Color PowerColor = new Color(231, 181, 76);
        private static readonly Color AccentColor = new Color(156, 104, 55);
        private static readonly Color ShadowColor = new Color(20, 12, 8, 100);
        private static readonly Color BorderColor = new Color(156, 104, 55, 120);
        private static readonly Color HealthColor = Color.Crimson;
        private static readonly Color ShieldColor = Color.Cyan;
        private static readonly Color GemColor = Color.HotPink;
        private readonly SpriteFont font;

        public PlayerHud(SpriteFont font)
        {
            this.font = font;
        }

        public void Draw(SpriteBatch spriteBatch, IPlayer player, string roomName, string roomStatus)
        {
            Fill(spriteBatch, new Rectangle(PanelLeft, PanelTop, PanelWidth, PanelHeight), BackgroundColor);
            DrawPanel(spriteBatch, new Rectangle(PanelLeft, PanelTop, PanelWidth, PanelHeight));
            DrawHealth(spriteBatch, player);
            DrawInventory(spriteBatch, player);
            DrawRoomStatus(spriteBatch, player, roomName, roomStatus);
        }

        private void DrawHealth(SpriteBatch spriteBatch, IPlayer player)
        {
            DrawText(spriteBatch, "HEALTH", ContentLeft, LabelY, HeadingColor, LabelScale);
            HeartMeter.Draw(spriteBatch, new Vector2(ContentLeft, HeartsY), player.Health, player.MaxHealth,
                HealthHeartCount, HealthColor);
        }

        private void DrawInventory(SpriteBatch spriteBatch, IPlayer player)
        {
            DrawCounter(spriteBatch, "MED KITS", player.MedkitCount.ToString(CultureInfo.InvariantCulture), MedkitsLeft, HeadingColor);
            string shieldStatus = player.ShieldCount.ToString(CultureInfo.InvariantCulture);
            if (player.IsShieldActive)
            {
                shieldStatus += "  ACTIVE";
            }
            DrawCounter(spriteBatch, "SHIELDS", shieldStatus, ShieldsLeft, ShieldColor,
                player.IsShieldActive ? ShieldColor : Color.White);
            DrawCounter(spriteBatch, "PINK GEMS", player.EnergonCount.ToString(CultureInfo.InvariantCulture), GemsLeft, GemColor);
            DrawCounter(spriteBatch, "BOMB PACKS", $"{player.BombLevel}/{PlayerInventory.MaximumBombLevel}", BombPacksLeft, PowerColor);
            // Player shots do not consume a resource in the current weapon system.
            DrawCounter(spriteBatch, "FIREBALL AMMO", "Unlimited", AmmoLeft, PowerColor);
        }

        private void DrawCounter(SpriteBatch spriteBatch, string label, string value, int left, Color labelColor,
            Color? valueColor = null)
        {
            DrawText(spriteBatch, label, left, LabelY, labelColor, LabelScale);
            DrawText(spriteBatch, value, left, ValueY, valueColor ?? Color.White, CountScale);
        }

        private void DrawRoomStatus(SpriteBatch spriteBatch, IPlayer player, string roomName, string roomStatus)
        {
            DrawText(spriteBatch, $"{roomStatus}  {roomName}  |  F1 / H Help", ContentLeft,
                RoomStatusY, TextColor, RoomStatusScale);
            DrawText(spriteBatch, PowerUpText(player), PowerStatusLeft,
                RoomStatusY, PowerColor, RoomStatusScale);
        }

        private static string PowerUpText(IPlayer player)
        {
            string text = player.IsInvincible ? "INVINCIBLE " : string.Empty;
            text += player.SpeedScale > 1f ? "TURBO " : string.Empty;
            return text;
        }

        private static void Fill(SpriteBatch spriteBatch, Rectangle bounds, Color color)
        {
            spriteBatch.Draw(SpriteAtlas.Texture, bounds, SpriteAtlas.WhitePixel, color);
        }

        private static void DrawPanel(SpriteBatch spriteBatch, Rectangle bounds)
        {
            Fill(spriteBatch, new Rectangle(bounds.X + ShadowOffset, bounds.Y + ShadowOffset, bounds.Width, bounds.Height), ShadowColor);
            Fill(spriteBatch, new Rectangle(bounds.X, bounds.Y, AccentWidth, bounds.Height), AccentColor);
            Fill(spriteBatch, new Rectangle(bounds.X, bounds.Bottom - BorderHeight, bounds.Width, BorderHeight), BorderColor);
        }

        private void DrawText(SpriteBatch spriteBatch, string text, int x, int y, Color color, float scale)
        {
            spriteBatch.DrawString(font, text, new Vector2(x, y), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
