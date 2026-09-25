# Transformers Sprint 2

## What has been added

The original project contained the basic MonoGame template. The starter code now provides a runnable structure that allows different team members to work on separate features without putting everything inside `Game1.cs`.

### Main game setup

`Game1.cs` now handles the high-level game flow:

- Starts in a simple menu state.
- Changes to the gameplay state when the player presses Enter.
- Creates the player and keyboard controller.
- Updates and draws the player during gameplay.
- Keeps the player inside the game window.
- Provides methods for starting and resetting the game.

The game window title displays the controls because the project does not have a font asset yet.

### Interfaces

The `Interfaces` folder contains shared contracts for the major game object categories:

- `IGameObject`: an object with a position that can update and draw.
- `IPlayer`: player movement, transformation, damage, and reset behavior.
- `IEnemy`: an enemy game object that can take damage.
- `IItem`: a collectible game object.
- `IBlock`: a stationary or solid room object.
- `IProjectile`: a projectile with an active state.
- `ISprite`: drawing and animation behavior.
- `IController`: a class that processes input.
- `ICommand`: an action invoked by a controller.

These interfaces are foundations. Team members should create concrete classes that implement them.

### Player

`Entities/Player.cs` provides the first working player implementation. The player:

- Moves in four directions.
- Remembers the direction it is facing.
- Changes between robot and vehicle forms.
- Moves faster in vehicle form.
- Briefly changes color after taking damage.
- Can reset to its starting position and robot form.

The player's gameplay behavior is separate from its sprite and drawing code, following the Sprint 2 requirements.

### Keyboard controller and commands

`Controllers/KeyboardController.cs` reads keyboard input. It uses command objects instead of directly changing the player or game.

The starter includes these commands:

- `MovePlayerCommand`
- `TransformPlayerCommand`
- `DamagePlayerCommand`
- `StartGameCommand`
- `ResetGameCommand`
- `QuitCommand`

This follows the Command design pattern demonstrated in the course examples. New controls should normally be added by creating a command and registering it in the controller.

### Sprite system

`ISprite` represents an object responsible for drawing and animation. `SolidColorSprite` currently draws a simple colored rectangle so the program works before the team has final artwork.

`PlaceholderSpriteFactory` creates temporary sprites for:

- Robot form
- Vehicle form
- Four facing directions
- The damaged appearance

The sprite and animation teammate can later replace these placeholders with sprite-sheet implementations without rewriting the player's movement behavior.

## Controls

- Enter: start gameplay
- Arrow keys or WASD: move and face a direction
- Space: transform between robot and vehicle forms
- E: show the damaged state briefly
- R: reset to the start state
- Q or Escape: quit

Colored rectangles are temporary sprites. Replace `PlaceholderSpriteFactory` with factories that create animated sprites from sprite sheets.

## Suggested team split

1. Player behavior and transformations
2. Player sprites and animation
3. Enemies and enemy factory
4. Items and projectiles
5. Blocks, obstacles, and room objects
6. Controllers, commands, game states, integration, and documentation

Player work is split between two people because the assignment identifies it as the largest feature area.

## Folder responsibilities

- `Interfaces`: shared contracts; discuss changes with the team before editing
- `Controllers`: reads input and invokes commands
- `Commands`: small actions that operate on game objects
- `Entities`: gameplay behavior and state
- `Sprites`: drawing and animation only
- `Factories`: creates sprites and later other object families
- `Core`: shared enums and game-level state

## Next steps

- Add real sprite sheets to `Content/Content.mgcb`.
- Create animated robot and vehicle sprites implementing `ISprite`.
- Add concrete classes implementing `IEnemy`, `IItem`, `IBlock`, and `IProjectile`.
- Add cycle controls for blocks, items, and enemies.
- Add timer-driven or random state changes for enemies.
- Record controls, known bugs, code reviews, and analyzer results.

Enemies, items, blocks, projectiles, real animation, and final artwork have not been implemented yet. The current code is the shared foundation for those features.
