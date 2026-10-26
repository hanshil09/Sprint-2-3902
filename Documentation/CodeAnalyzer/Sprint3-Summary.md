# Sprint 3 Code Analyzer Summary

Updated: October 19, 2026. Analysis level: `latest-all` (Strict configuration).

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

No new analyzer suppressions were added. The existing CA1859 exception for interface-based integration remains in `.editorconfig`.

Strict now treats warnings as errors so future changes cannot silently reintroduce warnings.

## Reproduce

From the repository root:

```powershell
dotnet build "3902 proj.sln" -c Strict -t:Rebuild
```

The verified full strict rebuild used `--no-restore` with restored dependencies. Rebuild forces analyzers to run even when output is up to date.

## Metrics note

Retrospective source-size comparison collected October 19: pre-integration commit `4275443` had 65 production C# files / 3,585 physical lines; a pre-cleanup integrated snapshot had 79 files / 5,130 lines, excluding bin/obj. This is source size, not measured cyclomatic complexity or a weekly metrics history.
