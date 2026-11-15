using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Collisions;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    // A short, non-colliding delivery animation; gameplay is paused while it runs.
    internal sealed class BazookaDelivery
    {
        private const float ApproachSeconds = 0.8f;
        private const float HandoffSeconds = 0.7f;
        private const float Duration = 2.3f;
        private const int SpriteSize = 88;
        private readonly Texture2D texture;
        private float elapsed;
        private Vector2 start;
        private Vector2 destination;
        private bool fromRight;

        public BazookaDelivery(Texture2D texture)
        {
            this.texture = texture;
        }
        public bool Active { get; private set; }

        public bool Start(IPlayer player, Rectangle view, System.Collections.Generic.IReadOnlyList<IBlock> blocks)
        {
            var feet = new Point((int)(player.Position.X + player.Width / 2f), (int)(player.Position.Y + player.Height));
            Rectangle playerSpace = new Rectangle(-player.Width / 2, -player.Height, player.Width, player.Height);
            Rectangle helperSpace = new Rectangle(-SpriteSize / 2, -SpriteSize, SpriteSize, SpriteSize);
            var stage = new SelfieStage(blocks, view);
            if (!stage.TryFindAt(feet, playerSpace, helperSpace, out SelfieMarks marks)) return false;
            elapsed = 0;
            Active = true;
            fromRight = marks.ClickFeet.X > marks.PlayerFeet.X;
            start = new Vector2(marks.ClickStart.X - SpriteSize / 2, marks.ClickStart.Y - SpriteSize);
            destination = new Vector2(marks.ClickFeet.X - SpriteSize / 2, marks.ClickFeet.Y - SpriteSize);
            return true;
        }

        public void Update(GameTime time)
        {
            elapsed += (float)time.ElapsedGameTime.TotalSeconds;
            if (elapsed >= Duration) Active = false;
        }

        public void Cancel() => Active = false;

        public void Draw(SpriteBatch batch)
        {
            if (!Active) return;
            float amount = elapsed < ApproachSeconds ? elapsed / ApproachSeconds
                : elapsed < ApproachSeconds + HandoffSeconds ? 1f
                : 1f - (elapsed - ApproachSeconds - HandoffSeconds) / ApproachSeconds;
            bool handoff = elapsed >= ApproachSeconds && elapsed < ApproachSeconds + HandoffSeconds;
            int frame = handoff ? 2 : (int)(elapsed * 8) % 2;
            Rectangle source = new Rectangle(frame % 2 * 256, frame / 2 * 256, 256, 256);
            Vector2 position = Vector2.Lerp(start, destination, MathHelper.Clamp(amount, 0f, 1f));
            batch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, SpriteSize, SpriteSize), source,
                Color.White, 0f, Vector2.Zero, fromRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }
    }
}
