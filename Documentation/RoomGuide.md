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

- **Gold door:** next room, reached by the parkour route.
- **Cyan door:** previous room, near the entrance.
- Doors are 48 x 64 pixels and activate on player contact. Each destination uses its safe player spawn, away from the return door, preventing an immediate bounce.
- A/D or arrows move; W/Up/J jumps. Z/N shoots crates; 2 drops a bomb. The crate-route check assumes blocking crates have been destroyed first.
- F1 toggles full controls. The default HUD shows the current room and health without covering the platforms.
- Mouse clicks and Page Up/Page Down still jump between rooms for testing. Both ends wrap.
- Revisiting a room restores its initial enemies, pickups, and crates; health and inventory persist. Projectiles from the previous room clear.

## Room gravity and bombs — October 19, 2026

Player damage removes 3 health per accepted hit (shields still prevent damage). Using an inventory medkit restores health to 100; it is not consumed when health is already full. Health Cell pickups remain a separate +10 item.

Gravity is measured in pixels/second squared: Ground 1500, Brick 1450, Stone 1350, Metal 1200, Crate 1400, Hazard 1550. Lower values give slower falls and higher jumps. Each room applies its setting to the player, grounded enemies, and newly dropped bombs; flying enemies retain their flight behavior. Switching rooms resets player vertical velocity and applies the destination gravity.

Press **2** to drop a bomb. It falls onto solid platforms or the shared base-floor blocks, rests without bouncing, and falls again if its supporting crate breaks. Non-solid hazards do not support it. The fuse remains 1.2 seconds; the explosion stays in place for 0.32 seconds and damages each enemy at most once. Old-room bombs clear on room changes.

Verification: 91 automated checks pass, including bomb gravity, floor/platform support, destroyed supports, fuse/explosion lifecycle, invalid gravity rejection, and six geometry routes simulated with each room's actual gravity, 68 x 56 robot, movement speed 180, jump impulse 650, and 60 Hz steps. Strict build: zero warnings and errors. Route checks exclude enemies and do not prove combat balance or replace an interactive playthrough.
