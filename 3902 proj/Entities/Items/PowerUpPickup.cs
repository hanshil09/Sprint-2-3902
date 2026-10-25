using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
      internal sealed class PowerUpPickup : IPowerUp
    {
        private readonly ISprite sprite;
        private readonly ItemKind kind;
        private readonly ItemBob bob = new ItemBob();

        public PowerUpPickup(Vector2 position, ISprite sprite, ItemKind kind)
        {
            Position = position;
            this.sprite = sprite;
            this.kind = kind;
        }

        public Vector2 Position { get; set; }

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);

        // The effect is applied through Apply, because a decorator must replace the game's player reference.
        public bool TryCollect(IPlayer player)
        {
            return true;
        }

        public IPlayer Apply(IPlayer player, IPlayerHost host)
        {
            return kind == ItemKind.SparkCore
                ? Decorate<InvinciblePlayer>(player, () => new InvinciblePlayer(player, host))
                : Decorate<TurboPlayer>(player, () => new TurboPlayer(player, host));
        }

        public void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
            bob.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch) => sprite.Draw(spriteBatch, Position + bob.Offset);

        private static IPlayer Decorate<T>(IPlayer player, System.Func<T> create) where T : PlayerDecorator
        {
            T existing = PlayerDecorator.Find<T>(player);
            if (existing == null)
            {
                return create();
            }
            existing.Refresh();
            return player;
        }
    }
}
