using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;

namespace TransformersGame.Factories
{
    public class EnemySpriteFactory
    {
        private const int EnemyWidth = 3 * 16;
        private const int EnemyHeight = 3 * 16;
        private const double IdleFrameTime = 250;

        private static readonly Rectangle[] Enemy1Frames =
        {
            new Rectangle(0, 0, 16, 16),
            new Rectangle(16, 0, 16, 16)
        };

        private static readonly Rectangle[] Enemy2Frames =
        {
            new Rectangle(32, 0, 16, 16),
            new Rectangle(48, 0, 16, 16)
        };

        private static readonly Rectangle[] Enemy4Frames =
        {
            new Rectangle(0, 48, 16, 16),
            new Rectangle(16, 48, 16, 16),
            new Rectangle(32, 48, 16, 16)
        };

        private static readonly Rectangle[] Enemy5Frames =
        {
            new Rectangle(0, 64, 16, 16),
            new Rectangle(16, 64, 16, 16)
        };

        private static readonly Rectangle[] Enemy6Frames =
        {
            new Rectangle(32, 64, 16, 16),
            new Rectangle(48, 64, 16, 16)
        };

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
            enemySpriteSheet = content.Load<Texture2D>("Sprites/metroidEnemyDemo");
        }

        public ISprite CreateEnemySprite(EnemyKind kind, Color tint)
        {
            Rectangle[] frames = kind switch
            {
                EnemyKind.Flyer => Enemy1Frames,
                EnemyKind.Crawler => Enemy2Frames,
                EnemyKind.Hopper => Enemy4Frames,
                EnemyKind.Beetle => Enemy5Frames,
                EnemyKind.Waver => Enemy6Frames,
                _ => Enemy1Frames
            };

            return new AnimatedSprite(
                enemySpriteSheet, frames,
                EnemyWidth, EnemyHeight, tint, IdleFrameTime);
        }
    }
}
