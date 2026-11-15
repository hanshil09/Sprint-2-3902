using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    internal sealed class BazookaAbility
    {
        private readonly BazookaDelivery delivery;
        private readonly Texture2D weapon;
        private bool aimUp;
        private bool needsSpace;

        public BazookaAbility(Texture2D helper, Texture2D weapon)
        {
            delivery = new BazookaDelivery(helper);
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
            if (Active || player.IsInBallForm || !player.Inventory.HasBazookaCall || player.Inventory.BazookaLoaded) return;
            needsSpace = !delivery.Start(player, view, blocks);
            if (!needsSpace) player.UseBazookaCall();
        }

        public void Fire(IPlayer player)
        {
            if (!Active) player.FireBazooka(aimUp);
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
            if (Active) return "BAZOOKA DELIVERY...";
            if (player.Inventory.BazookaLoaded) return $"BAZOOKA: {player.Inventory.BazookaAmmo} - 9 / SHIFT+9";
            if (player.Inventory.HasBazookaCall) return needsSpace ? "BAZOOKA: NEED FLAT SPACE" : "BAZOOKA CALL: press 8";
            return "BAZOOKA: empty";
        }

        public void Draw(SpriteBatch batch, IPlayer player)
        {
            if (player.Inventory.BazookaLoaded && !player.IsInBallForm && !Active)
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
