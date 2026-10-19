# Sprint 3 Code Analyzer Results

Date: October 19, 2026  
Branch tested: sprint3-room-lifecycle  
Analysis: built-in .NET Roslyn analyzers, latest-all

## Configuration error and fix

The original command failed with MSB4126 because Strict existed as a conditional configuration in the project file, but the solution listed only Debug and Release. Solution builds validate their configuration before loading the project, so analysis never ran.

Added Strict to the solution's configuration list for Any CPU, x64, and x86. Each maps to the project's Strict / Any CPU configuration, consistent with the existing Debug and Release platform mappings. The project already sets AnalysisLevel to latest-all when Configuration is Strict.

## Reproduce the analyzer run

From the repository root:

```powershell
dotnet build "3902 proj.sln" -c Strict -t:Rebuild
```

Rebuild ensures the compiler and analyzers run even if an earlier build is up to date. To retain the full console output:

```powershell
dotnet build "3902 proj.sln" -c Strict -t:Rebuild 2>&1 | Tee-Object -FilePath "Documentation\Sprint3-CodeAnalysis.txt"
```

If dependencies have already been restored, add --no-restore. To bypass the solution and analyze the project directly:

```powershell
dotnet build "3902 proj/3902 proj.csproj" -c Strict -t:Rebuild
```

## Recorded result

Verified command: `dotnet build "3902 proj.sln" -c Strict --no-restore -t:Rebuild -v:q`

- Build completed successfully, exit code 0.
- Errors: 0.
- Analyzer warnings: 148.
- MSB4126 no longer occurs.

The console repeats warning details in its final summary. Do not add both copies together; use the final MSBuild warning total or deduplicate identical diagnostics.

## Warning review and remaining work

| Rule | Count | Meaning | Required follow-up |
|---|---:|---|---|
| CA1515 | 90 | Application types can be internal | Review visibility with the separate RegressionChecks assembly in mind; avoid breaking its references. |
| CA1062 | 49 | Public methods need null argument validation | Add argument checks at appropriate public boundaries and verify behavior. |
| CA1002 | 3 | Public APIs expose concrete List collections | Prefer read-only collection interfaces where callers do not need mutation. |
| CA1030 | 3 | Fire-prefixed methods look like event raisers | Review command names or explain why these are ordinary weapon methods. |
| CA2213 | 3 | Disposable fields require ownership/disposal review | Check graphics manager, sprite batch, and controls overlay cleanup against MonoGame lifetime behavior. |

These warnings are unresolved work, not compiler errors or proof of runtime failure. This change fixes the configuration and records the analysis; it does not claim that all strict warnings are resolved. No new warning suppressions were added. The existing CA1859 suppression remains documented in .editorconfig for interface-based integration.

This report is the Sprint 3 analyzer evidence. The older Sprint3-Quality-Baseline.md is a historical measurement from before final integration. Weekly metrics and human code reviews remain separate team requirements.
