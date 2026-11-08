# Sprint 3

This Metroid-inspired MonoGame platformer includes seven connected rooms,
two enemies per room, eight enemy kinds including a boss, nine pickup kinds,
multiple weapons, and interactive platforms. Climb to each room's gold exit.
In the final room, defeat the Decepticon Warlord and touch the unlocked beacon
to complete the game.

Repository: [hanshil09/Sprint-2-3902](https://github.com/hanshil09/Sprint-2-3902).


## Controls

| Input | Action |
|---|---|
| Enter | Start from the menu. |
| A/D or Left/Right | Move horizontally. |
| W, Up, or J | Jump. |
| S or Down | Face downward; this does not move the player down. |
| Space | Switch between standing armadillo and smaller, faster ball form. |
| Z or N | Fire a normal horizontal shot in standing form. |
| K | Fire an angled shot in standing form. |
| 1 | Fire an orb in standing form. |
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
standing form. Medkits cannot be used at full health; stored Shields cannot be used
while a shield is already active. Object previews appear briefly when cycled
and do not replace or spawn the room's gameplay objects.

## Rooms and progression

Room layouts, spawns, and gravity are loaded from external text files in
[Content/Levels](../3902%20proj/Content/Levels). Each file has `name:` and `gravity:`
headers followed by a `[tiles]` grid. Parsing and validation are handled by
[LevelParser.cs](../3902%20proj/Core/Levelcreation/LevelParser.cs).

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
upgrades persist when switching rooms. Regular enemies respawn on a revisit,
but collected placed pickups stay gone, opened supply crates stay open, and
defeated bosses stay defeated. An opened crate's drop remains available if it
was not collected before leaving. Moving and crumbling platforms are recreated;
crumbling selections stay the same for that game, but their phases and
countdowns restart. Old projectiles are cleared. Press R to start a new game
and reset room progress.

## Pickups and health

Health and maximum health start at 100. Touch pickups to collect them.

| Pickup | Effect |
|---|---|
| Medkit | Adds one stored full-heal item, used with 3. |
| Shield | Adds one stored five-second shield, used with 4. |
| Health Cell | Immediately restores 10 HP, capped at current maximum health. |
| Shield Battery | Adds one stored shield charge, used with 4 for five seconds of protection. |
| Spark Core | Grants eight seconds of invincibility. |
| Turbo Boost | Grants six seconds of faster movement. |
| Energon | Increases the collected Energon count. |
| Bomb Pack | Increases bomb blast size, up to upgrade level 3. |
| Max Health Up | Raises maximum health by 20, up to 200, and fully heals. |

Health Cells stay available at full health, and Bomb Packs stay available at
level 3.
Max Health upgrades stay available when health and maximum health are both 200;
they can still heal an injured player at that capacity cap. Stored Medkits,
Shields, and Energon remain collectible. Repeated Spark Core and Turbo Boost
pickups refresh their existing effect instead of stacking another decorator.

Active shields prevent damage. At zero health, gameplay stops at the game-over
screen; R resets and Q/Escape quits.

## Blocks and supply crates

The game includes Ground, Brick, Stone, Metal, Crate, Hazard, Wood, Caution,
Alloy, and Morph Ball crate blocks. Ordinary crates break on player shots or
bomb explosions; red hazards damage the player on contact. Orange-red Morph
Ball crates only break when a bomb is dropped in vehicle (Morph Ball) form.
Touching them or shooting them does not break them.

Crate Workshop includes green supply crates containing one Health Cell and
gold supply crates containing one Energon pickup, plus an orange-red crate
containing a Bomb Pack. Destroy a supply crate, then touch its released pickup.
An opened crate cannot be farmed by revisiting the room. Plain crates remain
empty. Room files use `H` for Health Cell crates, `E` for Energon crates, `Q`
for a Morph Ball-only Bomb Pack crate, and `C` for empty crates. See
[supply crates](SupplyCrates.md).

Blue and purple reinforced crates resist shots and bombs. Press Space to transform, then
roll into one from either side. Level 1 has a fixed tutorial crate (`R`) that
drops Energon. Crate Workshop reinforces one of its existing supported supply
crates per new game; it keeps its original pickup and the same spot on revisits.

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
selection; layouts can repeat by chance. See [crumbling platforms](CrumblingPlatforms.md).

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
