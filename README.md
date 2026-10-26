# Sprint 3 — Armadillo Platformer

A MonoGame platformer with seven rooms, a boss, multiple weapons and pickups,
moving platforms, supply crates, and crumbling platforms. Reach each gold exit;
defeat the Warlord and touch the final beacon to finish.

## Run

Install the .NET 9 SDK, then run from the repository root:

```powershell
dotnet run --project "3902 proj/3902 proj.csproj"
```

## Basic controls

| Keys | Action |
|---|---|
| Enter | Start |
| A/D or arrows | Move |
| W / Up / J | Jump |
| Space | Transform |
| Z / N | Shoot |
| 2 | Drop bomb |
| 3 / 4 | Use medkit / shield |
| Left / right mouse click | Previous / next room (testing) |
| F1 | Full controls |
| R | Reset to menu |
| Q / Escape | Quit |

See the [gameplay guide](Documentation/GameplayGuide.md) for all weapons,
pickups, room details, limitations, and artwork credits.

## Known bugs and limitations

- Normal-shot recoil changes position directly without wall collision
  resolution; firing beside a wall can cause overlap.
- Facing down does not aim normal shots downward; use K for angled shots.
- Revisiting a room resets enemies, pickups, crates, and platform phases,
  allowing pickups to be collected again. Player inventory/upgrades persist.
- Automated route checks model simplified static geometry, not full combat or
  every randomized crumbling-platform route. Manual gameplay and balance
  testing is still needed.
- When Player is hit with a projectile while on a platform moving upwards,
  they fall through the platform. This is a collision error.

## Tools and development process

- **Static analysis:** built-in .NET Roslyn analyzers, with a Strict build using
  `latest-all` and warnings treated as errors to catch code-quality problems.
- **Automated regression checks:** a basic room suite plus optional focused
  suites for combat/physics, crumbling platforms, supply crates, and pickups.
- **Team workflow:** Git branches and merges, GitHub task tracking, peer code
  reviews.
- **Code metrics:** the analyzer summary records an October 19 retrospective
  comparison of production source-file and line counts. These measure project
  size, not complexity or proof of quality; no continuous complexity history
  is claimed.

## Sprint 3 code quality and reviews

- **Sprint reflection:** [Team progress and next-sprint plans](Documentation/SprintReflection.md).
- **Code analyzer:** [Sprint 3 summary and October 14–26 history](Documentation/CodeAnalyzer/Sprint3-Summary.md).
- **Code reviews:** [Sprint 3 review folder](Documentation/CodeReviews/sprint3_reviews).

Run strict analysis and the basic room checks:

```powershell
dotnet build "3902 proj.sln" -c Strict
dotnet run --project RegressionChecks -c Strict --no-restore
```

The basic suite contains 43 room assertions. Extra checks are optional;
see the [regression guide](RegressionChecks/README.md). Automated reachability
checks use simplified geometry and do not replace manual playtesting.
