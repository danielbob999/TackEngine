using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Main;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Source.Audio;

namespace TackEngine.Core.Audio {
    internal abstract class AudioManager {
        public static AudioManager Instance { get; protected set; }

        private float m_masterVolume = 1f;
        private int m_nextClipId = 0;

        public float MasterVolume {
            get { return m_masterVolume; }
            set {
                if (value >= 0f) {
                    m_masterVolume = value;
                } else {
                    TackConsole.EngineLog(TackConsole.LogType.Error, "Cannot set AudioManager volume to a negative number. Valid range is: 0 - float.Infinity");
                }
            }
        }

        public AudioManager() {

        }

        public int GetNextClipId() {
            return m_nextClipId++;
        }

        internal abstract void OnStart();
        internal abstract void OnUpdate();
        internal abstract void OnClose();

        internal abstract void CreateAudioSource(AudioComponent component);
        internal abstract void PlayAudioSource(AudioComponent component);
        internal abstract void PauseAudioSource(AudioComponent component);
        internal abstract void StopAudioSource(AudioComponent component);
        internal abstract void DeleteAudioSource(AudioComponent component);
        internal abstract void SetAudioSourceClip(AudioComponent component, AudioClip clip);

        internal abstract void CreateAudioClip(AudioClip clip);
        internal abstract void DeleteAudioClip(AudioClip clip);

        internal abstract AudioClip LoadWavFile(string path);
        internal abstract AudioClip LoadOggFile(string path);
    }
}
