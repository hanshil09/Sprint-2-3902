# Sprint 4 Code Quality Changes
|Hanshil|
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
| `3902 proj/Core/PlayerInventory.cs`, `3902 proj/Entities/Player/Player.cs`, and `3902 proj/Entities/Player/PlayerShield.cs` | Inventory now handles pickup and item-use rules in one place. Health still handles healing and the shield still handles its timer. Player plays the item sound after successful use. |
| IPlayer.cs and Core/PlayerInventoryView.cs | The HUD reads item counts through one read-only inventory view. The player still handles item use. |
