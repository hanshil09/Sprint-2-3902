# Metroid Sprint 2

This MonoGame prototype demonstrates a player with two forms, platform movement, animated projectiles, block and item previews, and five enemies with distinct movement behaviors. Its input and platformer mechanics are themed after the classic NES Metroid.

## Repository

https://github.com/hanshil09/Sprint-2-3902

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
- **T/Y** cycle through four block previews. **U/I** cycle through collectible item previews. **O/P** cycle through five animated enemy types.
- **R** resets the game to the menu. **Q** or **Escape** quits.

## Implemented behavior

- The player can move, jump on platforms, face horizontal directions and down, switch forms, and animate. Damaged and temporary shooting states alter the player's sprite and available actions.
- Robot shots and orbs travel horizontally and expire after a time limit. Bombs show a timed explosion and then expire.
- The level draws a tiled platform layout, and the block showcase cycles through four sprite variants.
- The flyer patrols and swoops, the crawler stalks the player, the hopper leaps, the beetle charges, and the waver follows a drifting flight path.
- Ground enemies use gravity and avoid platform edges. Flying enemies collide with solid blocks and remain inside the game window.

## Known gaps

- Enemy destruction cannot currently be triggered during play because Sprint 2 does not yet connect player projectiles to enemy damage.
- Shots travel horizontally regardless of the player's vertical facing direction.
- The project has not been verified against the grader-approved Transformers feature list or through a full interactive playthrough.

## Development notes

- The game targets .NET 9 and uses MonoGame DesktopGL. The project file restores its NuGet dependencies, and `Content/Content.mgcb` builds the sprite sheets.
- Keep task estimates and status current on the team's project board. Record code review feedback, code-quality measurements or analyzer results, and the sprint reflection as required by the course.

## Credits
Metroid SNES Enemy Sprites by ronny14
https://www.spriters-resource.com/custom_edited/metroidcustoms/asset/55700/

Samus Aran Metroid Sprites by Mister Mike
https://www.spriters-resource.com/nes/metroid/asset/1774/
