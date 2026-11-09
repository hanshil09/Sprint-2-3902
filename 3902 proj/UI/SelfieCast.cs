using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Collisions;
using TransformersGame.Core;
using TransformersGame.Factories;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;
using TransformersGame.States;

namespace TransformersGame.UI
{
    internal sealed class SelfieCast
    {
        private const double RunFrameSeconds = 0.1;
        private const float EdgeFadeFraction = 0.18f;
        private readonly SelfieSpriteFactory sprites;
        private readonly List<(SelfiePose Pose, Point Feet)> enemyPoses = new();
        private readonly List<IEnemy> unposedEnemies = new();
        private readonly SelfiePose[] clickPoses = new SelfiePose[4];
        private readonly SelfiePose[] exitPoses = new SelfiePose[2];
        private SelfiePose playerStand;
        private SelfiePose playerPhoto;
        private SelfieMarks marks;

        public SelfieCast(SelfieSpriteFactory sprites)
        {
            this.sprites = sprites;
        }

        public int RunDistance => Math.Abs(marks.ClickStart.X - marks.ClickFeet.X);

        public bool Prepare(Room room, IPlayer player, Rectangle view)
        {
            Point origin = new Point((int)(player.Position.X + player.Width / 2f),
                (int)MathF.Round(player.Position.Y + player.Height));
            playerStand = Centered(player.IsInBallForm
                ? PlayerSpriteFactory.Instance.CreateBallSprite(player.Facing == Direction.Left)
                : PlayerSpriteFactory.Instance.CreateStandingRobotSprite(player.Facing));
            Rectangle playerSpace = Rectangle.Union(playerStand.BoundsAt(Point.Zero),
                BothDirections(sprites.CreatePlayer(player.IsInBallForm, false)));
            SelfieStage stage = new SelfieStage(room.Level.Blocks, view);
            if (!stage.TryFind(origin, playerSpace, ClickSpace(), out marks)) return false;

            bool facesLeft = marks.ClickFeet.X > marks.PlayerFeet.X;
            playerPhoto = sprites.CreatePlayer(player.IsInBallForm, facesLeft);
            foreach (ClickPose pose in Enum.GetValues<ClickPose>())
            {
                clickPoses[(int)pose] = sprites.CreateClick(pose, facesLeft);
            }
            exitPoses[0] = sprites.CreateClick(ClickPose.WalkFirst, !facesLeft);
            exitPoses[1] = sprites.CreateClick(ClickPose.WalkSecond, !facesLeft);
            PrepareEnemyPoses(room);
            return true;
        }

        public void DrawPlayer(SpriteBatch batch, bool posing) =>
            (posing ? playerPhoto : playerStand).Draw(batch, marks.PlayerFeet);

        public void DrawEnemies(SpriteBatch batch)
        {
            foreach ((SelfiePose pose, Point feet) in enemyPoses) pose.Draw(batch, feet);
            foreach (IEnemy enemy in unposedEnemies) enemy.Draw(batch);
        }

        public void DrawClick(SpriteBatch batch, SelfieSequence sequence)
        {
            int runFrame = (int)(sequence.Elapsed / RunFrameSeconds) % 2;
            SelfiePose pose = sequence.Phase switch
            {
                SelfiePhase.Entering => clickPoses[runFrame],
                SelfiePhase.Leaving => exitPoses[runFrame],
                SelfiePhase.Handoff => clickPoses[(int)ClickPose.Handoff],
                _ => clickPoses[(int)ClickPose.Shutter]
            };
            int x = sequence.Phase switch
            {
                SelfiePhase.Entering => RunX(marks.ClickStart.X, marks.ClickFeet.X, sequence.RunProgress),
                SelfiePhase.Leaving => RunX(marks.ClickFeet.X, marks.ClickStart.X, sequence.RunProgress),
                _ => marks.ClickFeet.X
            };
            float opacity = sequence.Phase switch
            {
                SelfiePhase.Entering => Math.Min(1f, sequence.RunProgress / EdgeFadeFraction),
                SelfiePhase.Leaving => Math.Min(1f, (1f - sequence.RunProgress) / EdgeFadeFraction),
                _ => 1f
            };
            pose.Draw(batch, new Point(x, marks.ClickFeet.Y), opacity);
        }

        private Rectangle ClickSpace()
        {
            Rectangle space = BothDirections(sprites.CreateClick(ClickPose.WalkFirst, false));
            foreach (ClickPose pose in Enum.GetValues<ClickPose>())
            {
                space = Rectangle.Union(space, BothDirections(sprites.CreateClick(pose, false)));
            }
            return space;
        }

        private void PrepareEnemyPoses(Room room)
        {
            enemyPoses.Clear();
            unposedEnemies.Clear();
            foreach (IGameObject obj in room.Enemies.Objects)
            {
                if (obj is not IEnemy enemy || enemy.IsDestroyed) continue;
                if (enemy.Bounds.IsEmpty)
                {
                    unposedEnemies.Add(enemy);
                    continue;
                }
                Rectangle bounds = EnemyDrawBounds(enemy);
                SelfiePose pose = Centered(sprites.CreateEnemy(enemy.Kind, bounds.Size));
                enemyPoses.Add((pose, new Point(bounds.Center.X, bounds.Bottom)));
            }
        }

        // The boss collision box is inset from the rendered sprite.
        private static Rectangle EnemyDrawBounds(IEnemy enemy) => enemy.Kind == EnemyKind.Warlord
            ? new Rectangle((int)enemy.Position.X, (int)enemy.Position.Y,
                EnemySpriteFactory.BossSize, EnemySpriteFactory.BossSize)
            : enemy.Bounds;

        private static SelfiePose Centered(ISprite sprite) =>
            new SelfiePose(sprite, new Point(sprite.Width / 2, sprite.Height));

        private static Rectangle BothDirections(SelfiePose pose)
        {
            Rectangle bounds = pose.BoundsAt(Point.Zero);
            return Rectangle.Union(bounds, new Rectangle(-bounds.Right, bounds.Y, bounds.Width, bounds.Height));
        }

        private static int RunX(int start, int end, float progress) =>
            (int)MathF.Round(MathHelper.Lerp(start, end, progress));
    }
}
