# Sprint 3 collision matrix and regression checklist

Updated October 19, 2026 for the integrated hanshil_tasks / sprint3-room-lifecycle result.

Evidence command: `dotnet run --project RegressionChecks/RegressionChecks.csproj --no-restore`.
Recorded result: 29 automated checks passed. A test checks selected code paths; it does not prove every gameplay scenario. Automated tests use lightweight sprite/entity doubles where graphics are unnecessary.

Status: **Automated PASS** means named assertions ran successfully. **Implemented / manual pending** means code exists but a complete graphical test has not been recorded. **Intentional ignore** describes a game rule, not missing coverage disguised as a pass.

| Pair | Expected response | Evidence / status |
|---|---|---|
| Player / solid block top | Land and set grounded | Automated PASS: landing |
| Player / solid block side | Stop at nearest edge | Automated PASS: horizontal wall crossing |
| Player / block underside | Stop rising at ceiling | Automated PASS: upward ceiling crossing for movement primitive; graphical jump pending |
| Player / room bounds | Clamp complete sprite within screen | Implemented in Game1; manual pending |
| Player / enemy | Apply health damage, knockback, one-second damaged decoration; shields prevent damage | Implemented; manual timing/knockback pending |
| Player / enemy projectile | Damage once and deactivate projectile | Automated PASS with player double; full shield/health interaction manual pending |
| Player / Medkit | Add once to stored inventory; key 3 heals 25 up to maximum | Integrated; manual pending |
| Player / Shield | Add once to inventory; key 4 grants five seconds protection | Integrated; manual pending |
| Player / Health Cell | Heal 10 immediately, capped at maximum; remove pickup | Automated PASS for single-application routing; actual health/UI manual pending |
| Player / Shield Battery | Add five seconds protection, capped at ten seconds | Implemented; manual pending |
| Player / Door | Request previous/next room; queue until collision pass ends; enter destination spawn | Automated PASS for parsing and overlap callback; graphical transition pending |
| Player / Hazard | Damage through non-solid red trigger, respecting central protection rules | Automated PASS for trigger routing; invulnerability manual pending |
| Player / own shot, piercing shot, orb, spread | No self damage | Intentional ignore; manual pending |
| Player / own bomb explosion | No self damage | Intentional ignore; manual pending |
| Ordinary/orb/angled/spread projectile / enemy | Damage enemy, deactivate projectile, ignore destroyed enemy | Automated PASS for swept ordinary shot; each weapon variant manual pending |
| Piercing projectile / enemy | Damage each distinct enemy once; stop after three enemies | Automated PASS for multi-hit and repeat prevention; third-hit limit manual pending |
| Player projectile / solid block | Stop at first wall even if an enemy appears earlier in list | Automated PASS: wall impact and nearest-wall ordering |
| Player projectile / crate | Destroy crate and stop projectile | Automated PASS: crate breaks on impact |
| Enemy projectile / solid block or crate | Stop shot; crate remains intact | Implemented; manual pending |
| Enemy projectile / enemy | No friendly fire | Intentional ignore; manual pending |
| Player projectile / enemy projectile | Pass through each other | Intentional ignore; manual pending |
| Projectile / item or door or hazard | No pickup or door activation; projectile continues | Intentional ignore; manual pending |
| Bomb before explosion / enemy or crate | No damage | Implemented fuse gate; manual pending |
| Bomb explosion / enemy | Damage each enemy once per bomb | Implemented HashSet guard; manual pending |
| Bomb explosion / crate | Break crate | Implemented; manual pending |
| Bomb explosion / other solid block | Leave indestructible block intact | Implemented type restriction; manual pending |
| Projectile / room boundary | Deactivate as soon as current bounds leave world | Automated PASS: immediate boundary cleanup |
| Ground enemy / solid block | Land, resolve sides/ceilings, avoid ledges | Shared physics checked; all enemy behaviors manual pending |
| Flying enemy / solid block or boundary | Resolve movement on each axis and remain in world | Implemented; manual pending |
| Enemy / enemy | Allow overlap, no enemy body blocking | Intentional ignore; manual pending |
| Enemy / item, hazard, door | Does not collect or activate these player-only triggers | Intentional ignore; manual pending |
| Static item / block or item | No movement response | Intentional static placement |
| Static block / block, door / block | No dynamic response | Intentional static placement |
| Old room / new room objects | Never interact; clear old projectiles and use destination collections | Implemented lifecycle; repeated graphical switching pending |

## Reproducible manual run

Setup: build the game, start it, press Enter. Desert is first; right click opens Laboratory and left click returns. Use R and Enter to reset before each independent case. Use a debugger to position entities for hard-to-reach overlaps; record coordinates when doing so.

1. Run into both sides of each platform, jump into undersides, land on tops, and approach screen edges. Verify no penetration, shaking, or permanently stuck entities.
2. Walk into each of the four pickups. Check disappearance after one collection; damage with E before testing healing. Activate stored items with 3/4. Confirm shield duration/cap and no repeated damage while protected.
3. Fire Z/N, K, 1, 5, and 6 into each enemy and wall. Repeat with a wall between the shot and enemy. Verify nearest impact, projectile cleanup, destroyed enemy removal, and three-enemy piercing limit.
4. Drop bomb with 2 adjacent to an enemy and crate. Verify no fuse damage, one explosion hit per enemy, broken crate no longer solid or drawn, and indestructible blocks remain.
5. Let enemy shots cross player, walls, and crates. Verify player protection and projectile deactivation; enemy shots should not break crates.
6. Walk onto a red hazard. Confirm damage respects invulnerability/shields and reaching zero health freezes gameplay; R returns to a fresh start.
7. Walk into each metal door marker. Verify direction, destination spawn, no immediate bounce, preserved health/inventory, and cleared old projectiles.
8. Hold each mouse button for several seconds, then release and click again. Verify one transition per press. Switch at least 20 times and confirm no duplicate enemies, items, or projectiles.
9. Watch all seven enemy behaviors and test contacts with geometry. Record balance issues or corner/spawn problems, rather than marking the entire category passing from one example.

Teardown: reset or exit so the next case begins with fresh room state. A revisited room deliberately reloads its initial objects.

## Graphical evidence log

| Tester | Date / commit | Matrix pair or step | Expected | Actual | PASS/FAIL | Screenshot/video/issue |
|---|---|---|---|---|---|---|
| Pending human playthrough | | | | | NOT RUN | |

No graphical pass results have been invented. See Sprint3-Integration-Assessment.md for remaining course requirements and the AI-assisted review for source analysis.
