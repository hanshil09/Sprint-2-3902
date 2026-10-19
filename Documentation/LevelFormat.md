# Room file format

Rooms are UTF-8 text files in `3902 proj/Content/Levels`. They are copied to the output by the project file and read relative to the application directory.

Begin with `name: Room name`, then `[tiles]` and rows of symbols. `#` starts a whole-line comment. Each tile is 32 pixels. X increases rightward, Y downward. The grid bottom aligns to screen Y=540.

| Symbol | Object |
|---|---|
| . | Empty |
| P | Player spawn, exactly one required |
| G / B / S / M | Ground / Brick / Stone / Metal solid block |
| + / * | Medkit / Shield |
| f / c / h / b / w | Flyer / Crawler / Hopper / Beetle / Waver |
| t / v | Burst Sentry / vertical Sentinel |
| ! / $ | Immediate Health Cell / Shield Battery |
| C / X | Breakable crate / non-solid damage hazard |
| < / > | Previous-room / next-room door trigger |

Rooms should fit the 30-column, 17-row viewport. The parser warns about larger rooms because no scrolling camera exists. Short rows are padded with empty cells. Unknown tile symbols are warned about and treated as empty. Missing files or invalid spawn counts report diagnostics and use a built-in fallback room.

The ordered RoomPaths collection in Game1 contains six rooms, each with two enemies and a distinct layout. Each room has one gold door, 48 x 64 pixels, at the top of its parkour route. No return doors are placed. Left click/Page Up selects the previous room; right click/Page Down selects the next. Navigation wraps. Revisiting a room reloads its initial state. Old projectiles are cleared and the persistent player receives the destination block list, projectile manager, and spawn. See RoomGuide.md for the route.
