# Supply crates

Crate Workshop includes two Health Cell crates and two Energon crates. The room
grid controls contents, following the existing external level-data approach:

| Symbol | Appearance | Contents |
|---|---|---|
| C | Orange crate | Empty |
| H | Green crate | One Health Cell (+10 HP up to maximum health) |
| E | Gold crate | One Energon pickup |

Shoot or bomb a supply crate, then touch its released item to collect it. Health
Cells use the existing immediate-health behavior, including being consumed when
health is already full. Supply crates reset when a room is recreated, just like
existing crates and placed pickups; this feature does not change room persistence.

## Implementation

- LevelParser records H/E as crate blocks and stores their contents separately
  in LevelData.CrateDrops. Contents do not count as loose room pickups.
- SupplyCrate inherits existing crate destruction from Block and implements
  IItemDropBlock. Its release flag permits one successful TryReleaseItem call
  after destruction, even when Destroy is called repeatedly.
- Existing projectile/bomb collision logic works with the Block base class.
  No new object-creation responsibility is added to collision detection.
- Level checks for pending drops during Update and creates the pickup through
  EntityFactory. Spawning occurs on the next level update after impact, outside
  collision-list iteration. Normal item updating, drawing and collection apply.
- BlockSpriteFactory selects the tinted crate sprites. Gameplay behavior remains
  separate from drawing, following the course's factory/interface examples.

Press F1 for the crate legend. Regression command from the repository root:

```text
dotnet run --project RegressionChecks -c Strict --no-restore -- --crate-drops
```

Checks cover parser contents, normal shots, bomb explosions, repeated hits,
collection, unchanged empty crates, and indestructible ground. Rendering and a
full interactive playthrough are not validated by these headless checks.
