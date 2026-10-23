# Sprint 3

This MonoGame platformer has six connected parkour rooms, two enemies per room, four pickup types, and multiple weapons. Each room focuses on a different structure. See the [room guide](Documentation/RoomGuide.md) for the route and controls.

## Repository

https://github.com/hanshil09/Sprint-2-3902

## Run

From the repository root, run:

```text
dotnet run --project "3902 proj/3902 proj.csproj"
```

## Controls

Each new game (including **R** reset) selects **2–3 elevated platform sections per room**
to crumble when the player stands on them. Coral lasts **0.9 seconds**, gold **1.5 seconds**,
and purple **2.3 seconds**. They flash before disappearing, remain absent for three seconds,
then show a faded return preview. A platform waits to become solid if the player occupies
its space. Room revisits keep the current game's platform selection. Existing floor tiles
and platform positions are preserved. See [crumbling platforms](Documentation/CrumblingPlatforms.md).

Sprint 3 additions: **5** fires a three-enemy piercing shot; **6** fires a three-way spread (numpad supported). Cyan Health Cells heal 10 immediately; cyan Shield Batteries grant five seconds of protection, capped at ten seconds. Orange crates break on player shots/explosions; red hazards damage on contact. Metal door markers at room edges transition to the preceding/following room. At zero health, gameplay stops; **R** resets and **Q** quits.

Metal Towers includes an orange stationary burst Sentry and a cyan vertical Sentinel. Seven enemy kinds are distributed across the six rooms. Gold doors lead forward after the jumping route; cyan doors near the entrance lead back. Existing artwork is reused with tinting for these variants.

- **Enter** starts from the menu.
- **A/D** or **Left/Right** move horizontally. **W**, **Up**, or **J** jumps. **S** or **Down** changes the player's facing direction downward; it does not move the player down.
- **Space** changes between standing and morph ball forms, where morph ball is smaller and moves faster.
- **Z/N** fire a normal shot, while **K** fires an angled shot. **1** fires an orb in standing form, and **2** drops a bomb. The number pad's **1/2** keys also work.
- Touch a Medkit or Shield to add it to inventory. **3** uses a Medkit to restore up to 25 health; **4** activates a Shield for five seconds. The number pad's **3/4** keys also work. Medkits cannot be used at full health, and Shields cannot be stacked.
- **E** briefly shows the damaged effect.
- **F1** toggles full controls; the default display leaves the platforms visible. Showcase blocks/items are no longer drawn during gameplay.
- **Left/Right mouse click** switches to the previous/next room. **Page Up/Page Down** provides the same keyboard fallback. Room navigation wraps at both ends.
- **R** resets the game to the menu. **Q** or **Escape** quits.

## Implemented behavior

Six external room files define separate ground, brick, stone, metal, crate, and hazard layouts. Exactly two enemies update and draw in each room. See [room format](Documentation/LevelFormat.md) and [integration status](Documentation/Sprint3-Integration-Assessment.md). Run `dotnet run --project RegressionChecks/RegressionChecks.csproj --no-restore` from the repository root for automated checks.

- The player can move, jump on platforms, face horizontal directions and down, switch forms, and animate. Damaged and temporary shooting states alter the player's sprite and available actions.
- Normal shots and orbs travel horizontally, while the **K** shot travels at an angle. Bombs show a timed explosion and then expire.
- Each room has an elevated forward door and a ground-level return door. Crates can be destroyed; red hazards hurt the player. Durable ground platforms keep the crate room traversable after destruction.
- Pickups are removed from the level after collection and added once to the player's persistent inventory. Player health starts at 100; Medkits restore health up to that maximum, and active Shields block damage.
- The flyer patrols and swoops, the crawler stalks the player, the hopper leaps, the beetle charges, and the waver follows a drifting flight path.
- Ground enemies use gravity and avoid platform edges. Flying enemies collide with solid blocks and remain inside the game window.
- Only the active room updates and draws. Switching rooms unloads the prior room's projectiles and room-owned objects; the player persists and is repositioned at the selected room's start.

## Known gaps

- Automated movement checks cover all six exits, but full combat-balance playtesting is still pending.
- Normal shots still travel horizontally when the player is facing down. The separate **K** command fires the angled shot.
- The project has not been verified against the grader-approved Transformers feature list or through a full interactive playthrough.

## Development notes

- The game targets .NET 9 and uses MonoGame DesktopGL. The project file restores its NuGet dependencies, and `Content/Content.mgcb` builds the sprite sheets.

## Code quality process

Sprint 3 strict analysis: `dotnet build "3902 proj.sln" -c Strict -t:Rebuild`. See the [CodeAnalyzer summary](Documentation/CodeAnalyzer/Sprint3-Summary.md) for the October 14–19 history, fixes, and verified zero-warning result.

The project enables the built-in .NET Roslyn analyzers at the `latest-recommended` level. Run the following check before merging changes:

```text
dotnet build "3902 proj.sln" --no-restore
```

The first Sprint 2 analyzer pass found eight warnings. Its historical resolutions are preserved in [Sprint 2 results](Documentation/CodeAnalyzer/Sprint2-Results.md). Current analysis documents are grouped under Documentation/CodeAnalyzer.

The Sprint 3 collision inventory, expected responses, current pass/fail status, and reproducible manual regression steps are recorded in [Sprint3-Collision-Matrix.md](Documentation/Sprint3-Collision-Matrix.md).

## Code review process

Our completed review is saved in [Hanshil-Arnav-CodeReviews.txt](Documentation/CodeReviews/Hanshil-Arnav-CodeReviews.txt).

One completed review is saved in [Arnav-Hanshil-Branch-CodeReviews.txt](Documentation/CodeReviews/Arnav-Hanshil-Branch-CodeReviews.txt).

## Sprint reflection

The completed [Sprint 2 reflection](Documentation/SprintReflection.md) discusses the team's results, development process, burndown trend, and plans for the next sprint. 


## Credits
Metroid SNES Enemy Sprites by ronny14
https://www.spriters-resource.com/custom_edited/metroidcustoms/asset/55700/

Samus Aran Metroid Sprites by Mister Mike
https://www.spriters-resource.com/nes/metroid/asset/1774/
