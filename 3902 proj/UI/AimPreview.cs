using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Core;
using ArmadilloGame.Entities;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.UI
{
    // Draws a dotted aim guide in world space for the equipped weapon: a straight line for the
    // pistol, shotgun, and boomerang, and a gravity arc for the molotov. The guide stops at the
    // first solid block or enemy it meets. Always shown, brighter while firing or charging, and
    // rendered inside the camera transform so it lines up with the world.
    internal sealed class AimPreview : IDisposable
    {
        private const int DotSize = 3;
        private const float DotSpacing = 14f;     // pixels between dots along a straight line
        private const int MaxDots = 120;
        private const float ArcTimeStep = 0.03f;  // seconds between sampled arc points

        // Premultiplied-alpha tints: multiplying scales RGB and A together, so these render at the
        // intended faintness under the game's premultiplied blend state. 0.20 = 20% idle opacity.
        private static readonly Color IdleColor = Color.White * 0.20f;
        private static readonly Color ActiveColor = new Color(255, 236, 150) * 0.65f;

        private readonly Texture2D pixel;

        public AimPreview(GraphicsDevice graphicsDevice)
        {
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        // Draws the guide for the player's current weapon. isBlocked reports whether a world point is
        // inside a solid block or an enemy, so the guide can stop where the shot/throw would land.
        public void Draw(SpriteBatch spriteBatch, IPlayer player, WeaponKind kind, float molotovCharge,
            bool active, Func<Point, bool> isBlocked)
        {
            if (player.IsInBallForm)
            {
                return;
            }

            Color color = active ? ActiveColor : IdleColor;
            Vector2 origin = player.Center;
            Vector2 aim = player.AimDirection;

            switch (kind)
            {
                case WeaponKind.Molotov:
                    DrawArc(spriteBatch, origin, aim * WeaponTuning.MolotovSpeed(molotovCharge), player.Gravity, color, isBlocked);
                    break;
                case WeaponKind.Boomerang:
                    DrawLine(spriteBatch, origin, PlayerAim.SnapToOctant(aim), WeaponTuning.BoomerangPreviewLength, color, isBlocked);
                    break;
                case WeaponKind.Shotgun:
                    DrawLine(spriteBatch, origin, aim, WeaponTuning.ShotgunRange, color, isBlocked);
                    break;
                default:
                    DrawLine(spriteBatch, origin, aim, WeaponTuning.PistolPreviewLength, color, isBlocked);
                    break;
            }
        }

        private void DrawLine(SpriteBatch spriteBatch, Vector2 origin, Vector2 direction, float length,
            Color color, Func<Point, bool> isBlocked)
        {
            int dots = Math.Min(MaxDots, (int)(length / DotSpacing));
            for (int i = 1; i <= dots; i++)
            {
                Vector2 point = origin + direction * (i * DotSpacing);
                if (isBlocked(point.ToPoint()))
                {
                    return;
                }
                DrawDot(spriteBatch, point, color);
            }
        }

        // Samples the same projectile motion the molotov uses (constant X velocity, gravity on Y),
        // stopping at the first block or enemy the arc meets.
        private void DrawArc(SpriteBatch spriteBatch, Vector2 origin, Vector2 launchVelocity, float gravity,
            Color color, Func<Point, bool> isBlocked)
        {
            Vector2 position = origin;
            Vector2 velocity = launchVelocity;
            for (int i = 0; i < MaxDots; i++)
            {
                velocity.Y += gravity * ArcTimeStep;
                position += velocity * ArcTimeStep;
                if (isBlocked(position.ToPoint()))
                {
                    return;
                }
                DrawDot(spriteBatch, position, color);
            }
        }

        private void DrawDot(SpriteBatch spriteBatch, Vector2 worldPosition, Color color)
        {
            Rectangle rect = new Rectangle((int)worldPosition.X - DotSize / 2, (int)worldPosition.Y - DotSize / 2,
                DotSize, DotSize);
            spriteBatch.Draw(pixel, rect, color);
        }

        public void Dispose() => pixel.Dispose();
    }
}
