# Sprint 3 integrated requirement assessment

Date: October 19, 2026. Integration branch: hanshil_tasks.

All eight remote branches were fetched and integrated, including main, Enemies, projectiles, Levels, collision, sprint3-collision-fixes, sprint3-item-pickups-controls, and sprint3-room-lifecycle. Branch history is preserved with merge commits. The same final commit is intended for the local sprint3-room-lifecycle branch.

## Implemented improvements

- External room loading, parser diagnostics, invalid-data fallback, and two ordered room files.
- Active-room-only processing, all room enemies active, old projectile cleanup, safe entity spawn placement, and mouse/Page Up/Page Down navigation with wraparound.
- Rectangle collision handling, swept projectile tests, nearest-first impacts, wall and ceiling resolution, item collection, player damage/invulnerability, shield protection, health, and a game-over state.
- Seven enemy kinds: the original five plus stationary burst Sentry and vertically moving Sentinel.
- Four pickups: stored Medkit and Shield, immediate Health Cell (+10 health), and Shield Battery (+5 seconds protection, capped at 10 seconds).
- Additional projectile behaviors: key 5 fires a projectile that pierces up to three distinct enemies; key 6 fires a three-way spread. Ordinary shots, angled shots, orbs, and bombs remain.
- Environmental variety: crates break on player shot or explosion, red non-solid hazards damage players, and door triggers change rooms after collision processing completes.
- Added objects use tinted existing art. These are gameplay variants, not claims of new original artwork.
- Strict analyzer configuration, automated regression checks, source-size baseline, AI review, and explicit gaps.

## Requirement status

| Requirement | Status and evidence |
|---|---|
| Room subsets stored externally and initialized from files | Implemented in two .room files and LevelLoader/LevelParser. |
| Every room quickly reachable through mouse controls | Implemented; ordered rooms wrap and old projectiles clear. |
| Artificial test level includes every implemented object type | Laboratory includes all seven enemies, four pickups, crate/hazard/doors; base blocks are distributed across the two-room collection. Weapons are spawned by numeric controls. |
| Player/block, enemy/block, player/enemy, projectile/block/enemy/player | Implemented core responses; automated checks cover selected paths, not every combination. |
| Bomb/environment collision | Crates break; other block types intentionally remain indestructible. Bomb damage is once per enemy per explosion. |
| Item effects | Integrated inventory, healing, timed shields, and new immediate pickups. |
| Door transitions | Parsed door direction selects previous/next room, queues transition after collision pass, uses destination player spawn. |
| Damage/death | Health/invulnerability/shields integrated; zero health freezes gameplay and shows reset/quit menu. |
| Professor request for more enemies/items/projectiles/environment | Added two enemies, two pickups, two weapon behaviors, breakable crate, damage hazard, and doors. Grader acceptance and gameplay balance require review. |
| Controls | Platformer mapping retained: A/D/arrows move horizontally, W/Up jumps, S/Down faces down. This differs from literal four-direction Zelda movement; instructor approval of this project-specific deviation is not established. |
| Complete first dungeon | Not established. Two artificial rooms do not prove all rooms of the intended dungeon are implemented. |
| Six teammates review and are reviewed | Still requires actual human reviews from all six. AI reviews do not substitute for those records. |
| Reflection with each teammate's input | Still requires each member's own input. |
| Metrics tracked over time | Retrospective source-size baseline and current snapshot can be recorded now. Weekly Visual Studio metrics/complexity history remains to be collected. |
| Stricter analysis | Strict configuration enables latest-all; outstanding warnings are recorded in Sprint3-Quality-Baseline.md and are not hidden. |
| Complete pass/fail regression evidence | Automated coverage exists; a complete graphical playthrough and per-pair manual evidence remain pending. |
| Carmen, CATME, grader review | Team must complete actual course submissions and review arrangements. |

## Verification

Run from the repository root:

```
dotnet build "3902 proj.sln" --no-restore
dotnet run --project RegressionChecks/RegressionChecks.csproj --no-restore
dotnet build "3902 proj.sln" -c Strict
```

The normal build uses latest-recommended analyzers. The Strict configuration enables all analyzers and currently produces warnings that need triage. No visual playthrough or subjective professor-grade claim is made by this assessment.
