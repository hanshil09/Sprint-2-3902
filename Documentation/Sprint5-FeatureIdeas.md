# Sprint 5 Feature Ideas and Research

## What the game already has

The project already has seven connected rooms, several enemy types and a boss,
multiple weapons and pickups, two player forms, moving and crumbling platforms,
supply crates, collisions, and room transitions. These ideas do not repeat
those finished features. They focus on known gaps and possible improvements
found in the code and current documentation.

## Ranked ideas

| Priority | Feature | Difficulty | Sprint 5 fit |
|---|---|---|---|
| 1 | Keep the player on rising platforms after a hit | Medium | Strong |
| 2 | Make shot recoil respect walls | Easy | Strong |
| 3 | Add downward aiming | Medium | Good, if time allows |
| 4 | Add pause and resume | Medium | Good alternative |
| 5 | Remember room changes on revisits | Hard | Better for a later sprint |

### 1. Keep the player on rising platforms after a hit

- **Description:** Fix the known collision bug where a projectile hit can make
  the player fall through a platform that is moving upward.
- **Why it helps:** Players should not lose health or progress because a moving
  platform and a hit happen at the same time.
- **C# files and systems:** [BlockCollision.cs](../3902%20proj/Collisions/BlockCollision.cs),
  [CollisionManager.cs](../3902%20proj/Collisions/CollisionManager.cs),
  [Player.cs](../3902%20proj/Entities/Player.cs), and the moving-platform collision
  logic.
- **Difficulty:** Medium.
- **Basic plan:** Reproduce the problem with the player standing on an elevator.
  Trace how damage applies knockback and changes the player's support platform.
  Adjust collision resolution so the player remains outside solid platform
  bounds while the elevator moves.
- **How to test:** Add a regression check that damages the player while an
  elevator is moving upward, advances several updates, and checks that the player
  does not overlap or fall through it. Also test the same situation in a manual
  playthrough with an enemy projectile.

### 2. Make shot recoil respect walls

- **Description:** Stop the small backward movement caused by a normal shot
  from pushing the player into a wall.
- **Why it helps:** The README lists this as a known bug. Preventing the overlap
  makes shooting near walls feel fair and keeps the player's position valid.
- **C# files and systems:** [PlayerArsenal.cs](../3902%20proj/Entities/PlayerArsenal.cs),
  [CollisionManager.cs](../3902%20proj/Collisions/CollisionManager.cs),
  [BlockCollisionHelper.cs](../3902%20proj/Collisions/BlockCollisionHelper.cs),
  and [ExtendedChecks.cs](../RegressionChecks/ExtendedChecks.cs).
- **Difficulty:** Easy.
- **Basic plan:** Route the recoil distance through the existing horizontal
  collision movement instead of changing the player's X position directly.
  Keep the current recoil amount when there is room to move.
- **How to test:** Add checks for firing beside a wall in both directions. The
  player should never overlap the wall, and should still recoil by the usual
  amount when no wall is nearby. Manually fire against walls in both player
  forms.

### 3. Add downward aiming

- **Description:** Let the player shoot down when pressing the down control.
  Down currently changes which way the player faces, while normal shots travel
  horizontally. The K key already provides an angled shot, so this idea adds a
  missing direction rather than repeating that feature.
- **Why it helps:** Players could attack enemies below them and use the existing
  facing control for a more useful action.
- **C# files and systems:** [ControllerFactory.cs](../3902%20proj/Controllers/ControllerFactory.cs),
  [Player.cs](../3902%20proj/Entities/Player.cs),
  [PlayerArsenal.cs](../3902%20proj/Entities/PlayerArsenal.cs),
  [EnergyProjectile.cs](../3902%20proj/Entities/EnergyProjectile.cs), and the
  controls help in [ControlsOverlay.cs](../3902%20proj/UI/ControlsOverlay.cs).
- **Difficulty:** Medium.
- **Basic plan:** Decide how down plus shoot selects a downward shot without
  breaking existing left/right and K-key shots. Set the projectile's starting
  position and direction, then update the controls help.
- **How to test:** Add checks that downward shots move toward the bottom of the
  screen and can hit a target below the player. Confirm existing horizontal and
  angled shots still travel in their current directions. Try it in a room with
  enemies above and below the player.

### 4. Add pause and resume

- **Description:** Add a pause screen that lets the player stop and resume the
  current game.
- **Why it helps:** Players can take a break without losing their current room
  or restarting the game.
- **C# files and systems:** [GameState.cs](../3902%20proj/Core/GameState.cs),
  [Game1.cs](../3902%20proj/Game1.cs),
  [ControllerFactory.cs](../3902%20proj/Controllers/ControllerFactory.cs),
  and [ControlsOverlay.cs](../3902%20proj/UI/ControlsOverlay.cs).
- **Difficulty:** Medium.
- **Basic plan:** Add a paused game state and a pause/resume input. Keep drawing
  the current room behind a simple pause message, but stop gameplay updates
  while paused. Do not treat the pause input as the existing quit action.
- **How to test:** Pause during a room with an enemy, moving platform, and active
  shield. Check that they and their timers stop while paused, that the room
  remains visible, and that everything continues after resuming.

### 5. Remember room changes on revisits

- **Description:** Keep selected changes when the player leaves and returns to
  a room, such as collected pickups, opened supply crates, or defeated enemies.
- **Why it helps:** Progress through the connected rooms would feel more
  permanent and would avoid repeating the same fights and pickups.
- **C# files and systems:** [RoomManager.cs](../3902%20proj/Core/RoomManager.cs),
  [RoomFactory.cs](../3902%20proj/Core/RoomFactory.cs),
  [Room.cs](../3902%20proj/Core/Room.cs),
  [Level.cs](../3902%20proj/Core/Level.cs), and the enemy, item, and crate systems.
- **Difficulty:** Hard.
- **Basic plan:** First agree on exactly which changes should persist. Store that
  state by room when leaving, and restore it when the room is built again. Keep
  the current reset behavior when starting a new game.
- **How to test:** Collect an item, open a supply crate, and defeat an enemy.
  Leave the room and return; check that each selected change is still present.
  Then reset the game and check that all rooms start fresh.

## Research and implementation notes

- MonoGame's official [game-loop guide](https://docs.monogame.net/articles/getting_to_know/whatis/game_loop/)
  describes `Update` as the place for game logic and `Draw` as the place for
  rendering. It also explains that the default loop is fixed-step and that
  elapsed time should be used for time-based movement when the update interval
  can vary. This project already uses elapsed time for player movement in
  `Player.Update` and platform movement in `MovingPlatform.Update`. New movement
  and pause behavior should follow the same update flow.
- MonoGame's [`GameTime` API reference](https://docs.monogame.net/api/Microsoft.Xna.Framework.GameTime.html)
  defines `ElapsedGameTime` as the time since the previous update. That is useful
  when checking movement, platform timing, and pause behavior: tests should
  check the result after game updates rather than assume a particular computer
  frame rate.
- The project already has headless checks for room rules, physics, projectiles,
  pickups, supply crates, and crumbling platforms. The two collision fixes can
  extend [ExtendedChecks.cs](../RegressionChecks/ExtendedChecks.cs); new
  direction and pause behavior should also get focused checks where practical.
  Automated checks are useful, but they do not replace manually playing through
  a room to check how the change feels.

## Sprint 5 recommendation

Start with **rising-platform collision** and **wall-safe recoil**. Both address
issues already recorded in the README, have clear pass/fail tests, and can be
kept to small changes. If the team has time for one player-facing addition,
choose **downward aiming**. Pause/resume is another reasonable option if the
team prefers a user-interface task. Room-state persistence is worthwhile, but
its larger scope makes it a weaker fit for a short sprint.
