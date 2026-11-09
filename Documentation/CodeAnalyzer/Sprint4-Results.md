# Sprint 4 Analyzer Results

## November 8, 2026 — HUD update

I ran the analyzer on November 9 and found two issues in the November 8 HUD update.

- **CA2213 (1 error, Game1.cs):** I only needed the portrait texture when setting up the HUD, so I made it a local variable in LoadContent.
- **CA1822 (1 error, UI/PlayerHud.cs):** DrawIcon only needs the information passed to it. I moved it into HudIcon so it can draw icons on its own.

## November 9, 2026 — Later analyzer run

I ran the Strict build again and got seven errors.

- **CA1508 (2 errors, Game1.cs):** The analyzer thought the game state stayed the same. Pressing Tab can open or close the map, so I kept the checks and read the current state after handling input.
- **CA5394 (1 error, Core/Rooms/RoomFactory.cs):** I kept Random to give each room a starting seed.
- **CA5394 (1 error, Core/Levelcreation/LevelBlockFactory.cs):** I kept Random to pick which crate is reinforced.
- **CA5394 (3 errors, Core/Levels/CrumblingPlatformLayout.cs):** I kept Random to choose which platforms crumble and where they go.

I turned off CA5394 only for those gameplay calls. They don't handle anything security-related. Using the same room seed means the layout can repeat when I come back to that room.

After these changes, the Strict build passed with 0 warnings and 0 errors.
