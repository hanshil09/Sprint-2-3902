using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities
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

        public ItemKind Kind => kind;

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);

        public bool TryCollect(IPlayer player)
        {
            if (collected || !player.TryAddItem(kind))
            {
                return false;
            }
            collected = true;
            return true;
        }

        public void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
            bob.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch) => sprite.Draw(spriteBatch, Position + bob.Offset);
    }
}
