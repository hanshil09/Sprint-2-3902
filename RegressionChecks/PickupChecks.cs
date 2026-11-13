using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Interfaces;

internal static class PickupChecks
{
    private static int passed;
    private static GameTime Step(double seconds) => new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(seconds));

    public static void Run()
    {
        CheckHealthCells();
        CheckShieldBatteries();
        CheckBombPacks();
        CheckMaxHealthUpgrades();
        CheckStoredItems();
        CheckBazookaCallStorage();
        CheckBazookaShot();
        CheckBazookaAim();
        CheckDecoratedPlayer();
        CheckPowerUps();
        Console.WriteLine($"{passed} pickup checks passed.");
    }

    private static void Check(string name, bool condition)
    {
        if (!condition) throw new InvalidOperationException("FAIL: " + name);
        passed++;
        Console.WriteLine("PASS: " + name);
    }

    private static void CheckBazookaCallStorage()
    {
        var inventory = new PlayerInventory();
        Check("bazooka call is stored until used", inventory.TryCollect(ItemKind.BazookaCall) && inventory.HasBazookaCall);
        Check("a second call is not collected while one is stored", !inventory.TryCollect(ItemKind.BazookaCall));
        Check("using the call loads three shots", inventory.TryUseBazookaCall() && !inventory.HasBazookaCall && inventory.BazookaAmmo == 3);
        Check("an empty call cannot be used again", !inventory.TryUseBazookaCall());
        Check("a loaded shot prevents another pickup", !inventory.TryCollect(ItemKind.BazookaCall));
        Check("three shots can be consumed, but not a fourth", inventory.TryFireBazooka() && inventory.TryFireBazooka()
            && inventory.TryFireBazooka() && !inventory.TryFireBazooka());
        Check("another call can be collected after firing", inventory.TryCollect(ItemKind.BazookaCall));
    }

    private static void CheckBazookaShot()
    {
        var projectiles = new ProjectileManager();
        var player = new Player(Vector2.Zero, Array.Empty<IBlock>(), projectiles, gravity: 0);
        Check("bazooka cannot fire before collection", !player.FireBazooka());
        Check("collected call equips three shots", player.TryAddItem(ItemKind.BazookaCall)
            && player.UseBazookaCall() && player.Inventory.BazookaAmmo == 3);
        player.EnterRoom(Array.Empty<IBlock>(), projectiles, Vector2.Zero, gravity: 0);
        Check("the loaded shot stays available after a room change", player.Inventory.BazookaLoaded);
        player.Transform();
        Check("ball form cannot waste the shot", !player.FireBazooka() && player.Inventory.BazookaLoaded);
        player.Transform();
        Check("robot fires one bazooka projectile", player.FireBazooka()
            && player.Inventory.BazookaAmmo == 2 && projectiles.Projectiles.Count == 1);
        var enemy = new StubEnemy();
        ((IPlayerWeapon)projectiles.Projectiles[0]).HitEnemy(enemy);
        Check("bazooka deals five damage and deactivates on impact",
            enemy.Hits == 5 && !projectiles.Projectiles[0].IsActive);
        Check("robot fires a vertical shot", player.FireBazooka(true) && player.Inventory.BazookaAmmo == 1);
        IProjectile upward = projectiles.Projectiles[1];
        Vector2 before = upward.Position;
        upward.Update(Step(0.1));
        Check("upward shot travels straight up", upward.Position.Y < before.Y && upward.Position.X == before.X);
        Check("last shot fires and a fourth is rejected", player.FireBazooka()
            && player.Inventory.BazookaAmmo == 0 && !player.FireBazooka());
        var freshPlayer = new Player(Vector2.Zero, Array.Empty<IBlock>(), new ProjectileManager(), gravity: 0);
        Check("a fresh player starts without a call or loaded shot",
            !freshPlayer.Inventory.HasBazookaCall && !freshPlayer.Inventory.BazookaLoaded);
    }

    private static void CheckBazookaAim()
    {
        var projectiles = new ProjectileManager();
        var player = new Player(Vector2.Zero, Array.Empty<IBlock>(), projectiles, gravity: 0);
        var bazooka = new BazookaAbility(null, null);
        player.TryAddItem(ItemKind.BazookaCall);
        player.UseBazookaCall();
        bazooka.SetAim(new KeyboardState(Keys.LeftShift));
        bazooka.Fire(player);
        IProjectile shot = projectiles.Projectiles[0];
        Vector2 before = shot.Position;
        shot.Update(Step(0.1));
        Check("bazooka uses sampled upward aim when firing", shot.Position.Y < before.Y && shot.Position.X == before.X);
        bazooka.SetAim(new KeyboardState());
        bazooka.Fire(player);
        shot = projectiles.Projectiles[1];
        before = shot.Position;
        shot.Update(Step(0.1));
        Check("releasing aim restores horizontal fire", shot.Position.X > before.X && shot.Position.Y == before.Y);
    }

    private static Level CreateLevel(string symbols) => new Level(LevelParser.Parse(new[] { "[tiles]", "P..." + symbols }).Data);

    private static Player CreatePlayer(Level level) => new Player(level.Items[0].Position, level.Blocks, new ProjectileManager(), gravity: 0);

    private static void CheckHealthCells()
    {
        Level level = CreateLevel("!");
        Player player = CreatePlayer(level);
        IItem pickup = level.Items[0];
        Check("full health leaves Health Cell in room", level.CollectItems(player).Count == 0 && level.Items.Count == 1);
        Check("repeated overlap still leaves Health Cell", level.CollectItems(player).Count == 0 && level.Items[0] == pickup);
        player.Position += new Vector2(100, 0);
        player.TakeDamage();
        Check("damaged player must still touch pickup", level.CollectItems(player).Count == 0 && level.Items.Count == 1);
        player.Position = pickup.Position;
        Check("Health Cell collects after damage", level.CollectItems(player).Count == 1 && level.Items.Count == 0);
        Check("Health Cell clamps healing to maximum", player.Health == player.MaxHealth);
        player.TakeDamage();
        Check("collected Health Cell cannot apply twice", !pickup.TryCollect(player) && player.Health == player.MaxHealth - 3);
        for (int hit = 0; hit < 4; hit++) player.TakeDamage();
        int before = player.Health;
        Check("Health Cell heals ten when enough health is missing", player.TryAddItem(ItemKind.HealthCell) && player.Health == before + 10);
    }

    private static void CheckShieldBatteries()
    {
        Level level = CreateLevel("$");
        Player player = CreatePlayer(level);
        IItem pickup = level.Items[0];
        Check("battery collects into inventory", level.CollectItems(player).Count == 1 && player.Inventory.ShieldCount == 1);
        Check("battery does not activate automatically", !player.IsShieldActive);
        Check("collected battery cannot be counted twice", !pickup.TryCollect(player) && player.Inventory.ShieldCount == 1);
        Check("battery activates on manual use", player.UseShield() && player.IsShieldActive && player.Inventory.ShieldCount == 0);
        player.Update(Step(4));
        Check("battery can be stored while shield is active", player.TryAddItem(ItemKind.ShieldBattery) && player.Inventory.ShieldCount == 1);
        Check("active shield cannot consume another charge", !player.UseShield() && player.Inventory.ShieldCount == 1);
        player.Update(Step(1.1));
        Check("collecting battery does not extend active shield", !player.IsShieldActive);
        Check("stored battery can activate after expiry", player.UseShield() && player.Inventory.ShieldCount == 0 && player.IsShieldActive);
        player.Update(Step(5.1));
        Check("manual shield expires after five seconds", !player.IsShieldActive);
        Check("empty inventory cannot activate shield", !player.UseShield());
    }

    private static void CheckBombPacks()
    {
        Level level = CreateLevel("%");
        Player player = CreatePlayer(level);
        for (int upgrade = 0; upgrade < PlayerInventory.MaximumBombLevel - 1; upgrade++) player.TryAddItem(ItemKind.BombPack);
        Check("Bomb Pack collects below cap", level.CollectItems(player).Count == 1 && player.Inventory.BombLevel == PlayerInventory.MaximumBombLevel);
        level = CreateLevel("%");
        Check("Bomb Pack remains at maximum level", level.CollectItems(player).Count == 0 && level.Items.Count == 1);
        Check("direct bomb upgrade also rejects at cap", !player.TryAddItem(ItemKind.BombPack) && player.Inventory.BombLevel == PlayerInventory.MaximumBombLevel);
        var inventory = new PlayerInventory();
        for (int upgrade = 0; upgrade < PlayerInventory.MaximumBombLevel; upgrade++) inventory.TryCollect(ItemKind.BombPack);
        Check("inventory rejects capped Bomb Pack", !inventory.TryCollect(ItemKind.BombPack));
    }

    private static void CheckMaxHealthUpgrades()
    {
        Level level = CreateLevel("?");
        Player player = CreatePlayer(level);
        Check("health upgrade collects at full starting health", level.CollectItems(player).Count == 1 && player.MaxHealth == 120 && player.Health == 120);
        for (int upgrade = 0; upgrade < 4; upgrade++) player.TryAddItem(ItemKind.MaxHealthUp);
        level = CreateLevel("?");
        Check("health upgrade stays when both health and capacity are full", level.CollectItems(player).Count == 0 && level.Items.Count == 1 && player.MaxHealth == 200);
        player.TakeDamage();
        Check("health upgrade still heals at capacity cap", level.CollectItems(player).Count == 1 && level.Items.Count == 0 && player.Health == 200 && player.MaxHealth == 200);
        Check("direct health upgrade rejects without benefit", !player.TryAddItem(ItemKind.MaxHealthUp));
    }

    private static void CheckStoredItems()
    {
        Level level = CreateLevel("+*@");
        Player player = CreatePlayer(level);
        Check("stored items and Energon collect at full health", level.CollectItems(player).Count == 3 && level.Items.Count == 0);
        Check("stored items increase correct inventory counts", player.Inventory.MedkitCount == 1 && player.Inventory.ShieldCount == 1 && player.Inventory.EnergonCount == 1);
        Check("full health still prevents wasting stored Medkit", !player.UseMedkit() && player.Inventory.MedkitCount == 1);
        player.TakeDamage();
        Check("stored Medkit can heal later", player.UseMedkit() && player.Health == player.MaxHealth && player.Inventory.MedkitCount == 0);
        Check("stored Shield can activate later", player.UseShield() && player.IsShieldActive && player.Inventory.ShieldCount == 0);
    }

    private static void CheckDecoratedPlayer()
    {
        Level level = CreateLevel("!");
        Player player = CreatePlayer(level);
        IPlayer decorated = new InvinciblePlayer(player, new PickupHost { Player = player });
        Check("decorator forwards pickup rejection", level.CollectItems(decorated).Count == 0 && level.Items.Count == 1);
        player.TakeDamage();
        Check("decorator forwards successful collection", level.CollectItems(decorated).Count == 1 && player.Health == player.MaxHealth);
    }

    private static void CheckPowerUps()
    {
        foreach (string symbol in new[] { "&", "~" })
        {
            Level level = CreateLevel(symbol);
            Player player = CreatePlayer(level);
            var host = new PickupHost { Player = player };
            IReadOnlyList<IItem> items = level.CollectItems(player);
            Check(symbol + " power-up remains collectible", items.Count == 1 && level.Items.Count == 0);
            host.Player = ((IPowerUp)items[0]).Apply(player, host);
            host.Player.Update(Step(1));
            level = CreateLevel(symbol);
            items = level.CollectItems(host.Player);
            IPlayer refreshed = ((IPowerUp)items[0]).Apply(host.Player, host);
            Check(symbol + " repeat pickup refreshes existing decorator", ReferenceEquals(refreshed, host.Player)
                && refreshed is PlayerDecorator decorator
                && decorator.RemainingMilliseconds == (symbol == "&" ? InvinciblePlayer.Duration : TurboPlayer.Duration));
        }
    }
}

internal sealed class PickupHost : IPlayerHost
{
    public IPlayer Player { get; set; }
    public void RemovePlayerDecorator(PlayerDecorator decorator) => Player = PlayerDecorator.RemoveFromChain(Player, decorator);
}
