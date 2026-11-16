using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities
{
    internal sealed class Door : IGameObject
    {
        private readonly ISprite lockedSprite;
        private readonly ISprite unlockedSprite;

        public Door(Vector2 position, DoorSpec spec, ISprite unlockedSprite, ISprite lockedSprite)
        {
            Position = position;
            Spec = spec;
            IsLocked = spec.IsLocked;
            this.unlockedSprite = unlockedSprite;
            this.lockedSprite = lockedSprite;
        }

        public DoorSpec Spec { get; }

        // Legacy relative direction (-1 previous, +1 next); 0 for targeted doors.
        public int Direction => Spec.Direction;

        // Absolute destination room, or null for a relative door.
        public int? TargetRoomIndex => Spec.TargetRoomIndex;

        // Starts from the spec but can be cleared once the player spends a key (unlock stays open).
        public bool IsLocked { get; set; }

        // True when this door was locked behind the player on entry. Passing back through it requires
        // holding a key but does not spend one, unlike a door the player unlocks for the first time.
        public bool LockedBehind { get; set; }

        public Vector2 Position { get; set; }
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, unlockedSprite.Width, unlockedSprite.Height);
        public void Update(GameTime gameTime)
        {
            unlockedSprite.Update(gameTime);
            lockedSprite.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch) => (IsLocked ? lockedSprite : unlockedSprite).Draw(spriteBatch, Position);
    }
}
