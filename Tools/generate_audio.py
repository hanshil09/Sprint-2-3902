"""Rebuild the game's original synthesized audio (Python standard library only).

Run from any directory: python3 Tools/generate_audio.py
Assets are original project audio, not recordings from Transformers or other games.
"""
from pathlib import Path
import array
import math
import random
import sys
import wave

RATE = 22050
DEST = Path(__file__).resolve().parents[1] / '3902 proj/Content/Audio'


def write(name, samples):
    pcm = array.array('h', (int(max(-1, min(1, s)) * 32767) for s in samples))
    if sys.byteorder != 'little':
        pcm.byteswap()
    with wave.open(str(DEST / (name + '.wav')), 'wb') as output:
        output.setparams((1, 2, RATE, 0, 'NONE', 'not compressed'))
        output.writeframes(pcm.tobytes())


def tone(duration, start, end, volume=0.5, noise=0):
    rng = random.Random(3902)
    count = int(duration * RATE)
    phase = 0
    result = []
    for i in range(count):
        t = i / RATE
        phase += 2 * math.pi * (start + (end - start) * t / duration) / RATE
        envelope = min(1, t / 0.008) * (1 - i / count) ** 1.5
        signal = math.sin(phase) + 0.2 * math.sin(phase * 2)
        result.append(volume * envelope * ((1 - noise) * signal + noise * rng.uniform(-1, 1)))
    return result


def melody(notes, beat):
    return [sample for note in notes for sample in tone(beat, note, note, 0.4)]


def shutter():
    """Two dry mechanical transients: shutter release, then curtain return."""
    rng = random.Random(3902)
    samples = [0.0] * int(0.12 * RATE)
    for offset, strength, frequency in [(0.0, 0.64, 920), (0.041, 0.8, 1650)]:
        start = int(offset * RATE)
        previous = 0.0
        for i in range(int(0.035 * RATE)):
            t = i / RATE
            noise = rng.uniform(-1, 1)
            attack = min(1, t / 0.001)
            click = (noise - 0.55 * previous) * math.exp(-t / 0.003)
            mechanism = math.sin(2 * math.pi * frequency * t) * math.exp(-t / 0.006)
            samples[start + i] = attack * strength * (0.66 * click + 0.34 * mechanism)
            previous = noise
    return samples


def main():
    DEST.mkdir(parents=True, exist_ok=True)
    effects = {
        'Jump': tone(0.18, 250, 800),
        'Shoot': tone(0.12, 1400, 140, noise=0.12),
        'Transform': tone(0.35, 120, 900, noise=0.3),
        'Pickup': melody([660, 880, 1320], 0.08),
        'Hurt': tone(0.22, 180, 55, noise=0.4),
        'Explosion': tone(0.65, 100, 25, noise=0.75),
        'CrateBreak': tone(0.23, 390, 68, volume=0.7, noise=0.65),
        'Shutter': shutter(),
        'Item': melody([440, 660, 880], 0.12),
        'Victory': melody([523.25, 659.25, 783.99, 1046.5, 783.99, 1046.5], 0.18),
        'Defeat': melody([440, 349.23, 293.66, 220], 0.25),
    }
    for name, samples in effects.items():
        write(name, samples)

    # Eight bars at 120 BPM. Each layer decays to zero at step boundaries,
    # including the loop seam, so the WAV loops without a discontinuity.
    beat = 0.25
    roots = [110, 110, 87.307, 97.999, 110, 130.813, 97.999, 82.407]
    pattern = [0, 7, 12, 7, 3, 7, 15, 12]
    track = [0.0] * int(16 * RATE)
    for step in range(64):
        root = roots[step // 8]
        lead = root * 2 ** (pattern[step % 8] / 12) * 2
        layers = [tone(beat, lead, lead, 0.16), tone(beat, root, root, 0.22)]
        if step % 2 == 0:
            layers.append(tone(0.15, 110, 40, 0.35))
        if step % 4 == 2:
            layers.append(tone(0.1, 180, 100, 0.16, 0.85))
        offset = int(step * beat * RATE)
        for layer in layers:
            for i, value in enumerate(layer):
                track[offset + i] += value
    write('Background', track)
    print(f'Generated {len(effects) + 1} WAV assets in {DEST}')


if __name__ == '__main__':
    main()
