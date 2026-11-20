using Microsoft.Xna.Framework;
using ArmadilloGame.Core;
using ArmadilloGame.Entities;

internal static class BossChecks
{
    public static void Run()
    {
        var defaults = BossConfiguration.Warlord;
        Check(defaults.MaximumHealth == 30 && defaults.Size == 96
            && defaults.HoverY == 96f && defaults.FireballSpeed == 260f,
            "Warlord defaults preserve its current health, size, movement, and projectile speed");

        int poseRequests = 0;
        int projectileRequests = 0;
        var configuration = new BossConfiguration
        {
            MaximumHealth = 60,
            Size = 120,
            HitboxInset = 10,
            CreateSprite = (_, _, size) =>
            {
                poseRequests++;
                return new BossTestSprite(size);
            },
            CreateProjectileSprite = () =>
            {
                projectileRequests++;
                return new BossTestSprite(8);
            }
        };
        var projectiles = new ProjectileManager();
        var boss = new Boss(new Vector2(10, 20), new StubPlayer(), projectiles, configuration);

        Check(poseRequests > 0 && boss.Health == 60 && boss.Kind == EnemyKind.Warlord,
            "boss uses its supplied configuration and sprite factory");
        Check(boss.Bounds == new Rectangle(20, 30, 100, 110) && boss.Center == new Vector2(70, 80),
            "boss hitbox and center use the configured size and inset");

        boss.TakeDamage(20);
        Check(boss.Health == 40 && boss.Phase == 2, "configured maximum health determines boss phases");
        boss.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.09)));
        boss.TakeDamage(21);
        Check(boss.Health == 19 && boss.Phase == 3, "boss advances to its enraged phase after damage");

        boss.FireAtTarget(0f);
        Check(projectileRequests == 1 && projectiles.Projectiles.Count == 1,
            "boss attacks use the configured projectile sprite provider");
        Console.WriteLine("5 boss configuration checks passed.");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException("FAIL: " + message);
        Console.WriteLine("PASS: " + message);
    }
}

internal sealed class BossTestSprite(int size) : ArmadilloGame.Interfaces.ISprite
{
    public int Width => size;
    public int Height => size;
    public void Update(GameTime gameTime) { }
    public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Vector2 position) { }
}
