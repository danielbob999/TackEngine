using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TackEngine.Core.Audio;
using TackEngine.Core.Main;

namespace TackEngine.Core.Source.Audio {
    public class AudioClip {

        /// <summary>
        /// The internal Id of this AudioClip
        /// </summary>
        public int Id { get; private set; } = -1;

        /// <summary>
        /// The number if channels this AudioClip has
        /// </summary>
        public int NumberOfChannels { get; private set; }

        /// <summary>
        /// The number if bits per sample this AudioClip has
        /// </summary>
        public int BitsPerSample { get; private set; }

        /// <summary>
        /// The number of samples this AudioClip has
        /// </summary>
        public int NumberOfSamples { get; private set; }

        public byte[] Data { get; private set; }

        internal AudioClip(int chNum, int bitsPerSam, int samNum, byte[] data) {
            NumberOfChannels = chNum;
            BitsPerSample = bitsPerSam;
            NumberOfSamples = samNum;
            Data = data;

            Id = AudioManager.Instance.GetNextClipId();

            AudioManager.Instance.CreateAudioClip(this);
        }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            if (obj.GetType() != typeof(AudioClip)) {
                return false;
            }

            return Id == ((AudioClip)obj).Id;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        /// <summary>
        /// Loads a wav file
        /// </summary>
        /// <param name="path">The path of the audio</param>
        /// <returns></returns>
        public static AudioClip LoadWavFromFile(string path) {
            return AudioManager.Instance.LoadWavFile(path);
        }

        /// <summary>
        /// Loads a ogg file
        /// </summary>
        /// <param name="path">The path of the audio</param>
        /// <returns></returns>
        public static AudioClip LoadOggFromFile(string path) {
            return AudioManager.Instance.LoadOggFile(path);
        }
    }
}
