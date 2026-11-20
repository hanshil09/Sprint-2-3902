# Sprint 4 — Armadillo Platformer

Start in Command Hub, collect keys to unlock rooms, and defeat the Warlord.
Touch the final beacon to win.

## Run the game

Install the .NET 9 SDK and run this from the repository root:

```powershell
dotnet run --project "3902 proj/3902 proj.csproj"
```

## Controls

| Key | Action |
|---|---|
| Enter | Start |
| A / D or Left / Right | Move |
| W / Up / J | Jump |
| Space | Switch forms |
| Tab | Pause, preview rooms, and select a fireball |
| X / Shift + X | Fire / aim a compatible shot upward |
| 2 / 3 / 4 / 7 | Bomb / med kit / shield / boomerang |
| F1 / H | Show all controls |
| M / R / Q | Mute / reset / quit |

See the [gameplay guide](Documentation/GameplayGuide.md) for the other controls,
room and weapon details, and credits.

## Sprint 4 documentation

- [Reflection and burndown chart](Documentation/SprintReflection.md)
- [Code reviews](Documentation/CodeReviews/sprint4%20code%20rveiew)
- [Code quality changes](Documentation/CodeQuality/Sprint4-CodeQuality.md)
- [Strict .NET analyzer results](Documentation/CodeAnalyzer/Sprint4-Results.md)
- [Automated regression checks](RegressionChecks/README.md)

The team also playtested the game manually.

## Possible bugs

- Returning to the hub may fail if there’s no clear spot to the right of the door.
- The player may get stuck if they overlap multiple blocks at once.

## Sprint 5 planning

The current rooms have simple layouts. In Sprint 5, we plan to build more complex rooms and update combat, enemies, inventory, the HUD and menus, and Wild West audio.
