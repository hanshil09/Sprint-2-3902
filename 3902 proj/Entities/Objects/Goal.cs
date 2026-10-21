using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
  
    internal sealed class Goal : IPlayerTrigger
    {
        private readonly ISprite lockedSprite;
        private readonly ISprite activeSprite;

        public Goal(Vector2 position)
        {
            Position = position;
            lockedSprite = BlockSpriteFactory.Instance.CreateGoalSprite(false);
            activeSprite = BlockSpriteFactory.Instance.CreateGoalSprite(true);
        }

        public Vector2 Position { get; set; }

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, activeSprite.Width, activeSprite.Height);

        public bool IsUnlocked { get; private set; }

        public bool IsReached { get; private set; }

        public void SetUnlocked(bool unlocked) => IsUnlocked = unlocked;

        public void OnPlayerContact(IPlayer player)
        {
            if (IsUnlocked)
            {
                IsReached = true;
            }
        }

        public void Update(GameTime gameTime) => activeSprite.Update(gameTime);

        public void Draw(SpriteBatch spriteBatch) => (IsUnlocked ? activeSprite : lockedSprite).Draw(spriteBatch, Position);
    }
}
