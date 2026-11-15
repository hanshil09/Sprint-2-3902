# Sprint 4 Code Quality Changes

| File or folder | Why it improves code quality |
|---|---|
| `3902 proj/UI/PlayerHud.cs` | Keeps the counters together. Moving a counter also moves its divider, so there is one position to update. |
| `3902 proj/UI/HudFrame.cs` | Keeps the portrait and panel decoration together, so changing the frame is separate from changing player stats. |
| `3902 proj/UI/HudIcon.cs` | All counters share one icon drawing method. The text spacing follows the icon size. |
| `3902 proj/UI/HeartMeter.cs` | The player and boss share one heart renderer. Each can pass in its own color. |
| `3902 proj/Entities/Player/Player.cs` | Connects health, shields, movement, and sound. The HUD and controls still use the same player interface. |
| `3902 proj/Entities/Player/PlayerShield.cs` | Keeps shield activation and the timer in one place. |
| `3902 proj/Entities/Player/PlayerMovement.cs` | Groups walking, jumping, bouncing, and knockback. It uses the existing physics and collision code. |
| `3902 proj/Entities/Player/PlayerHealth.cs` | Groups damage and healing. Health cells, med kits, and upgrades share the same healing limit. |
| `3902 proj/Entities/` | Groups player, enemy, projectile, object, and item classes in folders so related files are easier to find. |
| `3902 proj/Entities/Player/PlayerForm.cs` | Groups form changes, sprite updates, and the room check for switching back to robot form. This keeps transformation rules easy to find. |
| `3902 proj/Core/Levelcreation/LevelSpawnSearch.cs` | Keeps the search for a clear starting spot in one place. Player and enemy placement share it, and the search only needs coordinates and occupied rectangles. |
| `3902 proj/Core/Levelcreation/LevelBlockFactory.cs` | Keeps crate selection and block creation together. The level can build the room without knowing the crate setup details. |
| `3902 proj/Core/Levels/Level.cs`, `Core/Rooms/Room.cs`, and `Interfaces/ILevel.cs` | Renames the level object list method to show that the room uses it for collision checks. |
| `3902 proj/Core/` | Groups room, level, enemy, and audio files so related code is easier to find. |
| `3902 proj/UI/PlayerHud.cs` and `3902 proj/Game1.cs` | Names the HUD counter positions, shows current and maximum health beside the hearts, and keeps the portrait texture variable where the HUD is created. |
| `3902 proj/Entities/Player/FireballSelection.cs` | Keeps each shot name and attack together. The map and HUD share the selected shot, and firing uses the existing player attacks. |
| `3902 proj/Entities/Projectiles/IceProjectile.cs` | Reuses normal projectile movement and collision. It adds only the special effect when Ice hits an enemy. |
| `3902 proj/Entities/Enemies/Enemy.cs` | Keeps the freeze timer with the enemy it affects. Existing attacks still use the same damage rules. |
| `3902 proj/Commands/GameActionCommand.cs` | Keeps the general action command in its own file, making it easier to find and update without changing other commands. |
| `3902 proj/Commands/ToggleMuteCommand.cs` | Separates the mute command so its behavior is easy to locate and maintain. |
| `3902 proj/Commands/ToggleHelpCommand.cs` | Separates the help command from unrelated game commands, making future updates easier to focus. |
| `3902 proj/Commands/BarrelStormCommand.cs` | Keeps the Barrel Storm command in its own file so its connection to the ability is easy to find. |
| `3902 proj/Commands/StartGameCommand.cs` | Separates the start-game command, making it easier to read and update without touching other commands. |
| `3902 proj/Commands/ResetGameCommand.cs` | Separates the reset command so its behavior is easier to find and maintain. |
| `3902 proj/Commands/QuitCommand.cs` | Gives the quit command its own file, keeping its small responsibility separate from other commands. |
| `3902 proj/Commands/DamagePlayerCommand.cs` | Separates the damage command so it can be found and changed without editing unrelated command code. |
| `3902 proj/Commands/RoomNavigationCommand.cs` | Keeps room navigation in its own file, making next/previous room behavior easier to find and update. |
| `3902 proj/Commands/GameCommands.cs` | Removed the combined file after moving its nine command classes into their own files. This follows the one-class-per-file guideline while preserving command behavior. |
| `3902 proj/Commands/MovePlayerCommand.cs` | Keeps movement command behavior in its own file, making it easier to locate, understand, and modify independently. |
| `3902 proj/Commands/JumpPlayerCommand.cs` | Keeps jump command behavior separate, so it can be understood and changed without searching through other player commands. |
| `3902 proj/Commands/UseItemCommand.cs` | Separates weapon/item activation from other commands, making its role easier to find and update independently. |
| `3902 proj/Commands/UseInventoryItemCommand.cs` | Keeps medkit and shield command handling in one focused file, making inventory command changes easier to review. |
| `3902 proj/Commands/TransformPlayerCommand.cs` | Separates the transform command so its existing player-form behavior is easier to locate and maintain. |
| `3902 proj/Commands/PlayerCommands.cs` | Removed the combined file after moving its five player command classes into separate files, following the one-class-per-file guideline. |
| `3902 proj/Core/GameObjectCycler.cs` and `3902 proj/Interfaces/IGameObjectCycler.cs` | Removed the unused `Draw` method and its interface declaration; the showcase already draws entries directly. Also removed a redundant count check after the empty-list case. The cycler and its navigation commands remain because rooms use it for enemies and the T/Y, U/I, and O/P showcase controls. |
| `3902 proj/Collisions/CollisionManager.cs` | Split `DetectCollision()` into helpers for projectile impacts, object-pair collisions, and out-of-bounds projectile cleanup. Keeping these steps separate makes collision processing easier to read, debug, and maintain while preserving their existing order. |
| `3902 proj/Collisions/BlockCollisionOperations.cs` | Renamed `BlockCollisionHelper` to show that it contains stateless block-collision operations. The similarly named movement methods in `BlockCollision` delegate to these algorithms, so they were not duplicate implementations and were kept unchanged. |
