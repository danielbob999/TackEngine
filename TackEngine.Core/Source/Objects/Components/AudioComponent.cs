using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Audio;
using TackEngine.Core.Math;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Source.Audio;

namespace TackEngine.Core.Objects.Components {
    public class AudioComponent : TackComponent {
        public enum AudioComponentState {
            Playing,
            Paused,
            Stopped
        }

        private float m_volume = 1f;

        /// <summary>
        /// Gets/Sets whether the AudioClip should loop
        /// </summary>
        public bool LoopAudio { get; set; }

        /// <summary>
        /// Gets/Sets the volume of this AudioComponent. Valid range is [0 - 1]
        /// </summary>
        public float Volume {
            get { return m_volume; }
            set { m_volume = TackMath.Clamp(value, 0f, 1f); }
        }

        /// <summary>
        /// Gets/Sets the pitch of this AudioComponent
        /// </summary>
        public float Pitch { get; set; }

        /// <summary>
        /// Gets the current state of this AudioComponent
        /// </summary>
        public AudioComponentState State { get; internal set; }

        public int AudioClipId { get; internal set; }

        /// <summary>
        /// Creates a new AudioComponent
        /// </summary>
        public AudioComponent() : base() {
            Volume = 1.0f;
            LoopAudio = false;
            Pitch = 1.0f;
            State = AudioComponentState.Stopped;
        }

        /// <summary>
        /// Creates a new AudioComponent
        /// </summary>
        /// <param name="volume">The volume of this component. Range: 0 - 1</param>
        public AudioComponent(float volume) : base() {
            Volume = volume;
            LoopAudio = false;
            Pitch = 1f;
            State = AudioComponentState.Stopped;
        }

        /// <summary>
        /// Creates a new AudioComponent
        /// </summary>
        /// <param name="volume">The volume of this component. Range: 0 - 1</param>
        /// <param name="pitch">The pitch of this component</param>
        public AudioComponent(float volume, float pitch) : base() {
            Volume = volume;
            LoopAudio = false;
            Pitch = pitch;
            State = AudioComponentState.Stopped;
        }

        /// <summary>
        /// Creates a new AudioComponent
        /// </summary>
        /// <param name="volume">The volume of this component. Range: 0 - 1</param>
        /// <param name="pitch">The pitch of this component</param>
        /// <param name="loopAudio">Should this component loop the AudioClip</param>
        public AudioComponent(float volume, float pitch, bool loopAudio) : base() {
            Volume = volume;
            LoopAudio = loopAudio;
            Pitch = pitch;
            State = AudioComponentState.Stopped;
        }

        public override void OnStart() {
            base.OnStart();
        }

        public override void OnUpdate() {
            base.OnUpdate();
        }

        public override void OnAttachedToTackObject() {
            base.OnAttachedToTackObject();

            AudioManager.Instance.CreateAudioSource(this);
        }

        public override void OnDetachedFromTackObject() {
            base.OnDetachedFromTackObject();

            AudioManager.Instance.DeleteAudioSource(this);
        }

        public override void OnClose() {
            base.OnClose();

            AudioManager.Instance.DeleteAudioSource(this);
        }

        /// <summary>
        /// Plays this AudioComponent
        /// </summary>
        public void Play() {
            AudioManager.Instance.PlayAudioSource(this);
        }

        /// <summary>
        /// Pauses the AudioComponent
        /// </summary>
        public void Pause() {
            AudioManager.Instance.PauseAudioSource(this);
        }

        /// <summary>
        /// Stops playing this AudioComponent
        /// </summary>
        public void Stop() {
            AudioManager.Instance.StopAudioSource(this);
        }

        /// <summary>
        /// Sets the audio clip for this source to play
        /// </summary>
        /// <param name="clip"></param>
        public void SetAudioClip(AudioClip clip) {
            if (clip == null) {
                return;
            }

            if (clip.Id == -1) {
                return;
            }

            AudioClipId = clip.Id;

            AudioManager.Instance.SetAudioSourceClip(this, clip);
        }
    }
}
