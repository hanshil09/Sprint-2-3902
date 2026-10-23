using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.UI
{
    internal sealed class ControlsOverlay : IDisposable
    {
        private static readonly Color PanelColor = new Color(8, 13, 24, 225);
        private static readonly Color AccentColor = new Color(42, 210, 194);
        private static readonly Color HeadingColor = new Color(245, 249, 255);
        private static readonly Color TextColor = new Color(190, 204, 220);
        private static readonly Color PowerColor = new Color(255, 220, 90);
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
            const int width = 800;
            const int height = 500;
            int left = (viewport.Width - width) / 2;
            int top = (viewport.Height - height) / 2;
            DrawPanel(spriteBatch, new Rectangle(left, top, width, height));
            DrawText(spriteBatch, MenuTitle(screen), left + 28, top + 22, HeadingColor, 1.25f);
            if (screen == GameState.Victory)
            {
                DrawText(spriteBatch, $"The Warlord is defeated and the beacon is lit. Energon collected: {player.EnergonCount}", left + 30, top + 66, PowerColor, 0.55f);
            }
            DrawText(spriteBatch, "Climb to the GOLD exit. Defeat the boss in the last room, then touch the beacon.", left + 30, top + 92, TextColor, 0.52f);
            bool ended = screen != GameState.StartMenu;
            DrawKeyLine(spriteBatch, ended ? "R" : "ENTER", ended ? "Reset to start menu" : "Start game", left + 30, top + 124);
            DrawKeyLine(spriteBatch, "A / D or arrows", "Move left / right", left + 30, top + 156);
            DrawKeyLine(spriteBatch, "W / UP / J", "Jump between platforms", left + 30, top + 184);
            DrawKeyLine(spriteBatch, "Z / N   K", "Shoot   /   angled shot", left + 30, top + 212);
            DrawKeyLine(spriteBatch, "1  5  6  7", "Orb, piercing shot, spread shot, boomerang", left + 30, top + 240);
            DrawKeyLine(spriteBatch, "2", "Drop a bomb (unlimited; Bomb Packs make the blast bigger)", left + 30, top + 268);
            DrawKeyLine(spriteBatch, "3  /  4", "Use a stored medkit  /  shield", left + 30, top + 296);
            DrawKeyLine(spriteBatch, "SPACE", "Switch robot / vehicle form", left + 30, top + 324);
            DrawKeyLine(spriteBatch, "T/Y  U/I  O/P", "Preview every block, item and enemy", left + 30, top + 352);
            DrawKeyLine(spriteBatch, "F1", "Show all controls during play", left + 30, top + 380);
            DrawKeyLine(spriteBatch, "Q / ESC", "Quit game", left + 30, top + 416);
        }

        public void DrawGameplay(SpriteBatch spriteBatch, IPlayer player, string roomName, string roomStatus, string objective)
        {
            DrawPanel(spriteBatch, new Rectangle(12, 8, 936, 64));
            string status = $"{roomStatus}  {roomName}   HP {player.Health}/{player.MaxHealth}   Blast Lv {player.BombLevel}   Energon {player.EnergonCount}";
            DrawText(spriteBatch, status, 26, 14, HeadingColor, 0.58f);
            DrawText(spriteBatch, PowerUpText(player), 700, 14, PowerColor, 0.58f);
            DrawText(spriteBatch, objective, 26, 43, TextColor, 0.5f);
            if (showHelp)
            {
                DrawHelp(spriteBatch, player);
            }
        }

        public void DrawPanel(SpriteBatch spriteBatch, Rectangle bounds)
        {
            spriteBatch.Draw(pixel, new Rectangle(bounds.X + 5, bounds.Y + 5, bounds.Width, bounds.Height), new Color(0, 0, 0, 90));
            spriteBatch.Draw(pixel, bounds, PanelColor);
            spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, 6, bounds.Height), AccentColor);
            spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Bottom - 2, bounds.Width, 2), new Color(42, 210, 194, 100));
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
                _ => "TRANSFORMERS DEMO"
            };
        }

        private static string PowerUpText(IPlayer player)
        {
            string text = player.IsInvincible ? "INVINCIBLE " : string.Empty;
            text += player.SpeedScale > 1f ? "TURBO " : string.Empty;
            return text + (player.IsShieldActive ? "SHIELD" : string.Empty);
        }

        private void DrawHelp(SpriteBatch spriteBatch, IPlayer player)
        {
            Rectangle panel = new Rectangle(16, 80, 580, 290);
            DrawPanel(spriteBatch, panel);
            DrawText(spriteBatch, "CONTROLS", 34, 94, HeadingColor, 0.9f);
            DrawHelpLine(spriteBatch, "MOVE", "A / D or arrows     W / UP / J jump     S / DOWN face", 127);
            DrawHelpLine(spriteBatch, "FIRE", "Z/N shot  K angled  1 orb  5 pierce  6 spread  7 boomerang", 155);
            DrawHelpLine(spriteBatch, "ITEMS", "2 bomb     3 medkit (full heal)     4 shield (5 seconds)", 183);
            DrawHelpLine(spriteBatch, "STATUS", $"Medkits {player.MedkitCount}   Shields {player.ShieldCount}   Blast Lv {player.BombLevel}   Energon {player.EnergonCount}", 211);
            DrawHelpLine(spriteBatch, "SHOW", "T/Y blocks   U/I items   O/P enemies   SPACE transform", 239);
            DrawHelpLine(spriteBatch, "ROOM", "LEFT / RIGHT CLICK or PAGE UP / DOWN", 267);
            DrawHelpLine(spriteBatch, "SYSTEM", "E damage     R reset     Q / ESC quit", 295);
            DrawHelpLine(spriteBatch, "CRUMBLE", "Coral 0.6s   Gold 1.2s   Purple 2s   Flash, fall, return", 323);
        }

        private void DrawHelpLine(SpriteBatch spriteBatch, string label, string text, int y)
        {
            DrawText(spriteBatch, label, 34, y, AccentColor, 0.62f);
            DrawText(spriteBatch, text, 112, y, TextColor, 0.5f);
        }

        private void DrawKeyLine(SpriteBatch spriteBatch, string key, string action, int x, int y)
        {
            DrawText(spriteBatch, key, x, y, AccentColor, 0.62f);
            DrawText(spriteBatch, action, x + 220, y, TextColor, 0.62f);
        }

        private void DrawText(SpriteBatch spriteBatch, string text, int x, int y, Color color, float scale)
        {
            spriteBatch.DrawString(font, text, new Vector2(x, y), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
