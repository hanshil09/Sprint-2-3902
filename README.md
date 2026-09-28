# Metroid Sprint 2

This MonoGame prototype demonstrates a player with two different forms, platform movement, sprite animation, basic projectiles, block previews, and the ability to cycle through idle animated enemies. Its input and platformer mechanics are themed after the classic NES Metroid.

## Run

From the repository root, run:

```text
dotnet run --project "3902 proj/3902 proj.csproj"
```

## Controls

- **Enter** starts from the menu.
- **A/D** or **Left/Right** move horizontally. **W**, **Up**, or **J** jumps. **S** or **Down** changes the player's facing direction downward; it does not move the player down.
- **Space** changes between standing and morph ball forms, where morph ball is smaller and moves faster.
- **Z/N** fire a shot in standing form. **1** fires an orb in robot form, and **2** drops a bomb in either form. The number pad's **1/2** keys also work.
- **E** briefly shows the damaged effect.
- **T/Y** cycle through four block previews. **O/P** cycle through five animated enemy types. **U/I** are mapped, but there are currently no items in their display list.
- **R** resets the game to the menu. **Q** or **Escape** quits.

## Implemented behavior

- The player can move, jump on platforms, face horizontal directions and down, switch forms, and animate. Damaged and temporary shooting states alter the player's sprite and available actions.
- Robot shots and orbs travel horizontally and expire after a time limit. Bombs show a timed explosion and then expire.
- The level draws a tiled platform layout, and the block showcase cycles through four sprite variants.
- The five enemy previews use different sprite animations, patrol speeds, patrol ranges, turn timers, and hop timers. Each active enemy changes between left-walking and right-walking states, and an enemy destroyed state is ready for later combat integration.

## Known gaps

- U/I does not display any items. No concrete item class is implemented.
- The enemy destroyed state cannot currently be triggered through gameplay because Sprint 2 does not yet connect enemies to combat or collision handling.
- Enemy destruction cannot currently be triggered during play. There is no enemy projectile system or player health system.
- **Up-facing artwork is not reachable through the current controls**, and shots travel horizontally regardless of facing direction.
- The start prompt appears in the window title; the menu does not draw text inside the game window.
- The project has not been verified against the grader-approved Transformers feature list or through a full interactive playthrough.

## Development notes

- The game targets .NET 9 and uses MonoGame DesktopGL. The project file restores its NuGet dependencies, and `Content/Content.mgcb` builds the sprite sheets.
- Keep task estimates and status current on the team's project board. Record code review feedback, code-quality measurements or analyzer results, and the sprint reflection as required by the course.

## Credits
Metroid SNES Enemy Sprites by ronny14
https://www.spriters-resource.com/custom_edited/metroidcustoms/asset/55700/

Samus Aran Metroid Sprites by Mister Mike
https://www.spriters-resource.com/nes/metroid/asset/1774/
