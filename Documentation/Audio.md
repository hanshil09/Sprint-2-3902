# Sprint 4 audio

The game includes an original 16-second looping sci-fi track and ten synthesized
effects: jump, weapon fire, transformation, pickup, damage, bomb explosion,
medkit/shield use, victory, defeat, and reinforced crate break. The WAV files are original project assets
generated with assistance from Codex, rather than copied game recordings.

Music starts when Enter starts gameplay, continues across rooms, and stops on
reset, victory, or defeat. Press M on any screen to change the music preference;
it persists when resetting. Sound effects remain audible while music is muted.
The HUD shows whether music is on or off. Leaving gameplay stops the loop before
playing the end-screen effect.

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

Manual checks: start and listen; press/hold M to verify a single toggle; shoot
and jump; transform into vehicle form and verify rejected shots stay silent;
drop a bomb and wait for the explosion; collect a pickup and use an inventory
item; change rooms; reset and restart with music muted. Verify the victory/death
effects and confirm a fresh build on Windows includes Content/Audio/*.xnb.
