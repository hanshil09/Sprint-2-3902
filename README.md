# Sprint 3

This Transformers-themed MonoGame platformer includes seven connected rooms,
two enemies per room, eight enemy kinds including a boss, nine pickup kinds,
multiple weapons, and interactive platforms. Climb to each room's gold exit.
In the final room, defeat the Decepticon Warlord and touch the unlocked beacon
to complete the game.

Repository: [hanshil09/Sprint-2-3902](https://github.com/hanshil09/Sprint-2-3902).

## Run

Install the .NET 9 SDK, then run from the repository root:

```text
dotnet run --project "3902 proj/3902 proj.csproj"
```

The project restores its NuGet dependencies. MonoGame's content builder builds
the sprite sheets and font; room files are copied to the output folder.

## Controls

| Input | Action |
|---|---|
| Enter | Start from the menu. |
| A/D or Left/Right | Move horizontally. |
| W, Up, or J | Jump. |
| S or Down | Face downward; this does not move the player down. |
| Space | Switch between standing robot and smaller, faster morph-ball forms. |
| Z or N | Fire a normal horizontal shot in robot form. |
| K | Fire an angled shot in robot form. |
| 1 | Fire an orb in robot form. |
| 2 | Drop a bomb in either form. Bombs are unlimited. |
| 3 | Use a stored Medkit to restore health to the current maximum. |
| 4 | Use a stored Shield for five seconds of protection. |
| 5 | Fire a piercing shot that can damage up to three different enemies. |
| 6 | Fire a three-way spread shot. |
| 7 | Throw a returning boomerang; only one can be active at a time. |
| T/Y | Preview previous/next block or interactive-object kind. |
| U/I | Preview previous/next pickup kind. |
| O/P | Preview previous/next enemy kind. |
| Left/Right mouse click | Switch to the previous/next room for testing. |
| Page Up/Page Down | Keyboard alternative for previous/next room. |
| F1 | Toggle the full controls and platform/crate legends during play. |
| E | Debug damage command; damages the player unless protection prevents it. |
| R | Reset gameplay or an end screen to the start menu. |
| Q or Escape | Quit. |

Numeric actions 1–7 also support the number pad. Weapons 1, 5, 6, and 7 require
robot form. Medkits cannot be used at full health; stored Shields cannot be used
while a shield is already active. Object previews appear briefly when cycled
and do not replace or spawn the room's gameplay objects.

## Rooms and progression

Room layouts, spawns, and gravity are loaded from external text files in
[Content/Levels](3902%20proj/Content/Levels). Each file has `name:` and `gravity:`
headers followed by a `[tiles]` grid. Parsing and validation are handled by
[LevelParser.cs](3902%20proj/Core/Levelcreation/LevelParser.cs).

| Order | Room | File | Gravity (pixels per second squared) |
|---|---|---|---:|
| 1 | Ground Courtyard | desert-01.room | 1800 |
| 2 | Brick Stairway | laboratory-02.room | 1000 |
| 3 | Stone Switchbacks | stone-03.room | 1900 |
| 4 | Metal Towers | metal-04.room | 600 |
| 5 | Crate Workshop | crates-05.room | 800 |
| 6 | Hazard Crossing | hazards-06.room | 1550 |
| 7 | Decepticon Fortress | fortress-07.room | 1100 |

The first six rooms have an elevated gold forward door and a cyan return door.
The fortress has a return door and a beacon that unlocks after the Warlord is
defeated. Quick room navigation wraps at either end. Moving platforms,
elevators, and spring pads provide additional movement interactions.

Only the active room updates and draws. The player's health, inventory, and
upgrades persist when switching rooms. The room is recreated on a revisit:
enemies, pickups, destroyed crates, and platform phases reset, and old
projectiles are cleared. Crumbling-platform selections stay the same for that
game; their countdowns do not persist between visits.

## Pickups and health

Health and maximum health start at 100. Touch pickups to collect them.

| Pickup | Effect |
|---|---|
| Medkit | Adds one stored full-heal item, used with 3. |
| Shield | Adds one stored five-second shield, used with 4. |
| Health Cell | Immediately restores 10 HP, capped at current maximum health. |
| Shield Battery | Immediately adds five seconds of shield time, capped at ten seconds remaining. |
| Spark Core | Grants eight seconds of invincibility. |
| Turbo Boost | Grants six seconds of faster movement. |
| Energon | Increases the collected Energon count. |
| Bomb Pack | Increases bomb blast size, up to upgrade level 3. |
| Max Health Up | Raises maximum health by 20, up to 200, and fully heals. |

Health Cells stay available at full health. Shield Batteries stay available at
the ten-second protection cap, and Bomb Packs stay available at level 3.
Max Health upgrades stay available when health and maximum health are both 200;
they can still heal an injured player at that capacity cap. Stored Medkits,
Shields, and Energon remain collectible. Repeated Spark Core and Turbo Boost
pickups refresh their existing effect instead of stacking another decorator.

Active shields prevent damage. At zero health, gameplay stops at the game-over
screen; R resets and Q/Escape quits.

## Blocks and supply crates

The game includes Ground, Brick, Stone, Metal, Crate, Hazard, Wood, Caution, and
Alloy block kinds. Ordinary crates break on player shots or bomb explosions;
red hazards damage the player on contact.

Crate Workshop includes green supply crates containing one Health Cell and
gold supply crates containing one Energon pickup. Shoot or bomb a crate, then
touch its released pickup. Each supply crate drops only once per room instance.
Plain crates remain empty. Room files use `H` for Health Cell crates, `E` for
Energon crates, and `C` for empty crates. See [supply crates](Documentation/SupplyCrates.md).

## Crumbling platforms

Each new game, including an R reset, selects 2–3 existing elevated platform
sections per room to crumble when the player stands on them.

| Color | Delay after standing |
|---|---|
| Coral | 0.9 seconds |
| Gold | 1.5 seconds |
| Purple | 2.3 seconds |

The countdown continues after stepping off. Platforms flash, disappear for at
least three seconds, and show a faded, non-solid preview for 0.6 seconds before
returning. Restoration waits if the player occupies their space. Main floor
tiles, crates, and hazards are excluded from selection. New games reroll the
selection; layouts can repeat by chance. See [crumbling platforms](Documentation/CrumblingPlatforms.md).

## Enemies and weapons

Flyers patrol and swoop, crawlers stalk, hoppers leap, beetles charge, and wavers
follow drifting flight paths. Metal Towers introduces a stationary burst Sentry
and a vertical Sentinel. Decepticon Fortress contains the Warlord boss. Ground
enemies use gravity, while enemy movement checks handle solid platforms and
room boundaries.

Shots, orbs, piercing shots, spread shots, boomerangs, bombs, and enemy
projectiles have separate movement or hit behavior. Bombs fall under the active
room's gravity, rest on solid blocks, and explode after a fuse. Collision
handling includes landing, ceilings, walls, hazards, enemies, projectile impacts,
and room transitions. Pickups and interactive-object triggers are processed
through their own interfaces.

## Checks and code quality

The project uses built-in .NET Roslyn analyzers: `latest-recommended` normally,
and `latest-all` with warnings treated as errors in the Strict configuration.
Run from the repository root:

```text
dotnet build "3902 proj.sln" -c Strict
dotnet run --project RegressionChecks -c Strict
dotnet run --project RegressionChecks -c Strict --no-build -- --extended
dotnet run --project RegressionChecks -c Strict --no-build -- --crumbling
dotnet run --project RegressionChecks -c Strict --no-build -- --crate-drops
dotnet run --project RegressionChecks -c Strict --no-build -- --pickups
```

The last verification after the pickup fix passed with zero build warnings or
errors and 278 assertions across five modes: 43 basic room, 129 extended,
51 crumbling-platform, 23 supply-crate, and 32 pickup checks. These are assertion
counts, not separate test-method counts. See the [regression-check guide](RegressionChecks/README.md)
for coverage and limitations.

The [Sprint 3 analyzer summary](Documentation/CodeAnalyzer/Sprint3-Summary.md)
records the October 14–19 history; the [Sprint 2 results](Documentation/CodeAnalyzer/Sprint2-Results.md)
preserve the earlier analysis. Current `.editorconfig` exceptions retain
interface-based integration (CA1859) and allow non-security gameplay randomness
only in RoomFactory and CrumblingPlatformLayout (CA5394). The latter exception
is also explained in the crumbling-platform documentation.

## Code reviews and sprint documentation

Sprint 3 reviews:

- [Hanshil's review](Documentation/CodeReviews/sprint3_reviews/Hanshil-Sprint3-CodeReview.txt).
- [Arnav's review of Hanshil's PlayerArsenal](Documentation/CodeReviews/sprint3_reviews/Arnav-Hanshil-Sprint3-CodeReview.txt).

Reviewers should record their actual review time and complete the required
readability and maintainability reviews, including having their own work
reviewed. Arnav's review currently has a review-time placeholder to fill in.
Historical reviews are kept in [sprint2_reviews](Documentation/CodeReviews/sprint2_reviews).

The saved [reflection](Documentation/SprintReflection.md) and
[burndown image](Documentation/burndown_chart.png) describe Sprint 2. A Sprint 3
reflection based on the current taskboard's burndown remains to be added.

## Known limitations and deviations

- This project uses platformer movement and Transformers/Metroid-style artwork
  instead of Zelda's top-down movement. The equivalent feature scope still
  needs confirmation against the grader-approved project plan.
- Normal shots remain horizontal when facing down; K is the separate angled
  shot command.
- Room object state resets on revisits, so collected items and crate drops can
  be obtained again. Player inventory and upgrades persist.
- Normal-shot recoil directly changes horizontal position without resolving
  wall collisions. Firing beside a wall needs a collision-safe recoil fix.
- Automated room reachability checks model static geometry. They do not prove
  every randomized, timed platform route is playable. A full interactive
  playthrough, combat-balance check, and upward platform-carrying/ceiling check
  remain pending.

## Credits

- Metroid SNES Enemy Sprites by ronny14:
  [sprite source](https://www.spriters-resource.com/custom_edited/metroidcustoms/asset/55700/).
- Samus Aran Metroid Sprites by Mister Mike:
  [sprite source](https://www.spriters-resource.com/nes/metroid/asset/1774/).
