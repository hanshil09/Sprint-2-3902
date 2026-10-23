using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Factories;
using TransformersGame.Interfaces;
using TransformersGame.States;

namespace TransformersGame.Entities
{
    internal sealed class CrumblingPlatform : IStandingBlock
    {
        private const double BlinkSeconds = 0.1;
        private readonly int tileCount;
        private readonly ISprite readySprite;
        private readonly ISprite warningSprite;
        private readonly ISprite reformSprite;
        private readonly CrumblingPlatformStateMachine stateMachine;
        private Rectangle playerBounds;
        private double time;

        public CrumblingPlatform(Vector2 position, int tileCount, CrumbleKind kind)
        {
            Position = position;
            this.tileCount = tileCount;
            Kind = kind;
            stateMachine = new CrumblingPlatformStateMachine(kind);
            readySprite = BlockSpriteFactory.Instance.CreateCrumblingSprite(kind, false, false);
            warningSprite = BlockSpriteFactory.Instance.CreateCrumblingSprite(kind, true, false);
            reformSprite = BlockSpriteFactory.Instance.CreateCrumblingSprite(kind, false, true);
        }

        public Vector2 Position { get; set; }
        public CrumbleKind Kind { get; }
        public bool IsSolid => stateMachine.IsSolid;
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y,
            tileCount * BlockSpriteFactory.BlockSize, BlockSpriteFactory.BlockSize);

        public void OnStand() => stateMachine.OnStand();
        public void SetPlayerBounds(Rectangle bounds) => playerBounds = bounds;

        public void Update(GameTime gameTime)
        {
            time += gameTime.ElapsedGameTime.TotalSeconds;
            stateMachine.Update(gameTime, !Bounds.Intersects(playerBounds));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (stateMachine.Phase == CrumblingPhase.Missing) return;
            ISprite sprite = SelectSprite();
            for (int i = 0; i < tileCount; i++)
            {
                sprite.Draw(spriteBatch, Position + new Vector2(i * BlockSpriteFactory.BlockSize, 0));
            }
        }

        private ISprite SelectSprite()
        {
            if (stateMachine.Phase == CrumblingPhase.Reforming) return reformSprite;
            if (stateMachine.Phase == CrumblingPhase.Cracking && (int)(time / BlinkSeconds) % 2 == 0)
            {
                return warningSprite;
            }
            return readySprite;
        }
    }
}
