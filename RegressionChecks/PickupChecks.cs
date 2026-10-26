using Microsoft.Xna.Framework;
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
        Check("first battery activates shield", player.TryAddItem(ItemKind.ShieldBattery) && player.IsShieldActive);
        Check("second battery reaches time cap", player.TryAddItem(ItemKind.ShieldBattery));
        Check("battery stays at shield time cap", level.CollectItems(player).Count == 0 && level.Items.Count == 1);
        player.Update(Step(1));
        Check("battery collects after shield time elapses", level.CollectItems(player).Count == 1 && level.Items.Count == 0);
        player.Update(Step(9.9));
        Check("battery refills remaining time", player.IsShieldActive);
        player.Update(Step(0.1));
        Check("battery does not exceed ten second cap", !player.IsShieldActive);
    }

    private static void CheckBombPacks()
    {
        Level level = CreateLevel("%");
        Player player = CreatePlayer(level);
        for (int upgrade = 0; upgrade < PlayerInventory.MaximumBombLevel - 1; upgrade++) player.TryAddItem(ItemKind.BombPack);
        Check("Bomb Pack collects below cap", level.CollectItems(player).Count == 1 && player.BombLevel == PlayerInventory.MaximumBombLevel);
        level = CreateLevel("%");
        Check("Bomb Pack remains at maximum level", level.CollectItems(player).Count == 0 && level.Items.Count == 1);
        Check("direct bomb upgrade also rejects at cap", !player.TryAddItem(ItemKind.BombPack) && player.BombLevel == PlayerInventory.MaximumBombLevel);
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
        Check("stored items increase correct inventory counts", player.MedkitCount == 1 && player.ShieldCount == 1 && player.EnergonCount == 1);
        Check("full health still prevents wasting stored Medkit", !player.UseMedkit() && player.MedkitCount == 1);
        player.TakeDamage();
        Check("stored Medkit can heal later", player.UseMedkit() && player.Health == player.MaxHealth && player.MedkitCount == 0);
        Check("stored Shield can activate later", player.UseShield() && player.IsShieldActive && player.ShieldCount == 0);
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
