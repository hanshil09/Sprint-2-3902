# Sprint 3 readability and maintainability review

> Follow-up integration: all team branches have since been combined on hanshil_tasks. Pickup inventory, health, shield behavior, doors, crate destruction, hazards, additional enemies/weapons, and nearest-first projectile impacts were added. Earlier findings below are historical. Current requirements and remaining work are tracked in Sprint3-Integration-Assessment.md. This is an AI-authored review, not an assertion that teammates performed it.

- Reviewer: Codex (AI-assisted review requested by Hanshil)
- Date: October 19, 2026
- Sprint: 3
- Original authors: Aarav, Levels branch; Sami, collision branch (attribution supplied by Hanshil)
- Additional source: existing sprint3-collision-fixes branch
- Reviewed files: Level.cs, LevelParser.cs, LevelLoader.cs, Game1.cs, CollisionManager.cs, Physics.cs, Room.cs, RoomManager.cs, MouseController.cs
- Duration: not recorded; this document does not claim a timed human review.

## Readability

The parser separates file reading, parsing, and object construction. Named tile dictionaries and line-numbered diagnostics make malformed files understandable. RoomManager keeps navigation separate from rendering and input. MouseController follows the existing command interface and tracks previous/current states clearly.

The original CollisionManager had empty response handlers, a position-only collision test, and a quadtree search that returned a single point candidate. It also was not called from the gameplay loop. A class named CollisionManager therefore implied functionality that the game did not execute. The integrated version now calls detection and uses rectangle bounds and swept projectile checks.

BuildTree is a legacy method name: the corrected implementation stores an object list and tests unordered pairs rather than building a tree. A future cleanup should rename it consistently across callers. Collision response dispatch is readable at the current size but should split into focused handlers if more object types are added.

## Findings and changes

| Severity | Finding | Resolution |
|---|---|---|
| High | Levels branch replaced the active-room lifecycle with a single level. | Preserve RoomManager and load each room through LevelLoader. |
| High | Room manager referenced hard-coded layouts after file parsing was introduced. | Both ordered rooms now have external .room files. |
| High | Only the selected enemy updated, drew, and entered the collision list. | Room now processes every file-defined enemy; destroyed enemies are ignored. |
| High | Central collision detection was never executed. | Call DetectCollision after the active-room object snapshot is built. |
| High | Original collision responses were empty. | Integrate existing rectangle/swept collision corrections, including side and ceiling physics. |
| Medium | Collected items remained in the cached object list until the next update. | Rebuild the snapshot from current block/item collections. |
| Medium | Projectile boundary cleanup considered its old swept path and delayed removal. | Use current bounds for world-exit cleanup. |
| High | Bomb explosion was visual only. | Add explosion damage with a per-bomb set preventing repeated enemy damage. |

## Maintainability and hypothetical change

Suppose the dungeon gains a third room. Add its .room data and its path to Game1.RoomPaths; the room count follows the collection length, and both keyboard and mouse navigation reuse RoomManager's wraparound rules. No collision code should need modification. The ordered collection is currently compiled into Game1; a manifest would make adding rooms possible without rebuilding code.

Suppose a new hazard needs its own collision response. CollisionManager currently uses type-based dispatch, so that change requires editing the dispatcher and adding regression coverage. Separate response classes or a documented dispatch table would keep that growth manageable. Avoid adding empty handlers and treating their presence as completed functionality.

## Verification and limits

The integrated game builds with zero warnings and errors. RegressionChecks exercises both room files, invalid spawns, wall/ceiling/landing movement, swept projectile hits, block impact, boundary cleanup, and mouse hold/rebuild behavior. All 17 checks passed on October 19, 2026.

No interactive graphical playthrough was performed for this review. Automated checks do not establish complete gameplay correctness, and this AI review does not satisfy the requirement that each of six teammates personally review and be reviewed.

Remaining requirement gaps are listed in Sprint3-Integration-Assessment.md. In particular, no door objects, destructible blocks, health/death system, or pickup effects are supplied by these merged branches.
