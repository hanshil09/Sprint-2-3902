# Room Respawn Decision

## Decision

When the player returns to a room during the same game:

1. Regular enemies respawn.
2. A placed collectible that was successfully collected stays gone.
3. Each collected permanent upgrade stays collected. In this game, the
   permanent upgrade pickups are Bomb Pack and Max Health Up.
4. A defeated boss stays defeated.
5. Player health, inventory, and upgrades continue to carry over between rooms.
6. A supply crate cannot repeatedly award the same item. An opened crate stays
   open; if its released item was not collected before leaving, that one item
   can still be collected on return.

   For Bomb Packs and Max Health Ups, "once" means once per placed pickup. If a
   room has more than one of that upgrade, each placement can still be collected
   once; revisiting a room cannot collect the same placement again.

Starting a new game with **R** clears this saved room progress. Every room,
enemy, item, boss, and crate returns to its starting state, and the player
starts with their usual health and inventory.

This decision is implemented for the current game run. Room state is not saved
to disk and starts fresh whenever a new game begins.

## Original behavior before room-progress tracking

Before this decision was implemented, `RoomManager.SwitchTo` called its room
factory every time the player switched to a different room. It then unloaded
the old room and called
`Player.EnterRoom` with the new room's blocks, projectiles, starting position,
and gravity. The old projectiles are cleared by `Room.Unload`.

The room factory reloaded the room data and made a new `Level` and `Room`.
`Level` built the placed items and crates from the room data again, and created
a fresh enemy list. This meant that on return:

- Regular enemies start over, including their health and positions.
- Placed items appear again, even if they were collected earlier.
- Crates are whole again and can release their items again.
- The boss returns with full health.
- Moving and crumbling platforms are recreated. The crumbling layout uses the
  same per-room seed for the current game, but its timers and platform phases
  start again.
- The existing `Player` object is not replaced, so the player's health,
  inventory counts, and upgrades carry over.

The R reset follows a different path: `Game1.InitializeGameObjects` calls
`RoomFactory.StartNewGame`, creates a new player, and creates a new room
manager. This is the natural point to start with an empty room-progress record.

Relevant current code:

- [RoomManager.cs](../3902%20proj/Core/RoomManager.cs) builds a fresh room on
  `SwitchTo` and keeps the same player.
- [RoomFactory.cs](../3902%20proj/Core/RoomFactory.cs) loads each level and
  builds a new room.
- [Room.cs](../3902%20proj/Core/Room.cs) owns the current level, enemies, and
  projectiles. Its goal unlocks after no live boss remains.
- [Level.cs](../3902%20proj/Core/Level.cs) creates placed items and crates,
  collects items, and releases crate drops.
- [PlayerInventory.cs](../3902%20proj/Core/PlayerInventory.cs) holds carried
  counts such as Medkits, Shields, Energon, and Bomb Pack level. Health and
  maximum health are held by [Player.cs](../3902%20proj/Entities/Player.cs).
- [Enemy.cs](../3902%20proj/Entities/Enemy.cs), [Boss.cs](../3902%20proj/Entities/Boss.cs),
  [ResourcePickup.cs](../3902%20proj/Entities/Items/ResourcePickup.cs),
  [PowerUpPickup.cs](../3902%20proj/Entities/Items/PowerUpPickup.cs), and
  [SupplyCrate.cs](../3902%20proj/Entities/SupplyCrate.cs) contain the current
  enemy, boss, pickup, and crate behavior.

## Comparing the choices

| Choice | Advantages | Disadvantages |
|---|---|---|
| **Everything respawns** | Keeps revisited rooms busy and lets players recover missed health or supplies. It is the simplest match for the current room-rebuild behavior. | Repeating a room can give the same health, Energon, Bomb Packs, and Max Health Ups repeatedly. A player could farm crates and upgrades. A defeated boss would also return, which conflicts with the final-room progression. |
| **Nothing respawns** | Makes cleared rooms feel complete and prevents repeated rewards. | A room may become empty and safe after one visit. It can punish players who need another attempt or leave a collectible behind. Respawning regular enemies would require more state than this option. |
| **Enemies respawn; collected items do not** | Keeps repeat visits active while making collected rewards meaningful. Missed items can still be collected, and collected upgrades cannot be farmed. Boss defeat and player progress can remain permanent for that game. | Requires the game to remember room progress and identify each item and crate drop consistently. It is more work than rebuilding everything without any saved information. |

### Why the third choice is recommended

The game is a room-based action platformer. Rebuilding regular enemies gives a
returning player something to fight, so the player is not simply walking
through an already-cleared room. At the same time, a collectible reward should
not become an unlimited source of health, Energon, or upgrades just because the
player can switch rooms.

The hybrid choice also makes the final Warlord fight feel completed once won,
while preserving the player's carried health and inventory. It is a useful
balance between keeping rooms playable and respecting the player's progress.

These are tradeoffs for this project's current rooms and rewards, not a rule
that every game must follow. The [gameplay guide](GameplayGuide.md) describes
the current carried inventory, upgrade caps, and room-revisit behavior. The
project's [supply-crate guide](SupplyCrates.md) also explains why crate drops
need to be considered separately from placed items.

## Classes and methods involved

| Class or system | Role in current behavior |
|---|---|
| `Game1.InitializeGameObjects` | Calls `RoomFactory.StartNewGame`, which makes a fresh progress record for every room and creates a new player. |
| `RoomFactory.LoadLevel` | Passes that room's `RoomProgress` into its newly created `Level`. Regular enemies are still created for every visit. |
| `Level` and `RoomProgress` | Use room-local grid positions as stable IDs. Record successful collections and opened crates, and restore an uncollected drop from an opened crate. |
| `Room.Unload` and `Level.FillEnemyCycler` | Record a boss defeat and save a destroyed crate if the room is left before its next update. A defeated boss is skipped when rebuilding the room, and the goal stays unlocked. |
| `Player` and `PlayerInventory` | Continue carrying health, inventory counts, and upgrades between rooms. |

`RoomFactory` owns one `RoomProgress` record for each room in the current game.
Because each record belongs to one room, placed items use their grid position
for an ID (`placed:column:row`), and crates use theirs (`crate:column:row`).
A crate drop adds `:drop` to the crate ID. These IDs avoid relying on object
references, because the objects are rebuilt on every visit.

## Implementation overview

`RoomProgress` keeps the collected item IDs, opened crate IDs, collected crate
drop IDs, and boss status in memory for the current run. IDs use room-local tile
coordinates, so they still match after the room's objects are rebuilt. Each
successful collection is recorded; rejected pickups remain available. On a
revisit, regular enemies are recreated, a defeated boss is skipped, collected
items are omitted, and an opened crate is restored without a duplicate drop.
When leaving a room, `Room.Unload` records any crate destroyed just before the
exit, so its uncollected drop can be restored on return. `RoomFactory.StartNewGame`
clears these records.

This decision is only for progress during the current run. Saving this
information to disk is a separate feature and is not included.

## Test scenarios

1. **Regular enemy returns:** Defeat a regular enemy, leave, and revisit. The
   regular enemies should be present again with full health.
2. **Collected placed item stays gone:** Collect a placed Health Cell, leave,
   and revisit. It should remain gone. Repeat with a Bomb Pack or Max Health Up.
3. **Uncollected item remains available:** Leave a room without collecting an
   item and return. It should still be there.
4. **Rejected item remains available:** Try a Health Cell at full health or a
   Bomb Pack at the upgrade cap. If collection is rejected, revisit and check
   the item is still present.
5. **Boss stays defeated:** Defeat the Warlord, leave the fortress, and return.
   The Warlord should not return, and the beacon should remain unlocked.
6. **One crate reward only:** Open a supply crate and collect its drop. Leave
   and revisit several times. The crate and its collected drop should not
   return, and the player should not gain the reward again.
7. **Uncollected crate drop is preserved:** Open a supply crate but leave its
   drop behind. Return and verify the crate remains open and its one drop is
   still available. Also try leaving immediately after destroying a crate.
   Collect the drop and revisit again; the drop should then be gone.
8. **Player progress carries over:** Change health, collect a Medkit or Shield,
   and collect an upgrade. Switch rooms and verify those player values remain.
9. **Reset clears room progress:** Collect an item and defeat the boss, then
   press R. Confirm that rooms, enemies, collectibles, crates, the boss, player
   health, and inventory return to their new-game state.

Automated regression checks can cover IDs, restored room state, and reset
behavior. A full playthrough should also check that room visits feel fair and
that opened crate drops are easy to find.

## Research and references

- The respawn comparison above applies common design tradeoffs to this game's
  existing rooms and items: repeating enemies adds activity, while repeating
  collectible rewards can make resource farming possible. These conclusions
  are recommendations for this project's current reward and progression
  systems, rather than claims that respawn behavior has one universal answer.
- MonoGame's official [game-loop guide](https://docs.monogame.net/articles/getting_to_know/whatis/game_loop/)
  describes `Update` as the place for game logic. It also explains that the
  game loop calls updates on a fixed interval by default. Room restoration and
  pickup state should therefore be handled as explicit game state during room
  construction and updates, not as a drawing-only effect.
- Current behavior is verified against the project source and its
  [gameplay guide](GameplayGuide.md) and [supply-crate guide](SupplyCrates.md).
