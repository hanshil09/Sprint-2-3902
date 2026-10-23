# Crumbling platforms

New-game initialization gives each room a random seed. RoomFactory reuses that
seed on room revisits and replaces all seeds on reset. Each room selects two or
three existing elevated platform runs, with distinct fast, medium, and slow
variants. The main floor, crates, and hazards are not candidates. Layout geometry
is unchanged when platforms are initially solid; individual layouts can repeat
by chance, as with any random selection.

| Variant | Color | Delay after standing |
|---|---|---|
| Fast | Coral | 0.6 seconds |
| Medium | Gold | 1.2 seconds |
| Slow | Purple | 2 seconds |

Standing starts a countdown that continues after stepping off. The platform
flashes, becomes non-solid and invisible for three seconds, then displays a
non-solid faded preview for 0.6 seconds before returning. Restoration waits if
the player's last reported bounds overlap the platform. Side contact, enemy
contact, and projectile hits do not start the timer.

## Design and course examples

- IStandingBlock extends IBlock with player-contact notifications. Player uses
  that interface; general block collision continues to use IBlock.IsSolid.
- CrumblingPlatform delegates phases and elapsed-time timers to
  CrumblingPlatformStateMachine, using an enum and switch like the professor's
  `9-GoombaStateMachineExample.cs` rather than embedding everything in Player.
- BlockSpriteFactory creates ISprite instances using the existing brick artwork,
  source rectangles and color tint, following the singleton sprite-factory example.
- CrumblingPlatformLayout handles selection separately from object behavior.
  System.Random is used only for gameplay, as contemplated by the Sprint 2
  timer/counter/random state-change objective. CA5394 is disabled only in the
  two layout-randomization files, with an explanation in .editorconfig; no
  cryptographic randomness is needed for level variation.

Press F1 for the in-game color legend. No new sprite-sheet assets are required.

## Verification

Run from the repository root:

```text
dotnet run --project RegressionChecks -c Strict -- --crumbling
```

The checks cover state transitions and exact variant delays, repeated standing
contact, actual Player landing and falling, side contact, occupied restoration,
24 deterministic seeds per room, floor/spawn safety, within-run reproducibility,
and variation after starting new games. The existing room reachability suite
models the static room geometry; it does not prove every timed jumping route.
Visual rendering and a full playthrough still require manual verification.
