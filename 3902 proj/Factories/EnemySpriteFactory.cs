using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;

namespace TransformersGame.Factories
{
    public class EnemySpriteFactory
    {
        private const int EnemyWidth = 68;
        private const int EnemyHeight = 56;
        private const double WalkFrameTime = 250;

        private static readonly Rectangle[] RightWalkFrames = new Rectangle[]
        {
            new Rectangle(0, 768, 170, 140),
            new Rectangle(1190, 768, 170, 140)
        };
        private static readonly Rectangle[] LeftWalkFrames = new Rectangle[]
        {
            new Rectangle(680, 768, 170, 140),
            new Rectangle(850, 768, 170, 140)
        };
        private static readonly Rectangle WreckFrame = new Rectangle(840, 952, 170, 154);

        private static EnemySpriteFactory instance = new EnemySpriteFactory();

        private Texture2D enemySpriteSheet;

        private EnemySpriteFactory()
        {
        }

        public static EnemySpriteFactory Instance
        {
            get
            {
                return instance;
            }
        }

        public void LoadAllTextures(ContentManager content)
        {
            enemySpriteSheet = content.Load<Texture2D>("Sprites/transformer-directional-combat-spritesheet");
        }

        public ISprite CreateWalkingEnemySprite(bool facingLeft, Color tint)
        {
            Rectangle[] frames = facingLeft ? LeftWalkFrames : RightWalkFrames;
            return new AnimatedSprite(enemySpriteSheet, frames, EnemyWidth, EnemyHeight, tint, WalkFrameTime);
        }

        public ISprite CreateDestroyedEnemySprite(Color tint)
        {
            return new TextureRegionSprite(enemySpriteSheet, WreckFrame, EnemyWidth, EnemyHeight, tint);
        }
    }
}
