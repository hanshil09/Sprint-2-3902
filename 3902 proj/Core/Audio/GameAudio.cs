using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace TransformersGame.Core
{
    internal sealed class GameAudio : IDisposable
    {
        private readonly Dictionary<GameSound, SoundEffect> effects = new();
        private SoundEffectInstance music;
        private SoundEffect shutter;
        private float unmutedVolume;

        public bool IsMuted { get; private set; }

        public void Load(ContentManager content)
        {
            foreach (GameSound sound in Enum.GetValues<GameSound>())
            {
                effects.Add(sound, content.Load<SoundEffect>($"Audio/{sound}"));
            }
            music = content.Load<SoundEffect>("Audio/Background").CreateInstance();
            music.IsLooped = true;
            music.Volume = 0.22f;

            // A short PCM WAV can be loaded directly without the audio content processor.
            using Stream shutterFile = TitleContainer.OpenStream(Path.Combine(content.RootDirectory, "Audio", "Shutter.wav"));
            shutter = SoundEffect.FromStream(shutterFile);
        }

        public void Play(GameSound sound)
        {
            if (!IsMuted)
            {
                effects[sound].Play(0.55f, 0f, 0f);
            }
        }

        public void PlayShutter()
        {
            if (!IsMuted) shutter.Play(0.65f, 0f, 0f);
        }

        public void StartMusic() => music.Play();

        public void StopMusic() => music.Stop();

        public void ToggleMute()
        {
            if (IsMuted)
            {
                SoundEffect.MasterVolume = unmutedVolume;
            }
            else
            {
                unmutedVolume = SoundEffect.MasterVolume;
                // The master volume also silences effects that are already playing.
                SoundEffect.MasterVolume = 0f;
            }
            IsMuted = !IsMuted;
        }

        public void Dispose()
        {
            music?.Dispose();
            music = null;
            shutter?.Dispose();
            shutter = null;
            if (IsMuted)
            {
                ToggleMute();
            }
            // ContentManager owns the loaded SoundEffect assets.
        }
    }
}
