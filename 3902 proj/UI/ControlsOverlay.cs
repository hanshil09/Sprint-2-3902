using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.UI
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
            const int width = 800;
            const int height = 500;
            int left = (viewport.Width - width) / 2;
            int top = (viewport.Height - height) / 2;
            DrawPanel(spriteBatch, new Rectangle(left, top, width, height));
            DrawText(spriteBatch, MenuTitle(screen), left + 28, top + 22, HeadingColor, 1.25f);
            if (screen == GameState.Victory)
            {
                DrawText(spriteBatch, $"The Warlord is defeated and the beacon is lit. Energon collected: {player.Inventory.EnergonCount}", left + 30, top + 66, PowerColor, 0.55f);
            }
            DrawText(spriteBatch, "Climb to the GOLD exit. Defeat the boss in the last room, then touch the beacon.", left + 30, top + 92, TextColor, 0.52f);
            bool ended = screen != GameState.StartMenu;
            DrawKeyLine(spriteBatch, ended ? "R" : "ENTER", ended ? "Reset to start menu" : "Start game", left + 30, top + 124);
            DrawKeyLine(spriteBatch, "A / D or arrows", "Move left / right", left + 30, top + 156);
            DrawKeyLine(spriteBatch, "W / UP / J", "Jump between platforms", left + 30, top + 184);
            DrawKeyLine(spriteBatch, "X / TAB", "Fire selected shot / choose fireball and view map", left + 30, top + 212);
            DrawKeyLine(spriteBatch, "7", "Throw boomerang", left + 30, top + 240);
            DrawKeyLine(spriteBatch, "2", "Drop a bomb (unlimited; Bomb Packs make the blast bigger)", left + 30, top + 268);
            DrawKeyLine(spriteBatch, "3  /  4", "Use a stored medkit  /  shield", left + 30, top + 296);
            DrawKeyLine(spriteBatch, "SPACE", "Switch robot / vehicle form", left + 30, top + 324);
            DrawKeyLine(spriteBatch, "B", "Barrel Storm (falling barrels, damages enemies, 14-second cooldown)", left + 30, top + 352);
            DrawKeyLine(spriteBatch, "T/Y  U/I  O/P", "Preview every block, item and enemy", left + 30, top + 380);
            DrawKeyLine(spriteBatch, "F1 / H   M", "Show controls / mute all audio", left + 30, top + 408);
            DrawKeyLine(spriteBatch, "Q / ESC", "Quit game", left + 30, top + 436);
        }

        public void DrawGameplayHelp(SpriteBatch spriteBatch, IPlayer player, string objective)
        {
            if (showHelp)
            {
                DrawHelp(spriteBatch, player, objective);
            }
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
                _ => "TRANSFORMERS DEMO"
            };
        }

        private void DrawHelp(SpriteBatch spriteBatch, IPlayer player, string objective)
        {
            Rectangle panel = new Rectangle(16, 104, 800, 414);
            spriteBatch.Draw(pixel, panel, new Color(10, 12, 20, 245));
            DrawPanel(spriteBatch, panel);
            DrawText(spriteBatch, "HELP - F1 / H TO CLOSE", 34, 118, HeadingColor, 0.9f);
            DrawHelpLine(spriteBatch, "MOVE", "A / D or arrows     W / UP / J jump     S / DOWN face", 151);
            DrawHelpLine(spriteBatch, "FIRE", "X fire   Shift+X aim up   TAB choose shot   7 boomerang", 179);
            DrawHelpLine(spriteBatch, "ITEMS", "2 bomb     3 medkit (full heal)     4 shield (5 seconds)", 207);
            DrawHelpLine(spriteBatch, "ABILITY", "B Barrel Storm (9 falling barrels; 14-second cooldown)", 235);
            DrawHelpLine(spriteBatch, "STATUS", $"Medkits {player.Inventory.MedkitCount}   Shields {player.Inventory.ShieldCount}   Blast Lv {player.Inventory.BombLevel}   Energon {player.Inventory.EnergonCount}", 263);
            DrawHelpLine(spriteBatch, "SHOW", "T/Y blocks   U/I items   O/P enemies   SPACE transform", 291);
            DrawHelpLine(spriteBatch, "ROOM", "LEFT / RIGHT CLICK or PAGE UP / DOWN", 319);
            DrawHelpLine(spriteBatch, "SYSTEM", "F2 selfie  F3 gallery  8 call  9 fire  SHIFT+9 up  M mute", 347);
            DrawHelpLine(spriteBatch, "CRUMBLE", "Coral 0.9s   Gold 1.5s   Purple 2.3s   Flash, fall, return", 375);
            DrawHelpLine(spriteBatch, "CRATES", "Green: Health   Gold: Energon   Orange-red: Morph Ball bomb", 403);
            DrawHelpLine(spriteBatch, "CRATES", "Metal cases: roll into them   Green / gold: shoot or bomb", 431);
            DrawHelpLine(spriteBatch, "GOAL", objective, 459);
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
