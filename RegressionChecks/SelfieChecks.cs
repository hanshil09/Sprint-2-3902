using Microsoft.Xna.Framework;
using TransformersGame.Collisions;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;
using TransformersGame.States;

internal static class SelfieChecks
{
    public static void Run()
    {
        CheckSequence();
        CheckFooting();
        CheckRoomLayouts();
    }

    private static void CheckSequence()
    {
        var sequence = new SelfieSequence();
        sequence.Start(224);
        sequence.Update(0.5);
        Check(sequence.Phase == SelfiePhase.Entering && sequence.RunProgress is > 0.3f and < 0.4f,
            "Click runs in before the handoff");
        sequence.Update(1);
        Check(sequence.Phase == SelfiePhase.Handoff && !sequence.IsPosing, "handoff comes before poses");
        sequence.Update(0.5);
        Check(sequence.Phase == SelfiePhase.Handoff, "Click holds the handoff for half a second");
        sequence.Update(0.06);
        Check(sequence.Phase == SelfiePhase.Posing && sequence.IsPosing, "poses are visible before the shutter");
        sequence.Update(0.6);
        Check(sequence.Phase == SelfiePhase.Posing && sequence.IsPosing, "live poses hold long enough to see");
        sequence.Update(0.06);
        Check(sequence.Phase == SelfiePhase.Capture && sequence.IsPosing, "the shutter captures the same poses");
        sequence.Update(10);
        Check(sequence.Phase == SelfiePhase.Capture && sequence.IsPosing,
            "capture waits for Draw even when frames are skipped");
        sequence.PhotoTaken();
        Check(sequence.Phase == SelfiePhase.Leaving && sequence.FlashOpacity > 0, "the exit starts with the flash");
        sequence.Update(0.13);
        float progress = sequence.RunProgress;
        sequence.PhotoTaken();
        Check(sequence.Phase == SelfiePhase.Leaving && sequence.RunProgress == progress,
            "a second draw cannot restart the flash or exit");
        Check(sequence.FlashOpacity == 0 && progress > 0, "the preview does not delay the exit");
        sequence.Update(1.1);
        Check(sequence.Phase == SelfiePhase.Finished, "the exit ends the scene without a pause");
        sequence.Start(56);
        Check(sequence.Phase == SelfiePhase.Entering && sequence.Elapsed == 0, "next selfie starts fresh");
        Console.WriteLine("12 selfie sequence checks passed.");
    }

    private static void CheckFooting()
    {
        var view = new Rectangle(0, 0, 480, 240);
        var player = new Rectangle(-45, -90, 90, 90);
        var click = new Rectangle(-48, -52, 96, 52);
        var stage = new SelfieStage(
            new IBlock[] { Tile(0, 208, 240), Tile(240, 208, 240), Tile(160, 130, 48) }, view);
        Check(stage.TryFind(new Point(180, 125), player, click, out SelfieMarks marks)
            && marks.PlayerFeet.Y == 208 && marks.ClickFeet.Y == 208 && marks.ClickStart.Y == 208,
            "airborne player and Click use the same floor instead of a narrow ledge");

        stage = new SelfieStage(new IBlock[] { Tile(0, 208, 96), Tile(120, 208, 96) }, view);
        Check(!stage.TryFind(new Point(45, 208), player, click, out _),
            "Click cannot walk across a gap or unsupported edge");

        stage = new SelfieStage(new IBlock[] { Tile(0, 208, 480, false) }, view);
        Check(!stage.TryFind(new Point(180, 208), player, click, out _),
            "non-solid platforms cannot support the photo cast");

        stage = new SelfieStage(new IBlock[] { Tile(0, 208, 640), Tile(96, 150, 200) },
            new Rectangle(0, 0, 640, 240));
        Check(stage.TryFind(new Point(180, 208), player, click, out marks)
            && !(marks.PlayerFeet.Y == 208 && marks.PlayerFeet.X > 51 && marks.PlayerFeet.X < 341),
            "staging avoids headroom blocked by an overhead platform");

        stage = new SelfieStage(new IBlock[] { Tile(800, 408, 480) }, new Rectangle(800, 200, 480, 240));
        Check(stage.TryFind(new Point(870, 408), player, click, out marks)
            && marks.PlayerFeet == new Point(870, 408)
            && Math.Abs(marks.ClickStart.X - marks.ClickFeet.X) == 224,
            "Click uses the longer grounded runway in a scrolled view");
    }

    private static void CheckRoomLayouts()
    {
        var view = new Rectangle(0, 0, 960, 540);
        var player = new Rectangle(-61, -90, 122, 90);
        var click = new Rectangle(-52, -52, 104, 52);
        foreach (string path in Directory.GetFiles("3902 proj/Content/Levels", "*.room"))
        {
            Level level = new Level(LevelParser.Parse(File.ReadAllLines(path)).Data, new Random(3902));
            SelfieStage stage = new SelfieStage(level.Blocks, view);
            Check(stage.TryFind(new Point(120, 280), player, click, out SelfieMarks marks)
                && marks.PlayerFeet.Y == marks.ClickFeet.Y,
                Path.GetFileName(path) + ": clear footing from a midair start");
        }
    }

    private static IBlock Tile(int x, int y, int width, bool solid = true) =>
        new Block(new Vector2(x, y), new PlatformSprite(width), solid);

    private sealed class PlatformSprite(int width) : ISprite
    {
        public int Width => width;
        public int Height => 32;
        public void Update(GameTime gameTime) { }
        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 location) { }
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException("FAIL: " + message);
        Console.WriteLine("PASS: " + message);
    }
}
