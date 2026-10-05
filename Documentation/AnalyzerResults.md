# Sprint 2 Code Analyzer Results

## October 5 2026

Command used:

```text
dotnet build "3902 proj.sln" --no-restore -p:EnableNETAnalyzers=true -p:AnalysisLevel=latest-recommended
```

Initial result: the build succeeded with 0 errors and 8 analyzer warnings.

| Rule | Location | Resolution |
| --- | --- | --- |
| CA1822 | `Behaviors/GroundBehavior.cs` | Made `WalkSafely` static because it does not access instance state. |
| CA1716 | `IGameObjectCycler.Next` | Renamed the method to `NextObject` because `Next` is a reserved word in another .NET language. |
| CA1001 | `UI/ControlsOverlay.cs` | Implemented `IDisposable` and disposed its generated texture from `Game1.UnloadContent`. |
| CA1859 | Four integration fields in `Game1.cs` | Suppressed in `.editorconfig`. The fields intentionally use interfaces to preserve replaceable implementations and match the assignment's interface-based design goal. |

Final result: the build succeeded with 0 errors and 0 warnings.

