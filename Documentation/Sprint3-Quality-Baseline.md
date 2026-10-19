# Sprint 3 analyzer and metrics baseline

Date: October 19, 2026. Reviewer/tool operator: Codex.

The default build retains latest-recommended analysis. Configuration Strict enables latest-all:

```
dotnet build "3902 proj.sln" -c Strict
```

A full latest-all build before the final door integration completed with 148 warnings and zero errors. Counts below remove the duplicate warning listing printed in MSBuild's summary:

| Rule | Count | Disposition |
|---|---:|---|
| CA1515 | 89 | Review public/internal boundaries. RegressionChecks references game APIs, so blanket internal conversion is inappropriate without a friend assembly or test boundary change. |
| CA1062 | 49 | Missing public argument validation; open cleanup work. |
| CA1002 | 3 | Public concrete collection exposure; replace with read-only abstractions where practical. |
| CA1030 | 3 | Fire-named methods are commands rather than events. Review naming; no blanket suppression added. |
| CA2213 | 3 | Review MonoGame ownership/disposal for graphics manager, sprite batch, and overlay. |
| CA1307 | 1 | Fixed LevelParser.IndexOf to explicitly use ordinal comparison after this baseline. |

No latest-all warnings were suppressed to create a clean result. Future changes may change these counts; rerun Strict on the submission commit and record results.

A source-size snapshot before final documentation updates contained 79 production C# files and 5,130 physical source lines (excluding bin/obj). This is a reproducible size metric, not cyclomatic complexity or maintainability index. It is the first current snapshot, not proof of weekly historical measurements. Record future Visual Studio code metrics with date/commit, complexity, maintainability index, and warning counts.

Retrospective comparison: commit 4275443 (pre-integration main) contains 65 production C# files and 3,585 physical source lines. Both measurements were collected October 19; the earlier commit measurement was reconstructed from Git, not recorded contemporaneously.

Human reviews and reflection statements must be supplied by the actual team members. AI-generated reviews should remain labeled as such.
