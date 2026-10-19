# Six-room parkour route

Each room contains exactly two enemies and one pickup. The shared ground floor provides a safe starting surface; themed blocks are separated into different rooms. The old floating object previews no longer appear in gameplay.

| Order | Room | Main structure | Enemies |
|---|---|---|---|
| 1 | Ground Courtyard | Broad ground platforms; gentle jumps | Crawler, Flyer |
| 2 | Brick Stairway | Rising brick stairs and a high exit | Hopper, Beetle |
| 3 | Stone Switchbacks | Climb right, jump left, then cross right | Waver, Crawler |
| 4 | Metal Towers | Narrow metal platforms with smaller rises | Sentry, Sentinel |
| 5 | Crate Workshop | Ground platforms with shootable crate obstacles | Hopper, Beetle |
| 6 | Hazard Crossing | Elevated platforms above red floor hazards | Crawler, Flyer |

- **One gold door per room:** at the top of the parkour route, leading to the next room. No ground-level return doors are placed.
- Doors are 48 x 64 pixels and activate on player contact. Each destination uses its safe player spawn away from the elevated exit.
- A/D or arrows move; W/Up/J jumps. Z/N shoots crates; 2 drops a bomb. The crate-route check assumes blocking crates have been destroyed first.
- F1 toggles full controls. The default HUD shows the current room and health without covering the platforms.
- Mouse clicks and Page Up/Page Down still jump between rooms for testing. Both ends wrap.
- Revisiting a room restores its initial enemies, pickups, and crates; health and inventory persist. Projectiles from the previous room clear.

Verification: 60 automated checks pass, including six geometry routes simulated with the actual physics resolver, 68 x 56 robot, movement speed 180, jump impulse 650, and 60 Hz steps. Route checks exclude enemies and do not prove combat balance or replace an interactive playthrough.
