using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace TransformersGame.Core
{
    internal sealed class GameAudio : IDisposable
    {
        private readonly Dictionary<GameSound, SoundEffect> effects = new();
        private SoundEffectInstance music;
        private bool musicMuted;

        public bool MusicMuted => musicMuted;

        public void Load(ContentManager content)
        {
            foreach (GameSound sound in Enum.GetValues<GameSound>())
            {
                effects.Add(sound, content.Load<SoundEffect>($"Audio/{sound}"));
            }
            music = content.Load<SoundEffect>("Audio/Background").CreateInstance();
            music.IsLooped = true;
            music.Volume = 0.22f;
        }

        public void Play(GameSound sound) => effects[sound].Play(0.55f, 0f, 0f);

        public void StartMusic()
        {
            if (!musicMuted && music.State != SoundState.Playing)
            {
                if (music.State == SoundState.Paused) music.Resume();
                else music.Play();
            }
        }

        public void StopMusic() => music.Stop();

        public void ToggleMusic(bool playing)
        {
            musicMuted = !musicMuted;
            if (musicMuted) music.Pause();
            else if (playing) StartMusic();
        }

        public void Dispose()
        {
            music?.Dispose();
            music = null;
            // ContentManager owns the loaded SoundEffect assets.
        }
    }
}
