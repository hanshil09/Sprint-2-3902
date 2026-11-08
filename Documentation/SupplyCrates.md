# Supply crates

Crate Workshop includes two Health Cell crates and two Energon crates. The room
grid controls contents, following the existing external level-data approach:

| Symbol | Appearance | Contents |
|---|---|---|
| C | Orange crate | Empty |
| H | Green crate | One Health Cell (+10 HP up to maximum health) |
| E | Gold crate | One Energon pickup |
| Q | Orange-red crate | One Bomb Pack; only a bomb dropped in Morph Ball form can break it |

Shoot or bomb a regular supply crate, then touch its released item to collect it.
Q crates need a bomb dropped while the player is in Morph Ball form. Shooting
or touching a Q crate does not destroy it. Health Cells heal immediately when
health is missing and stay available when health is already full.

On room revisits, an opened crate remains open. If its released item was not
collected before leaving, that one item remains available. Collected crate
items and placed pickups do not return during the current game. Press R to
start a new game and reset this progress.

## Implementation

- LevelParser records H/E/Q as crate blocks and stores their contents separately
  in LevelData.CrateDrops. Contents do not count as loose room pickups.
- SupplyCrate inherits existing crate destruction from Block and implements
  IItemDropBlock. Its release flag permits one successful TryReleaseItem call
  after destruction, even when Destroy is called repeatedly.
- Q uses the existing bomb and collision systems but is only destroyed by a
  bomb dropped while Player is in Morph Ball form. Its orange-red tint uses the
  existing crate art.
- Existing projectile/bomb collision logic works with the Block base class.
  No new object-creation responsibility is added to collision detection.
- Level checks for pending drops during Update and creates the pickup through
  EntityFactory. Spawning occurs on the next level update after impact, outside
  collision-list iteration. Normal item updating, drawing and collection apply.
- RoomProgress stores collected pickup IDs, opened crate IDs, collected crate
  drop IDs, and boss defeat for each room until a new game starts.
- BlockSpriteFactory selects the tinted crate sprites. Gameplay behavior remains
  separate from drawing, following the course's factory/interface examples.

Press F1 for the crate legend. Regression command from the repository root:

```text
dotnet run --project RegressionChecks -c Strict --no-restore -- --crate-drops
```

Checks cover parser contents, normal shots, bomb explosions, repeated hits,
Morph Ball-only crate access, collection, unchanged empty crates, and
indestructible ground. Rendering and a full interactive playthrough are not
validated by these headless checks.
