using Microsoft.Xna.Framework;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

internal static class AudioChecks
{
    public static void Run()
    {
        int passed = 0;
        void Check(string name, bool condition)
        {
            if (!condition) throw new InvalidOperationException("FAIL: " + name);
            Console.WriteLine("PASS: " + name);
            passed++;
        }

        var projectiles = new ProjectileManager();
        var player = new Player(Vector2.Zero, Array.Empty<IBlock>(), projectiles, gravity: 0);
        var sounds = new List<GameSound>();
        player.SoundRequested += sounds.Add;
        player.Jump();
        Check("airborne jump stays silent", sounds.Count == 0);
        player.Shoot(false);
        Check("accepted shot emits one sound", sounds.SequenceEqual(new[] { GameSound.Shoot }));
        player.Shoot(false);
        Check("shot cooldown stays silent", sounds.Count == 1);
        player.Transform();
        Check("transform emits sound", sounds[^1] == GameSound.Transform && sounds.Count == 2);
        player.Shoot(false);
        Check("vehicle cannot emit shooting sound", sounds.Count == 2);
        player.DropBomb();
        Check("bomb placement does not emit explosion", sounds.Count == 2);
        var step = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1.3));
        projectiles.Update(step);
        Check("bomb fuse emits explosion once", sounds[^1] == GameSound.Explosion && sounds.Count == 3);
        projectiles.Update(step);
        Check("expired explosion does not repeat", sounds.Count == 3);
        player.UseMedkit();
        player.UseShield();
        Check("empty inventory actions stay silent", sounds.Count == 3);
        player.TryAddItem(ItemKind.ShieldBattery);
        player.UseShield();
        Check("accepted shield emits item sound", sounds[^1] == GameSound.Item && sounds.Count == 4);
        player.TryAddItem(ItemKind.ShieldBattery);
        player.UseShield();
        Check("already active shield stays silent", sounds.Count == 4);
        Console.WriteLine($"{passed} audio event checks passed.");
    }
}
