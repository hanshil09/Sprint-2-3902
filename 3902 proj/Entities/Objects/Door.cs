using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    internal sealed class Door : IGameObject
    {
        private readonly ISprite sprite;

        public Door(Vector2 position, DoorSpec spec, ISprite sprite)
        {
            Position = position;
            Spec = spec;
            IsLocked = spec.IsLocked;
            this.sprite = sprite;
        }

        public DoorSpec Spec { get; }

        // Legacy relative direction (-1 previous, +1 next); 0 for targeted doors.
        public int Direction => Spec.Direction;

        // Absolute destination room, or null for a relative door.
        public int? TargetRoomIndex => Spec.TargetRoomIndex;

        // Starts from the spec but can be cleared once the player spends a key (unlock stays open).
        public bool IsLocked { get; set; }

        public Vector2 Position { get; set; }
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);
        public void Update(GameTime gameTime) => sprite.Update(gameTime);
        public void Draw(SpriteBatch spriteBatch) => sprite.Draw(spriteBatch, Position);
    }
}
