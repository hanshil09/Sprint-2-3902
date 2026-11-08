
using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;

namespace TransformersGame.UI
{
    internal sealed class PlayerHud
    {
        private const int PanelLeft = 12;
        private const int PanelTop = 8;
        private const int PanelWidth = 936;
        private const int PanelHeight = 52;

        // Character portrait.
        private const int PortraitX = 18;
        private const int PortraitY = 12;
        private const int PortraitSize = 44;

        private static readonly Rectangle PortraitSource =
            new Rectangle(878, 78, 78, 78);

        private const int HealthHeartCount = 5;

        // Inventory columns.
        private const int MedkitsLeft = 260;
        private const int ShieldsLeft = 390;
        private const int GemsLeft = 520;
        private const int BombPacksLeft = 650;
        private const int AmmoLeft = 790;

        private const int ContentLeft = 80;
        private const int LabelY = 13;
        private const int ValueY = 33;
        private const int HeartsY = 33;

        private const int RoomStatusY = 66;
        private const int HelpStatusLeft = 440;
        private const int PowerStatusLeft = 790;

        private const int IconPixelSize = 2;
        private const int IconTextSpacing = 6;
        private const int IconSize = 16;
        private const int IconOutlineThickness = 1;

        private const float LabelScale = 0.52f;
        private const float CountScale = 0.65f;
        private const float RoomStatusScale = 0.42f;

        // Warm brown and brass palette.
        private static readonly Color BackgroundColor =
            new Color(55, 34, 23, 250);

        private static readonly Color HeadingColor =
            new Color(255, 225, 177);

        private static readonly Color TextColor =
            new Color(226, 201, 164);

        private static readonly Color PowerColor =
            new Color(231, 181, 76);

        private static readonly Color AccentColor =
            new Color(169, 115, 57);

        private static readonly Color BorderColor =
            new Color(190, 139, 73, 210);

        private static readonly Color DividerColor =
            new Color(210, 160, 100, 75);

        // Inventory icon outline.
        private static readonly Color IconOutlineColor =
    new Color(105, 69, 39);

        private static readonly Color HealthColor = Color.Crimson;
        private static readonly Color ShieldColor = Color.Cyan;
        private static readonly Color GemColor = Color.HotPink;

        // Pixel-art inventory icons.
        private static readonly string[] MedkitIcon =
        {
            "...XX...",
            "...XX...",
            "XXXXXXXX",
            "XXXXXXXX",
            "...XX...",
            "...XX...",
            "...XX...",
            "........"
        };

        private static readonly string[] ShieldIcon =
        {
            "..XXXX..",
            ".XXXXXX.",
            "XXXXXXXX",
            "XXXXXXXX",
            ".XXXXXX.",
            "..XXXX..",
            "...XX...",
            "........"
        };

        private static readonly string[] GemIcon =
        {
            "...XX...",
            "..XXXX..",
            ".XXXXXX.",
            "XXXXXXXX",
            ".XXXXXX.",
            "..XXXX..",
            "...XX...",
            "........"
        };

        private static readonly string[] BombIcon =
        {
            "....XX..",
            "...XX...",
            "..XXXX..",
            ".XXXXXX.",
            ".XXXXXX.",
            ".XXXXXX.",
            "..XXXX..",
            "...XX..."
        };

        private static readonly string[] FireballIcon =
        {
            "...XX...",
            "..XXXX..",
            ".XXXXXX.",
            ".XXXXXX.",
            "..XXXXX.",
            "...XXXX.",
            "....XX..",
            "........"
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
            string roomName,
            string roomStatus)
        {
            Rectangle panel = new Rectangle(
                PanelLeft,
                PanelTop,
                PanelWidth,
                PanelHeight);

            Fill(spriteBatch, panel, BackgroundColor);

            DrawPanelDetails(spriteBatch, panel);
            DrawPortrait(spriteBatch);
            DrawHealth(spriteBatch, player);
            DrawInventory(spriteBatch, player);
            DrawRoomStatus(spriteBatch, player, roomName, roomStatus);
        }

        private void DrawPortrait(SpriteBatch spriteBatch)
        {
            Rectangle destination = new Rectangle(
                PortraitX,
                PortraitY,
                PortraitSize,
                PortraitSize);

            // Dark outline.
            Fill(
                spriteBatch,
                new Rectangle(
                    destination.X - 2,
                    destination.Y - 2,
                    destination.Width + 4,
                    destination.Height + 4),
                new Color(25, 14, 9));

            // Brass frame.
            Fill(
                spriteBatch,
                new Rectangle(
                    destination.X - 1,
                    destination.Y - 1,
                    destination.Width + 2,
                    destination.Height + 2),
                AccentColor);

            spriteBatch.Draw(
                portraitTexture,
                destination,
                PortraitSource,
                Color.White);
        }

        private void DrawHealth(SpriteBatch spriteBatch, IPlayer player)
        {
            DrawText(
                spriteBatch,
                "HEALTH",
                ContentLeft,
                LabelY,
                HeadingColor,
                LabelScale);

            HeartMeter.Draw(
                spriteBatch,
                new Vector2(ContentLeft, HeartsY),
                player.Health,
                player.MaxHealth,
                HealthHeartCount,
                HealthColor);
        }

        private void DrawInventory(SpriteBatch spriteBatch, IPlayer player)
        {
            DrawCounter(
                spriteBatch,
                "MED KITS",
                player.MedkitCount.ToString(CultureInfo.InvariantCulture),
                MedkitsLeft,
                HeadingColor,
                AccentColor,
                MedkitIcon);

            string shieldStatus =
                player.ShieldCount.ToString(CultureInfo.InvariantCulture);

            if (player.IsShieldActive)
            {
                shieldStatus += " ACTIVE";
            }

            DrawCounter(
                spriteBatch,
                "SHIELDS",
                shieldStatus,
                ShieldsLeft,
                ShieldColor,
                ShieldColor,
                ShieldIcon,
                player.IsShieldActive ? ShieldColor : TextColor);

            DrawCounter(
                spriteBatch,
                "PINK GEMS",
                player.EnergonCount.ToString(CultureInfo.InvariantCulture),
                GemsLeft,
                GemColor,
                GemColor,
                GemIcon);

            DrawCounter(
                spriteBatch,
                "BOMB PACKS",
                $"{player.BombLevel}/{PlayerInventory.MaximumBombLevel}",
                BombPacksLeft,
                PowerColor,
                PowerColor,
                BombIcon);

            DrawCounter(
                spriteBatch,
                "FIREBALL AMMO",
                "Unlimited",
                AmmoLeft,
                PowerColor,
                PowerColor,
                FireballIcon);
        }

        private void DrawCounter(
            SpriteBatch spriteBatch,
            string label,
            string value,
            int left,
            Color labelColor,
            Color iconColor,
            string[] icon,
            Color? valueColor = null)
        {
            DrawText(
                spriteBatch,
                label,
                left,
                LabelY,
                labelColor,
                LabelScale);

            DrawIcon(
                spriteBatch,
                icon,
                left,
                ValueY + 1,
                iconColor);

            float textHeight = font.MeasureString(value).Y * CountScale;

            int textY = ValueY +
                (int)Math.Round((IconSize - textHeight) / 2f);

            DrawText(
                spriteBatch,
                value,
                left + IconSize + IconTextSpacing,
                textY,
                valueColor ?? TextColor,
                CountScale);
        }

        private void DrawIcon(
            SpriteBatch spriteBatch,
            string[] shape,
            int x,
            int y,
            Color color)
        {
            int rows = shape.Length;
            int columns = shape[0].Length;

            // Draw a thick black outline around the outside silhouette.
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (shape[row][column] != 'X')
                        continue;

                    int pixelX = x + column * IconPixelSize;
                    int pixelY = y + row * IconPixelSize;

                    // Top edge.
                    if (row == 0 || shape[row - 1][column] != 'X')
                    {
                        Fill(
                            spriteBatch,
                            new Rectangle(
                                pixelX,
                                pixelY - IconOutlineThickness,
                                IconPixelSize,
                                IconOutlineThickness),
                            IconOutlineColor);
                    }

                    // Bottom edge.
                    if (row == rows - 1 || shape[row + 1][column] != 'X')
                    {
                        Fill(
                            spriteBatch,
                            new Rectangle(
                                pixelX,
                                pixelY + IconPixelSize,
                                IconPixelSize,
                                IconOutlineThickness),
                            IconOutlineColor);
                    }

                    // Left edge.
                    if (column == 0 || shape[row][column - 1] != 'X')
                    {
                        Fill(
                            spriteBatch,
                            new Rectangle(
                                pixelX - IconOutlineThickness,
                                pixelY,
                                IconOutlineThickness,
                                IconPixelSize),
                            IconOutlineColor);
                    }

                    // Right edge.
                    if (column == columns - 1 || shape[row][column + 1] != 'X')
                    {
                        Fill(
                            spriteBatch,
                            new Rectangle(
                                pixelX + IconPixelSize,
                                pixelY,
                                IconOutlineThickness,
                                IconPixelSize),
                            IconOutlineColor);
                    }
                }
            }

            // Draw the colored icon pixels over the outline.
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (shape[row][column] != 'X')
                        continue;

                    Rectangle pixel = new Rectangle(
                        x + column * IconPixelSize,
                        y + row * IconPixelSize,
                        IconPixelSize,
                        IconPixelSize);

                    spriteBatch.Draw(
                        SpriteAtlas.Texture,
                        pixel,
                        SpriteAtlas.WhitePixel,
                        color);
                }
            }
        }

        private void DrawRoomStatus(
            SpriteBatch spriteBatch,
            IPlayer player,
            string roomName,
            string roomStatus)
        {
            DrawText(
                spriteBatch,
                $"{roomStatus}  {roomName}",
                ContentLeft,
                RoomStatusY,
                TextColor,
                RoomStatusScale);

            DrawText(
                spriteBatch,
                "F1 / H HELP",
                HelpStatusLeft,
                RoomStatusY,
                AccentColor,
                RoomStatusScale);

            string powerStatus = PowerUpText(player);

            if (!string.IsNullOrEmpty(powerStatus))
            {
                DrawText(
                    spriteBatch,
                    powerStatus,
                    PowerStatusLeft,
                    RoomStatusY,
                    PowerColor,
                    RoomStatusScale);
            }
        }

        private static string PowerUpText(IPlayer player)
        {
            string text = player.IsInvincible
                ? "INVINCIBLE "
                : string.Empty;

            text += player.SpeedScale > 1f
                ? "TURBO "
                : string.Empty;

            return text;
        }

        private static void DrawPanelDetails(
            SpriteBatch spriteBatch,
            Rectangle bounds)
        {
            // Thin top highlight.
            Fill(
                spriteBatch,
                new Rectangle(
                    bounds.X,
                    bounds.Y,
                    bounds.Width,
                    1),
                new Color(255, 225, 190, 65));

            // Brass bottom border.
            Fill(
                spriteBatch,
                new Rectangle(
                    bounds.X,
                    bounds.Bottom - 2,
                    bounds.Width,
                    2),
                BorderColor);

            // Left accent.
            Fill(
                spriteBatch,
                new Rectangle(
                    bounds.X,
                    bounds.Y,
                    3,
                    bounds.Height),
                AccentColor);

            // Inventory separators.
            int[] separators =
            {
                245, 375, 505, 635, 775
            };

            foreach (int x in separators)
            {
                Fill(
                    spriteBatch,
                    new Rectangle(
                        x,
                        bounds.Y + 7,
                        1,
                        bounds.Height - 14),
                    DividerColor);
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

        // Artificially bolds all HUD text using a one-pixel offset.
        private void DrawText(
            SpriteBatch spriteBatch,
            string text,
            int x,
            int y,
            Color color,
            float scale)
        {
            Vector2 position = new Vector2(x, y);

            spriteBatch.DrawString(
                font,
                text,
                position,
                color,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);

            spriteBatch.DrawString(
                font,
                text,
                position + new Vector2(1, 0),
                color,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        }
    }
}
