# Sprint 3 Collision Matrix and Regression Checklist

> Historical baseline: the matrix below describes the pre-integration branch. Levels and collision corrections were subsequently merged on October 19. Its PASS/FAIL counts are not the current result. See [the integration assessment](Sprint3-Integration-Assessment.md) and [Sprint 3 review](CodeReviews/Sprint3-Levels-Collision-Review.md) for current implementation status and 17 passing automated checks. The unchecked interactive tests remain pending.

**Issue:** #11  
**Branch:** `hanshil_tasks`  
**Recorded:** October 19, 2026  
**Scope:** Collision behavior present on this branch after the room-lifecycle and mouse-room-navigation work

## Status definitions

- **PASS:** The implemented behavior and code path satisfy the expected result.
- **PARTIAL:** Part of the expected result works, but an effect or collision direction is missing.
- **FAIL:** The objects can overlap or interact, but the expected response is not implemented.
- **NOT IMPLEMENTED:** The required game-object type or gameplay path does not exist on this branch.
- **N/A:** The pair cannot occur in the current runtime configuration or intentionally has no response.

## System-level findings

1. `Game1.UpdateGameplay` rebuilds the collision tree but never calls `CollisionManager.DetectCollision`.
2. `CollisionManager.CheckCollision` compares object positions within two pixels instead of comparing sprite rectangles.
3. `CollisionManager.HandleCollision` contains empty branches for player/block, player/enemy projectile, player/enemy, and enemy/block interactions.
4. `Room.GetCollisionObjects` includes level objects, the player, the currently selected enemy, and active projectiles. It does not include every enemy stored by the cycler at the same time.
5. No `Door` class or door collision path exists on this branch. Mouse and Page Up/Page Down navigation switch rooms directly through `RoomManager`.

These findings mean the current collision manager cannot be treated as passing even where another class independently provides a working response.

## Collision matrix

| ID | Collision pair | Expected result | Current implementation and evidence | Status |
|---|---|---|---|---|
| CM-01 | Player vs solid block, falling onto top | Player lands on the block, vertical velocity becomes zero, and grounded state becomes true. | `Physics.Apply` calls `LandOnBlocks`; `IsLandingOn` checks horizontal overlap and crossing the block top. | PASS |
| CM-02 | Player vs solid block, left/right side | Player stops at the block edge and cannot pass through it. | `Player.MoveHorizontally` changes X without testing blocks. The collision-manager response is empty and never called. | FAIL |
| CM-03 | Player vs solid block, jumping into underside | Player stops below the block and begins falling. | `Physics.Apply` checks only downward movement and top-surface landings. | FAIL |
| CM-04 | Player vs screen boundary | Player remains fully inside the 960 x 540 viewport. | `Game1.KeepPlayerOnScreen` clamps the player position after movement. | PASS |
| CM-05 | Player vs medkit | Medkit is collected once, disappears, and applies its health effect. | `Level.CollectItems` removes an intersecting item, but `Medkit.Collect` is empty, so no health effect occurs. | PARTIAL |
| CM-06 | Player vs shield | Shield is collected once, disappears, and applies its shield effect. | `Level.CollectItems` removes an intersecting item, but `Shield.Collect` is empty, so no shield effect occurs. | PARTIAL |
| CM-07 | Player vs enemy | Player takes damage once, enters the damaged state, and receives knockback or invulnerability. | `CollisionManager.HandleCollision` has an empty player/enemy branch and detection is not called. | FAIL |
| CM-08 | Player vs enemy projectile | Player takes damage once and the projectile expires or is removed. | `CollisionManager.HandleCollision` has an empty player/enemy-projectile branch. `EnemyProjectile` only expires after its lifetime. | FAIL |
| CM-09 | Player energy projectile vs enemy | Enemy takes damage; a destroyed enemy enters its destroyed state; the projectile becomes inactive. | No response pair exists. `Enemy.TakeDamage` exists but no collision path calls it. | FAIL |
| CM-10 | Player energy projectile vs solid block | Projectile stops or becomes inactive on impact. | No projectile/block response exists. The projectile continues until its three-second lifetime ends. | FAIL |
| CM-11 | Bomb/explosion vs enemy | An enemy inside the explosion area takes damage once. | `Bomb` changes its sprite after the fuse but exposes no explosion bounds or damage collision response. | FAIL |
| CM-12 | Bomb/explosion vs solid block | Destructible blocks react; indestructible blocks remain unchanged. | No bomb/block response or destructible-block state exists. | FAIL |
| CM-13 | Ground enemy vs solid block | Enemy lands on top surfaces, does not walk through walls, and avoids platform edges. | Shared `Physics` supports landing and `HasGroundAhead` supports ledge avoidance. Horizontal `MoveBy` checks only screen bounds, not block sides. | PARTIAL |
| CM-14 | Flying enemy vs solid block | Enemy is repositioned outside the block and its behavior can react to being blocked. | `Enemy.FlyHorizontally` and `FlyVertically` use rectangle intersection and correct the position at the contacted edge. | PASS |
| CM-15 | Enemy vs screen boundary | Enemy remains fully inside the viewport. | `MoveBy`, `FlyHorizontally`, and `FlyVertically` clamp movement to screen dimensions. | PASS |
| CM-16 | Enemy vs enemy | Enemies do not overlap, or the design explicitly documents that overlap is allowed. | Only `Enemies.CurrentObject` is updated/drawn and added to the collision list, so multiple enemies cannot currently collide during normal play. | N/A |
| CM-17 | Enemy projectile vs solid block | Projectile becomes inactive when it hits a solid block. | No response pair exists; the projectile remains active until its lifetime ends. | FAIL |
| CM-18 | Projectile vs screen boundary | Projectile becomes inactive after leaving the room. | Both projectile types use a three-second lifetime rather than boundary collision. They may remain active off-screen until the timer expires. | PARTIAL |
| CM-19 | Player vs door | Crossing a door loads the connected room and places the player at the correct entrance. | No door object or door collision path exists. Rooms switch only through mouse or Page Up/Page Down input. | NOT IMPLEMENTED |
| CM-20 | Projectile vs item | Items ignore projectiles and remain available to collect. | No response exists, which matches the expected no-interaction rule. | PASS |
| CM-21 | Item vs block | Static item placement remains stable; no runtime response is required. | Both objects remain static. Items are removed only by player intersection. | PASS |

## Reproducible regression checklist

Use a clean run for each test unless the steps explicitly say otherwise. Record the tester, commit, and result in the evidence table below.

### Setup

1. Build the solution with `dotnet build "3902 proj.sln" --no-restore`.
2. Start the game with `dotnet run --project "3902 proj/3902 proj.csproj"`.
3. Press Enter to begin gameplay.
4. Use Page Up/Page Down or left/right mouse click to choose the room named by the test.
5. Press R between tests when a clean player, item, enemy, and projectile state is required.

### Tests

- [ ] **RT-01: Player landing.** Jump with W, Up, or J and land on each platform type. Expected: the player rests on the platform without sinking or vibrating. Covers CM-01.
- [ ] **RT-02: Player side collision.** Walk horizontally into the side of a raised platform for two seconds. Expected: the player stops at the edge. Current expected evidence: failure because horizontal block resolution is missing. Covers CM-02.
- [ ] **RT-03: Player underside collision.** Jump into the underside of a platform. Expected: upward movement stops at the underside. Current expected evidence: failure because underside handling is missing. Covers CM-03.
- [ ] **RT-04: Player world bounds.** Hold movement toward the left and right edges and use a jump near the top/bottom limits. Expected: the complete player sprite remains visible. Covers CM-04.
- [ ] **RT-05: Medkit pickup.** Walk through the medkit, leave the area, and return. Expected: it disappears after the first contact and does not reappear until reset; record that no health effect exists. Covers CM-05.
- [ ] **RT-06: Shield pickup.** Walk through the shield, leave the area, and return. Expected: it disappears after the first contact and does not reappear until reset; record that no shield effect exists. Covers CM-06.
- [ ] **RT-07: Player and enemy contact.** Move the active enemy into the player. Expected: one damage event and temporary damaged state. Current expected evidence: failure because the handler is empty. Covers CM-07.
- [ ] **RT-08: Enemy projectile and player.** Select an enemy that fires, wait for a shot to overlap the player, and observe player state and projectile lifetime. Expected: damage and projectile removal. Current expected evidence: failure. Covers CM-08.
- [ ] **RT-09: Player shot and enemy.** Fire Z or N through the active enemy. Expected: enemy damage/destruction and projectile removal. Current expected evidence: failure. Covers CM-09.
- [ ] **RT-10: Player shot and block.** Fire a normal and angled shot into solid platforms. Expected: each projectile stops on impact. Current expected evidence: failure. Covers CM-10.
- [ ] **RT-11: Bomb and enemy.** Move beside an enemy, press 2, and keep the enemy within the explosion sprite. Expected: enemy damage once. Current expected evidence: failure. Covers CM-11.
- [ ] **RT-12: Bomb and block.** Drop a bomb beside every block type. Expected: only designated destructible blocks change. Current expected evidence: failure because destructible-block behavior is absent. Covers CM-12.
- [ ] **RT-13: Ground enemy platforms.** Cycle through ground enemies with O/P and watch at least ten seconds near edges and raised blocks. Expected: landings and ledge avoidance pass; record any horizontal wall overlap. Covers CM-13.
- [ ] **RT-14: Flying enemy blocks.** Cycle through flying enemies and observe contact with platform sides and undersides. Expected: no penetration and a visible behavior response. Covers CM-14.
- [ ] **RT-15: Enemy world bounds.** Observe every enemy type for at least ten seconds near screen edges. Expected: the full sprite remains inside the viewport. Covers CM-15.
- [ ] **RT-16: Enemy projectile and block.** Place the player so an enemy fires through a platform. Expected: projectile stops at the platform. Current expected evidence: failure. Covers CM-17.
- [ ] **RT-17: Projectile world bounds.** Fire shots at both outer edges and time cleanup. Expected: immediate removal after leaving the room. Current expected evidence: partial because cleanup waits for the lifetime timer. Covers CM-18.
- [ ] **RT-18: Door transition.** Attempt to locate and cross a door in each room. Expected: connected-room transition and correct spawn. Current expected evidence: not implemented because there are no door objects. Covers CM-19.
- [ ] **RT-19: Projectile and item.** Fire every player projectile through both item types. Expected: items remain until player contact. Covers CM-20.
- [ ] **RT-20: Room-switch cleanup.** Fire several projectiles, switch rooms with both mouse buttons, and switch back repeatedly. Expected: old-room projectiles are cleared, player spawn changes, and no room-owned objects duplicate.

## Test evidence

| Test run | Date | Tester | Commit | Build result | Passed | Failed | Notes or evidence link |
|---|---|---|---|---|---:|---:|---|
| Code inspection baseline | 2026-10-19 | Hanshil | Working tree on `hanshil_tasks` | PASS: 0 warnings, 0 errors | 6 | 9 | CM-01 through CM-21 above; four partial, one N/A, and one not implemented are reported separately. |
| Interactive regression | YYYY-MM-DD | Name | Commit SHA | PASS/FAIL | 0 | 0 | Add screenshots, recording, or issue links here. |

## Exit criteria for a future all-green run

- `CollisionManager.DetectCollision` runs once per gameplay update after collision objects are collected.
- Collision detection uses object rectangles or another documented shape, including projectile and explosion dimensions.
- Every FAIL or PARTIAL row above has an implemented response and a passing regression result.
- Door objects and connected-room spawn behavior are either implemented and tested or explicitly removed from the game requirements with instructor approval.
- A teammate reruns RT-01 through RT-20 on the merge commit and records evidence in this file.
