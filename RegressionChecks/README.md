# Regression checks

Run commands from the repository folder.

## Basic room checks (default)

`dotnet run --project RegressionChecks -c Strict --no-restore`

Program.cs checks seven room files, then six conditions per room: valid data/player spawn, two different enemies, one to three pickups, a return door plus exactly one forward door or goal, an elevated exit, and simulated exit reachability. A successful run reports 43 assertions across seven rooms, not 43 separate test methods.

The existing movement checks use MovementProbe, a headless body routed through
the game's current Physics and CollisionManager APIs. ParkourCheck uses the
actual safe spawn, sprite dimensions, and form speeds. It searches in robot
form, then tries the smaller transformed form if needed. It still assumes
blocking crates have been cleared and excludes combat, springs, and dynamic
platform behavior. Some arena sizes and the jump impulse remain fixed test
settings. This is a limited reachability search, not a full game simulation.

October 26 verification: 43 basic, 129 extended, 51 crumbling, 23 crate-drop,
and 32 pickup assertions passed. Existing counts are unchanged; no new checks
were added. The Strict solution and regression builds have zero warnings/errors.

## Pickup checks (optional)

`dotnet run --project RegressionChecks -c Strict --no-restore -- --pickups`

Checks capped pickups staying available, later collection after damage or shield
time elapses, exact healing/timer caps, inventory items at full health, decorator
forwarding, and repeated temporary power-up refreshes.

## Supply crate checks (optional)

`dotnet run --project RegressionChecks -c Strict --no-restore -- --crate-drops`

Checks room-file contents, shot and bomb destruction, exactly one pickup per
crate, collection effects, and empty/indestructible blocks.

## Crumbling platform checks (optional)

`dotnet run --project RegressionChecks -c Strict --no-restore -- --crumbling`

Covers platform phases, player contact, restoration safety, and randomized layouts
across all rooms. These assertions are separate from the basic and extended totals.

## Original extended checks (optional)

`dotnet run --project RegressionChecks -c Strict --no-restore -- --extended`

ExtendedChecks.cs preserves the original full suite, including floor themes, physics, projectiles, mouse input, and bombs. These checks are not included in the basic total. Existing historical full-suite results do not describe the smaller default run.

All modes stop at the first failure. For the presentation, replace one empty dot in a room with an enemy symbol to demonstrate the enemy-count failure, then restore the dot. Keep row widths unchanged. Parkour simulation is simplified and does not replace manual visual/combat playtesting.
