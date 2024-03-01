using TackEngine.Core.Main;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Source.Audio;
using Android.Media;
using Android.Content.Res;
using TackEngine.Core.Math;

namespace TackEngine.Android.Audio {
    internal class AndroidAudioManagerImpl : TackEngine.Core.Audio.AudioManager {
        private List<AndroidAudioSourceData> m_audioSources;

        public AndroidAudioManagerImpl() : base() {
            Instance = this;

            m_audioSources = new List<AndroidAudioSourceData>();
        }

        internal override void OnStart() {
        }

        internal override void OnUpdate() {
            for (int i = 0; i < m_audioSources.Count; i++) {

                m_audioSources[i].Component.State = GetStateFromMediaPlayer(m_audioSources[i].Player);

                m_audioSources[i].Player.Looping = m_audioSources[i].Component.LoopAudio;

                float volume = m_audioSources[i].Component.Volume * MasterVolume;

                m_audioSources[i].Player.SetVolume(volume, volume);
            }
        }

        internal override void OnClose() {
            // Release resources for any remaining MediaPlayers
            for (int i = 0; i < m_audioSources.Count; i++) {
                m_audioSources[i].Player.Release();
            }
        }

        internal override void CreateAudioSource(AudioComponent component) {
            if (m_audioSources.FindIndex(s => s.Component.Id == component.Id) != -1) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, "The AudioManager already contains data for the AudioComponent with Id: {0}", component.Id);
                return;
            }

            MediaPlayer player = new MediaPlayer();
            player.Looping = component.LoopAudio;

            m_audioSources.Add(new AndroidAudioSourceData(component, player));
        }

        internal override void DeleteAudioSource(AudioComponent component) {
            AndroidAudioSourceData? data = m_audioSources.Find(s => s.Component.Id == component.Id);

            if (data == null) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, "Cannot delete audio source because there is no data associated with AudioComponent with Id: {0}", component.Id);
                return;
            }

            if (data.Player.IsPlaying) {
                data.Player.Stop();
            }

            data.Player.Release();
            m_audioSources.Remove(data);
        }

        internal override void PlayAudioSource(AudioComponent component) {
            AndroidAudioSourceData? data = m_audioSources.Find(s => s.Component.Id == component.Id);

            if (data == null) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, "Cannot play audio source because there is no data associated with AudioComponent with Id: {0}", component.Id);
                return;
            }

            data.Player.Start();
        }

        internal override void PauseAudioSource(AudioComponent component) {
            AndroidAudioSourceData? data = m_audioSources.Find(s => s.Component.Id == component.Id);

            if (data == null) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, "Cannot pause audio source because there is no data associated with AudioComponent with Id: {0}", component.Id);
                return;
            }

            data.Player.Pause();
        }

        internal override void StopAudioSource(AudioComponent component) {
            AndroidAudioSourceData? data = m_audioSources.Find(s => s.Component.Id == component.Id);

            if (data == null) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, "Cannot stop audio source because there is no data associated with AudioComponent with Id: {0}", component.Id);
                return;
            }

            data.Player.Stop();
            data.Player.Prepare();
        }

        internal override void SetAudioSourceClip(AudioComponent component, AudioClip clip) {
            AndroidAudioSourceData? sourceData = m_audioSources.Find(s => s.Component.Id == component.Id);

            if (sourceData == null) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, "Cannot set audio clip of audio source as the given source is invalid");
                return;
            }

            AssetFileDescriptor afd = AndroidContext.CurrentAssetManager.OpenFd(System.Text.Encoding.ASCII.GetString(clip.Data));

            sourceData.Player.Reset();
            sourceData.Player.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
            sourceData.Player.Prepare();

            afd.Close();
        }

        internal override void CreateAudioClip(AudioClip clip) {
        }

        internal override void DeleteAudioClip(AudioClip clip) {
        }

        internal override AudioClip LoadWavFile(string path) {
            AudioClip newClip = new AudioClip(0, 0, 0, System.Text.Encoding.ASCII.GetBytes(path));
            return newClip;
        }

        internal override AudioClip LoadOggFile(string path) {
            AudioClip newClip = new AudioClip(0, 0, 0, System.Text.Encoding.ASCII.GetBytes(path));
            return newClip;
        }

        private AudioComponent.AudioComponentState GetStateFromMediaPlayer(MediaPlayer player) {
            if (player.IsPlaying) {
                return AudioComponent.AudioComponentState.Playing;
            }

            return AudioComponent.AudioComponentState.Stopped;
        }
    }
}
