using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities
{
    internal sealed class ShotgunAbility
    {
        private readonly ShotgunDelivery delivery;
        private readonly Texture2D weapon;
        private bool aimUp;
        private bool needsSpace;

        public ShotgunAbility(Texture2D helper, Texture2D weapon)
        {
            delivery = new ShotgunDelivery(helper);
            this.weapon = weapon;
        }

        public bool Active => delivery.Active;

        public void SetAim(KeyboardState keys)
        {
            aimUp = keys.IsKeyDown(Keys.W) || keys.IsKeyDown(Keys.Up)
                || keys.IsKeyDown(Keys.LeftShift) || keys.IsKeyDown(Keys.RightShift);
        }

        public void Call(IPlayer player, Rectangle view, IReadOnlyList<IBlock> blocks)
        {
            if (Active || player.IsInBallForm || !player.Inventory.HasShotgunCall || player.Inventory.ShotgunLoaded) return;
            needsSpace = !delivery.Start(player, view, blocks);
            if (!needsSpace) player.UseShotgunCall();
        }

        public void Fire(IPlayer player)
        {
            if (!Active) player.FireShotgun(aimUp);
        }

        public void Update(GameTime time) => delivery.Update(time);

        public void Reset()
        {
            delivery.Cancel();
            needsSpace = false;
            aimUp = false;
        }

        public void ClearSpaceWarning() => needsSpace = false;

        public string Status(IPlayer player)
        {
            if (Active) return "Shotgun DELIVERY...";
            if (player.Inventory.ShotgunLoaded) return $"Shotgun: {player.Inventory.ShotgunAmmo} - 9 / SHIFT+9";
            if (player.Inventory.HasShotgunCall) return needsSpace ? "Shotgun: NEED FLAT SPACE" : "Shotgun CALL: press 8";
            return "Shotgun: empty";
        }

        public void Draw(SpriteBatch batch, IPlayer player)
        {
            if (player.Inventory.ShotgunLoaded && !player.IsInBallForm && !Active)
            {
                bool left = player.Facing == Direction.Left;
                int x = (int)player.Position.X + (left ? -30 : player.Width - 30);
                int y = (int)player.Position.Y + player.Height / 3;
                batch.Draw(weapon, new Vector2(x + 30, y + 15), null, Color.White,
                    aimUp ? -MathHelper.PiOver2 : 0f, new Vector2(32, 16), new Vector2(60f / 64, 30f / 32),
                    left && !aimUp ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }
            delivery.Draw(batch);
        }
    }
}
