using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;

namespace TransformersGame.Factories
{
    internal enum ClickPose { WalkFirst, WalkSecond, Handoff, Shutter }

    internal sealed class SelfieSpriteFactory
    {
        // Source rectangles crop the generated sheets to each pose with a small margin.
        private static readonly Rectangle Standing = new Rectangle(37, 49, 318, 297);
        private static readonly Rectangle Ball = new Rectangle(415, 107, 278, 233);
        private const int StandingFootX = 119;
        private const int BallFootX = 85;
        private const int StandingHeight = 90;
        private const int BallHeight = 54;
        private const int ClickHeight = 52;
        private static readonly Dictionary<EnemyKind, Rectangle> EnemyFrames = new()
        {
            [EnemyKind.Flyer] = new Rectangle(1124, 88, 285, 205),
            [EnemyKind.Crawler] = new Rectangle(44, 506, 245, 150),
            [EnemyKind.Hopper] = new Rectangle(435, 488, 198, 169),
            [EnemyKind.Beetle] = new Rectangle(769, 461, 265, 201),
            [EnemyKind.Waver] = new Rectangle(1175, 401, 226, 257),
            [EnemyKind.Sentry] = new Rectangle(45, 881, 245, 149),
            [EnemyKind.Sentinel] = new Rectangle(421, 785, 215, 245),
            [EnemyKind.Warlord] = new Rectangle(756, 726, 303, 337)
        };
        private static readonly Rectangle[] ClickFrames =
        {
            new Rectangle(67, 61, 428, 465),
            new Rectangle(738, 64, 426, 460),
            new Rectangle(68, 671, 609, 473),
            new Rectangle(759, 686, 439, 458)
        };
        private static readonly int[] ClickFootX = { 242, 243, 132, 210 };

        private readonly Texture2D poses;
        private readonly Texture2D click;

        public SelfieSpriteFactory(ContentManager content)
        {
            poses = content.Load<Texture2D>("Sprites/Selfie/selfie-pose-sheet");
            click = content.Load<Texture2D>("Sprites/Selfie/photographer-bot");
        }

        public SelfiePose CreatePlayer(bool isBall, bool facesLeft) =>
            CreatePose(poses, isBall ? Ball : Standing, isBall ? BallHeight : StandingHeight,
                isBall ? BallFootX : StandingFootX, facesLeft);

        public ISprite CreateEnemy(EnemyKind kind, Point size)
        {
            Rectangle frame = EnemyFrames[kind];
            float scale = Math.Min(size.X / (float)frame.Width, size.Y / (float)frame.Height);
            return new TextureRegionSprite(poses, frame, (int)MathF.Round(frame.Width * scale),
                (int)MathF.Round(frame.Height * scale), Color.White);
        }

        public SelfiePose CreateClick(ClickPose pose, bool facesLeft) =>
            CreatePose(click, ClickFrames[(int)pose], ClickHeight, ClickFootX[(int)pose], facesLeft);

        private static SelfiePose CreatePose(Texture2D texture, Rectangle frame, int height,
            int footX, bool facesLeft)
        {
            ISprite sprite = Create(texture, frame, height, facesLeft);
            int offset = (int)MathF.Round(footX * sprite.Width / (float)frame.Width);
            return new SelfiePose(sprite, new Point(facesLeft ? sprite.Width - offset : offset, height));
        }

        private static ISprite Create(Texture2D texture, Rectangle frame, int height, bool facesLeft)
        {
            int width = (int)MathF.Round(height * frame.Width / (float)frame.Height);
            return new TextureRegionSprite(texture, frame, width, height, Color.White,
                facesLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
        }
    }
}
