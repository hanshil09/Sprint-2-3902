# Regression checks

Run commands from the repository folder.

## Basic room checks (default)

`dotnet run --project RegressionChecks -c Strict --no-restore`

Program.cs checks seven room files, then six conditions per room: valid data/player spawn, two different enemies, one to three pickups, a return door plus exactly one forward door or goal, an elevated exit, and simulated exit reachability. A successful run reports 43 assertions across seven rooms, not 43 separate test methods.

## Original extended checks (optional)

`dotnet run --project RegressionChecks -c Strict --no-restore -- --extended`

ExtendedChecks.cs preserves the original full suite, including floor themes, physics, projectiles, mouse input, and bombs. These checks are not included in the basic total. Existing historical full-suite results do not describe the smaller default run.

Both modes stop at the first failure. For the presentation, replace one empty dot in a room with an enemy symbol to demonstrate the enemy-count failure, then restore the dot. Keep row widths unchanged. Parkour simulation is simplified and does not replace manual visual/combat playtesting.
