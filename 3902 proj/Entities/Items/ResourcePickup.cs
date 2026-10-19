using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    // A pickup whose effect is applied by the player's inventory or health (Health Cell, Energon, Bomb Pack...).
    internal sealed class ResourcePickup : IItem
    {
        private readonly ISprite sprite;
        private readonly ItemKind kind;
        private readonly ItemBob bob = new ItemBob();
        private bool collected;

        public ResourcePickup(Vector2 position, ISprite sprite, ItemKind kind)
        {
            Position = position;
            this.sprite = sprite;
            this.kind = kind;
        }

        public Vector2 Position { get; set; }

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);

        public void Collect(IPlayer player)
        {
            if (!collected)
            {
                player.AddItem(kind);
                collected = true;
            }
        }

        public void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
            bob.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch) => sprite.Draw(spriteBatch, Position + bob.Offset);
    }
}
