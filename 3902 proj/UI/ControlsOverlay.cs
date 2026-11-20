using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.UI
{
    internal sealed class ControlsOverlay : IDisposable
    {
        private static readonly Color AccentColor = new Color(156, 104, 55);
        private static readonly Color HeadingColor = new Color(255, 232, 190);
        private static readonly Color TextColor = new Color(220, 194, 151);
        private static readonly Color PowerColor = new Color(231, 181, 76);
        private const float SmallTextScale = 0.42f;

        private readonly SpriteFont font;
        private readonly Texture2D pixel;
        private bool showHelp;

        public ControlsOverlay(SpriteFont font, GraphicsDevice graphicsDevice)
        {
            this.font = font;
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public void ToggleHelp() => showHelp = !showHelp;

        public void DrawMenu(SpriteBatch spriteBatch, Viewport viewport, GameState screen, IPlayer player)
        {
            const int width = 820;
            const int height = 524;
            int left = (viewport.Width - width) / 2;
            int top = (viewport.Height - height) / 2;
            DrawPanel(spriteBatch, new Rectangle(left, top, width, height));
            DrawText(spriteBatch, MenuTitle(screen), left + 28, top + 16, HeadingColor, 1.15f);
            if (screen == GameState.Victory)
            {
                DrawText(spriteBatch, $"The Warlord is defeated and the beacon is lit. Energon collected: {player.Inventory.EnergonCount}", left + 30, top + 54, PowerColor, 0.52f);
            }
            DrawText(spriteBatch, "Climb to the GOLD exit. Defeat the boss in the last room, then touch the beacon.", left + 30, top + 76, TextColor, 0.5f);

            bool ended = screen != GameState.StartMenu;
            int x = left + 30;
            int y = top + 104;
            const int row = 23;
            const int gap = 4;

            DrawSection(spriteBatch, "GET STARTED", x, ref y);
            DrawKeyLine(spriteBatch, ended ? "R" : "ENTER", ended ? "Reset to start menu" : "Start game", x, y); y += row;

            y += gap;
            DrawSection(spriteBatch, "MOVE", x, ref y);
            DrawKeyLine(spriteBatch, "A / D or arrows", "Move left / right", x, y); y += row;
            DrawKeyLine(spriteBatch, "W / UP / SPACE", "Jump", x, y); y += row;
            DrawKeyLine(spriteBatch, "SHIFT", "Switch to / from ball form", x, y); y += row;

            y += gap;
            DrawSection(spriteBatch, "FIGHT", x, ref y);
            DrawKeyLine(spriteBatch, "1 / 2 / 3 / 4", "Equip boomerang / pistol / shotgun / molotov", x, y); y += row;
            DrawKeyLine(spriteBatch, "LEFT CLICK", "Fire the equipped weapon", x, y); y += row;
            DrawKeyLine(spriteBatch, "Ball form", "Roll into enemies for knockback damage", x, y); y += row;

            y += gap;
            DrawSection(spriteBatch, "ITEMS & SYSTEM", x, ref y);
            DrawKeyLine(spriteBatch, "C / V", "Use a stored medkit / shield", x, y); y += row;
            DrawKeyLine(spriteBatch, "TAB", "Map and weapon loadout", x, y); y += row;
            DrawKeyLine(spriteBatch, "`  (backtick)", "Toggle dev testing controls", x, y); y += row;
            DrawKeyLine(spriteBatch, "F1 / H   M   Q / ESC", "Help / mute / quit", x, y); y += row;
        }

        public void DrawGameplayHelp(SpriteBatch spriteBatch, IPlayer player, string objective)
        {
            if (showHelp)
            {
                DrawHelp(spriteBatch, player, objective);
            }
        }

        // A small panel listing the dev-only testing keys, shown while dev mode is on.
        public void DrawDevControls(SpriteBatch spriteBatch, Viewport viewport)
        {
            const int width = 360;
            const int height = 170;
            int left = viewport.Width - width - 16;
            int top = viewport.Height - height - 16;
            Rectangle panel = new Rectangle(left, top, width, height);
            spriteBatch.Draw(pixel, panel, new Color(10, 12, 20, 235));
            DrawPanel(spriteBatch, panel);
            DrawText(spriteBatch, "DEV MODE - ` TO CLOSE", left + 18, top + 14, HeadingColor, 0.62f);

            int x = left + 18;
            int y = top + 44;
            const int row = 24;
            DrawDevLine(spriteBatch, "T / Y", "Cycle block preview", x, y); y += row;
            DrawDevLine(spriteBatch, "U / I", "Cycle item preview", x, y); y += row;
            DrawDevLine(spriteBatch, "O / P", "Cycle enemy preview", x, y); y += row;
            DrawDevLine(spriteBatch, "PG UP / DN", "Previous / next room", x, y); y += row;
            DrawDevLine(spriteBatch, "E", "Damage the player", x, y);
        }

        private void DrawDevLine(SpriteBatch spriteBatch, string key, string action, int x, int y)
        {
            DrawText(spriteBatch, key, x, y, AccentColor, 0.5f);
            DrawText(spriteBatch, action, x + 130, y, TextColor, 0.5f);
        }

        public void DrawPanel(SpriteBatch spriteBatch, Rectangle bounds)
        {
            spriteBatch.Draw(
                pixel,
                new Rectangle(bounds.X + 5, bounds.Y + 5, bounds.Width, bounds.Height),
                new Color(20, 12, 8, 100));

            spriteBatch.Draw(
                pixel,
                new Rectangle(bounds.X, bounds.Y, 6, bounds.Height),
                AccentColor);

            spriteBatch.Draw(
                pixel,
                new Rectangle(bounds.X, bounds.Bottom - 2, bounds.Width, 2),
                new Color(156, 104, 55, 120));
        }

        public void DrawSmallText(SpriteBatch spriteBatch, string text, int x, int y, bool accent)
        {
            DrawText(spriteBatch, text, x, y, accent ? AccentColor : TextColor, SmallTextScale);
        }

        public void Dispose()
        {
            pixel.Dispose();
        }

        private static string MenuTitle(GameState screen)
        {
            return screen switch
            {
                GameState.GameOver => "GAME OVER",
                GameState.Victory => "MISSION COMPLETE",
                _ => "Armadillo DEMO"
            };
        }

        private void DrawHelp(SpriteBatch spriteBatch, IPlayer player, string objective)
        {
            Rectangle panel = new Rectangle(16, 104, 800, 360);
            spriteBatch.Draw(pixel, panel, new Color(10, 12, 20, 245));
            DrawPanel(spriteBatch, panel);
            DrawText(spriteBatch, "HELP - F1 / H TO CLOSE", 34, 118, HeadingColor, 0.9f);
            DrawHelpLine(spriteBatch, "MOVE", "A / D or arrows    W / UP / SPACE jump    SHIFT ball form", 151);
            DrawHelpLine(spriteBatch, "WEAPONS", "1 boomerang   2 pistol   3 shotgun   4 molotov", 179);
            DrawHelpLine(spriteBatch, "FIRE", "LEFT CLICK fires   roll into enemies in ball form", 207);
            DrawHelpLine(spriteBatch, "ITEMS", "C medkit (full heal)    V shield (5 seconds)", 235);
            DrawHelpLine(spriteBatch, "STATUS", $"Medkits {player.Inventory.MedkitCount}   Shields {player.Inventory.ShieldCount}   Energon {player.Inventory.EnergonCount}", 263);
            DrawHelpLine(spriteBatch, "SYSTEM", "TAB map / loadout   M mute   F2 selfie   F3 gallery", 291);
            DrawHelpLine(spriteBatch, "DEV", "` toggles dev testing keys", 319);
            DrawHelpLine(spriteBatch, "CRATES", "Green: Health   Gold: Energon   Orange-red: Morph Ball bomb", 347);
            DrawHelpLine(spriteBatch, "CRATES", "Metal cases: roll into them   Green / gold: shoot or bomb", 375);
            DrawHelpLine(spriteBatch, "GOAL", objective, 403);
        }

        private void DrawSection(SpriteBatch spriteBatch, string title, int x, ref int y)
        {
            DrawText(spriteBatch, title, x, y, PowerColor, 0.52f);
            y += 22;
        }

        private void DrawHelpLine(SpriteBatch spriteBatch, string label, string text, int y)
        {
            DrawText(spriteBatch, label, 34, y, AccentColor, 0.62f);
            DrawText(spriteBatch, text, 112, y, TextColor, 0.5f);
        }

        private void DrawKeyLine(SpriteBatch spriteBatch, string key, string action, int x, int y, int labelWidth = 220)
        {
            DrawText(spriteBatch, key, x, y, AccentColor, 0.62f);
            DrawText(spriteBatch, action, x + labelWidth, y, TextColor, 0.62f);
        }

        private void DrawText(SpriteBatch spriteBatch, string text, int x, int y, Color color, float scale)
        {
            spriteBatch.DrawString(font, text, new Vector2(x, y), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
