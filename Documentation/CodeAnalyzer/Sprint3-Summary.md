# Sprint 3 Code Analyzer Summary

Updated: October 26, 2026. Analysis level: `latest-all` (Strict configuration).

Code analysis was used during Sprint 3 and checked again before submitting
the sprint. This document combines the earlier cleanup with the final
pre-submission check.

## October 14–19 history


| Date | Recorded change or check |
|---|---|
| Oct 14 | Collision framework merged into main (`4275443`). |
| Oct 15 | External level parsing added on Levels (`e844f54`). |
| Oct 16 | No saved analyzer run. |
| Oct 17 | Collision corrections, pickup controls, and room lifecycle added on their branches (`4620a7b`, `f419293`, `71f55c0`). |
| Oct 18 | No saved analyzer run. |
| Oct 19 | Integrated branches; fixed missing Strict solution configuration. Initial strict result: **148 warnings, 0 errors**. After cleanup: **0 warnings, 0 errors**; **29 regression checks passed**. |

## Warning resolutions

| Rule | Initial count | Resolution |
|---|---:|---|
| CA1515 | 90 | Game implementation types are internal. Only RegressionChecks has friend-assembly access for testing. |
| CA1062 | 49 | Implementation methods no longer form an externally visible public API. Existing boundary validation remains; internal visibility does not imply every argument now has a null guard. |
| CA1002 | 3 | Room/level methods return read-only lists, and collision input is copied into an owned snapshot. |
| CA1030 | 3 | Renamed weapon commands to ShootEnergy, LaunchOrb, and LaunchProjectile and updated callers. |
| CA2213 | 3 | Dispose graphics resources in Game1.Dispose(bool), clearing references for repeated disposal. |

## October 26 — final analysis before Sprint 3 submission

Before submitting Sprint 3, we ran the Strict code analyzer again on the latest
game code. It reported eight analyzer findings, which were treated as build
errors. We fixed these issues without disabling rules or lowering the analysis
level.

| Rule | Count | Problem and fix |
|---|---:|---|
| CA1515 | 4 | IGameObject, IProjectile, Direction, and CollisionHelper did not need to be public. Changed them to internal. |
| CA1052 | 1 | CollisionHelper only contains static methods. Made the class static. |
| CA1062 | 1 | ImpactTime used projectile without checking for null. Added ArgumentNullException.ThrowIfNull(projectile). |
| CA1823 | 2 | Removed unused ShootFrameTime from PlayerSpriteFactory.cs and PanelColor from ControlsOverlay.cs. |

After these fixes, the game-only Strict rebuild passed with **0 warnings and
0 errors**. This final check followed the earlier October 19 cleanup; the two
results were recorded on different versions of the code, not one combined run.

No new analyzer suppressions were added for these fixes. Existing exceptions
remain in `.editorconfig`. Strict treats warnings as errors so new findings
cannot silently pass the build.

## Run code analysis

From the repository root:

```powershell
dotnet build "3902 proj/3902 proj.csproj" -c Strict -t:Rebuild
```

This checks the game project only and does not run regression tests.
The verified rebuild used `--no-restore` with dependencies already restored.
Rebuild forces analyzers to run even when previous output is up to date.

## Separate test-project repair

The October 26 solution build also exposed 23 compile errors in the existing
regression project. Its outdated movement calls and empty ParkourCheck were
repaired to match the current game, without adding checks. All existing suites
then passed, and the full Strict solution rebuild had zero warnings/errors.
These were test-project repairs, not additional code-analyzer findings.
See [the regression guide](../../RegressionChecks/README.md) for details.

## Metrics note

Retrospective source-size comparison collected October 19: pre-integration commit `4275443` had 65 production C# files / 3,585 physical lines; a pre-cleanup integrated snapshot had 79 files / 5,130 lines, excluding bin/obj. This is source size, not measured cyclomatic complexity or a weekly metrics history.
