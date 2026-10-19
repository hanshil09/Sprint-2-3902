using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.UI
{
    internal sealed class ControlsOverlay : IDisposable
    {
        private static readonly Color PanelColor = new Color(8, 13, 24, 225);
        private static readonly Color AccentColor = new Color(42, 210, 194);
        private static readonly Color HeadingColor = new Color(245, 249, 255);
        private static readonly Color TextColor = new Color(190, 204, 220);

        private readonly SpriteFont font;
        private readonly Texture2D pixel;
        private bool showHelp;

        public void ToggleHelp() => showHelp = !showHelp;

        public ControlsOverlay(SpriteFont font, GraphicsDevice graphicsDevice)
        {
            this.font = font;
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public void DrawMenu(SpriteBatch spriteBatch, Viewport viewport, bool gameOver = false)
        {
            const int width = 800;
            const int height = 450;
            int left = (viewport.Width - width) / 2;
            int top = (viewport.Height - height) / 2;

            DrawPanel(spriteBatch, new Rectangle(left, top, width, height));
            DrawText(spriteBatch, gameOver ? "GAME OVER" : "TRANSFORMERS DEMO", left + 28, top + 24, HeadingColor, 1.25f);
            DrawText(spriteBatch, "HOW TO PLAY", left + 30, top + 60, AccentColor, 0.75f);
            DrawText(spriteBatch, "Climb to the GOLD exit. The CYAN door returns to the previous room.", left + 30, top + 88, TextColor, 0.58f);
            DrawKeyLine(spriteBatch, gameOver ? "R" : "ENTER", gameOver ? "Reset to start menu" : "Start game", left + 30, top + 122);
            DrawKeyLine(spriteBatch, "A / D or arrows", "Move left / right", left + 30, top + 160);
            DrawKeyLine(spriteBatch, "W / UP / J", "Jump between platforms", left + 30, top + 188);
            DrawKeyLine(spriteBatch, "Z / N", "Shoot enemies and breakable crates", left + 30, top + 216);
            DrawKeyLine(spriteBatch, "2", "Drop a bomb (falls onto solid blocks)", left + 30, top + 244);
            DrawKeyLine(spriteBatch, "3", "Use collected medkit: restore 100 health", left + 30, top + 272);
            DrawKeyLine(spriteBatch, "4", "Use collected shield: 5 seconds protection", left + 30, top + 300);
            DrawKeyLine(spriteBatch, "SPACE", "Switch robot / vehicle form", left + 30, top + 328);
            DrawKeyLine(spriteBatch, "F1", "Show all controls during play", left + 30, top + 356);
            DrawKeyLine(spriteBatch, "Q / ESC", "Quit game", left + 30, top + 394);
        }

        public void DrawGameplay(SpriteBatch spriteBatch, IPlayer player, string roomName, int roomNumber, int roomCount)
        {
            DrawPanel(spriteBatch, new Rectangle(12, 8, 936, 64));
            DrawText(spriteBatch, $"{roomNumber}/{roomCount}  {roomName}     HP {player.Health}/{player.MaxHealth}     F1: help", 26, 16, HeadingColor, 0.62f);
            DrawText(spriteBatch, "GOLD door: next room    CYAN door: return    Mouse clicks: test rooms", 26, 43, TextColor, 0.53f);
            if (!showHelp) return;
            Rectangle panel = new Rectangle(16, 16, 560, 262);
            DrawPanel(spriteBatch, panel);
            DrawText(spriteBatch, "CONTROLS", 34, 30, HeadingColor, 0.9f);
            DrawText(spriteBatch, "MOVE", 34, 63, AccentColor, 0.65f);
            DrawText(spriteBatch, "A / D or arrows     W / UP / J jump     S / DOWN face", 105, 63, TextColor, 0.58f);
            DrawText(spriteBatch, "ACTION", 34, 91, AccentColor, 0.65f);
            DrawText(spriteBatch, "Z/N shot K angle 1 orb 2 bomb 5 pierce 6 spread SPACE form", 105, 91, TextColor, 0.48f);
            DrawText(spriteBatch, "CYCLE", 34, 119, AccentColor, 0.65f);
            DrawText(spriteBatch, "Left/Right click or Page Up/Down: test rooms", 105, 119, TextColor, 0.55f);
            DrawText(spriteBatch, "ITEMS", 34, 147, AccentColor, 0.65f);
            DrawText(spriteBatch, "3 medkit (restore 100)     4 shield (5 seconds)", 105, 147, TextColor, 0.55f);
            DrawText(spriteBatch, "STATUS", 34, 175, AccentColor, 0.65f);
            string shieldStatus = player.IsShieldActive ? "ACTIVE" : "inactive";
            string inventory = $"HP {player.Health}/{player.MaxHealth}   Medkits {player.MedkitCount}   Shields {player.ShieldCount} ({shieldStatus})";
            DrawText(spriteBatch, inventory, 105, 175, TextColor, 0.55f);
            DrawText(spriteBatch, "ROOM", 34, 203, AccentColor, 0.65f);
            DrawText(spriteBatch, "LEFT / RIGHT CLICK or PAGE UP / DOWN", 105, 203, TextColor, 0.52f);
            DrawText(spriteBatch, "SYSTEM", 34, 231, AccentColor, 0.65f);
            DrawText(spriteBatch, "E damage     R reset     Q / ESC quit", 105, 231, TextColor, 0.58f);
        }

        public void Dispose()
        {
            pixel.Dispose();
        }

        private void DrawPanel(SpriteBatch spriteBatch, Rectangle bounds)
        {
            spriteBatch.Draw(pixel, new Rectangle(bounds.X + 5, bounds.Y + 5, bounds.Width, bounds.Height), new Color(0, 0, 0, 90));
            spriteBatch.Draw(pixel, bounds, PanelColor);
            spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, 6, bounds.Height), AccentColor);
            spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Bottom - 2, bounds.Width, 2), new Color(42, 210, 194, 100));
        }

        private void DrawKeyLine(SpriteBatch spriteBatch, string key, string action, int x, int y)
        {
            DrawText(spriteBatch, key, x, y, AccentColor, 0.65f);
            DrawText(spriteBatch, action, x + 220, y, TextColor, 0.65f);
        }

        private void DrawText(SpriteBatch spriteBatch, string text, int x, int y, Color color, float scale)
        {
            spriteBatch.DrawString(font, text, new Vector2(x, y), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
