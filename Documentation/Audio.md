# Sprint 4 audio

The game includes an original 16-second looping sci-fi track and ten synthesized
effects: jump, weapon fire, transformation, pickup, damage, bomb explosion,
medkit/shield use, victory, defeat, and reinforced crate break. The WAV files are original project assets
generated with assistance from Codex, rather than copied game recordings.

Music starts when Enter starts gameplay, continues across rooms, and stops on
reset, victory, or defeat. Press M on any screen to mute or unmute all game
audio. Muting silences music and any sound effects already playing; new effects
are skipped while muted. Pressing M again restores the previous volume. The
music loop keeps its place in gameplay, and the mute setting persists across
room changes and resets. The HUD shows whether audio is on or off. Leaving
gameplay stops the loop before requesting the end-screen effect.

Audio uses the course's SoundEffect / SoundEffectInstance approach. WAV assets
are built through Content.mgcb and loaded once by GameAudio. The looping music
instance is disposed by Game1; ContentManager owns the loaded effects.
Player events keep gameplay usable without initializing audio in headless checks.
Sounds fire on accepted actions, not every frame or rejected weapon input.
Bombs play their explosion sound when the fuse expires, rather than on placement.

To regenerate the included assets, run `python3 Tools/generate_audio.py` from the
repository root. Python is only needed for regeneration, not building or playing.
Replace a WAV with your preferred licensed audio under the same name to change
the sound without modifying gameplay code.

Manual checks: start and listen; press/hold M to verify a single toggle and
confirm that playing audio goes silent. While muted, shoot, jump, transform,
detonate a bomb, and collect a pickup. Change rooms and reset/restart to verify
the mute setting persists. Press M again and confirm music and new effects are
audible. Verify victory and defeat sounds respect the setting, and confirm a
fresh build on Windows includes Content/Audio/*.xnb.
