using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using TackEngine.Core.Audio;
using TackEngine.Core.Main;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Source.Audio;

namespace TackEngine.Desktop.Audio {
    internal class DesktopAudioManagerImpl : AudioManager {
        private static readonly float CONVERT_TO_SC16 = 0x7FFF + 0.4999999999999999f;

        private List<DesktopAudioSourceData> m_audioSources;
        private List<DesktopAudioClipData> m_audioClips;

        private ALDevice m_audioDevice;
        private ALContext m_audioContext;

        public DesktopAudioManagerImpl() : base() {
            Instance = this;

            m_audioSources = new List<DesktopAudioSourceData>();
            m_audioClips = new List<DesktopAudioClipData>();
        }

        internal override void OnStart() {
            // Initialize the OpenAL device and make the context current
            m_audioDevice = ALC.OpenDevice(null);
            m_audioContext = ALC.CreateContext(m_audioDevice, new ALContextAttributes());

            ALC.MakeContextCurrent(m_audioContext);

            TackConsole.EngineLog(TackConsole.LogType.Message, "Audio Vendor: " + AL.Get(ALGetString.Vendor));
            TackConsole.EngineLog(TackConsole.LogType.Message, "Audio Renderer: " + AL.Get(ALGetString.Renderer));
            TackConsole.EngineLog(TackConsole.LogType.Message, "Audio Version: " + AL.Get(ALGetString.Version));
        }

        internal override void OnUpdate() {
            for (int i = 0; i < m_audioSources.Count; i++) {
                AL.Source(m_audioSources[i].OpenALSourceId, ALSourceb.Looping, m_audioSources[i].Component.LoopAudio);
                AL.Source(m_audioSources[i].OpenALSourceId, ALSourcef.Gain, (m_audioSources[i].Component.Volume * AudioManager.Instance.MasterVolume));
                AL.Source(m_audioSources[i].OpenALSourceId, ALSourcef.Pitch, m_audioSources[i].Component.Pitch);

                ALSourceState alState = AL.GetSourceState(m_audioSources[i].OpenALSourceId);

                m_audioSources[i].Component.State = GetStateFromALState(alState);
            }
        }

        internal override void OnClose() {
            foreach (DesktopAudioClipData clip in m_audioClips) {
                AL.DeleteBuffer(clip.OpenALBufferId);
                clip.DataHandle.Free();
            }
        }

        internal override void CreateAudioSource(AudioComponent component) {
            if (m_audioSources.FindIndex(s => s.Component.Id == component.Id) != -1) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, "The AudioManager already contains data for the AudioComponent with Id: {0}", component.Id);
                return;
            }

            m_audioSources.Add(new DesktopAudioSourceData(component, AL.GenSource()));
        }

        internal override void DeleteAudioSource(AudioComponent component) {
            if (m_audioSources.FindIndex(s => s.Component.Id == component.Id) == -1) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, "Cannot delete audio source because there is no data associated with AudioComponent with Id: {0}", component.Id);
                return;
            }

            DesktopAudioSourceData data = m_audioSources.Find(s => s.Component.Id == component.Id);

            AL.DeleteSource(data.OpenALSourceId);
            m_audioSources.Remove(data);
        }

        internal override void PlayAudioSource(AudioComponent component) {
            if (m_audioSources.FindIndex(s => s.Component.Id == component.Id) == -1) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, "Cannot play audio source because there is no data associated with AudioComponent with Id: {0}", component.Id);
                return;
            }

            DesktopAudioSourceData data = m_audioSources.Find(s => s.Component.Id == component.Id);

            //AL.Source(sourceData.OpenALSourceId, ALSourcei.Buffer, clipData.OpenALBufferId);
            AL.SourcePlay(data.OpenALSourceId);
        }

        internal override void PauseAudioSource(AudioComponent component) {
            if (m_audioSources.FindIndex(s => s.Component.Id == component.Id) == -1) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, "Cannot pause audio source because there is no data associated with AudioComponent with Id: {0}", component.Id);
                return;
            }

            DesktopAudioSourceData data = m_audioSources.Find(s => s.Component.Id == component.Id);

            AL.SourcePause(data.OpenALSourceId);
        }

        internal override void StopAudioSource(AudioComponent component) {
            if (m_audioSources.FindIndex(s => s.Component.Id == component.Id) == -1) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, "Cannot stop audio source because there is no data associated with AudioComponent with Id: {0}", component.Id);
                return;
            }

            DesktopAudioSourceData data = m_audioSources.Find(s => s.Component.Id == component.Id);

            AL.SourceStop(data.OpenALSourceId);
        }

        internal override void SetAudioSourceClip(AudioComponent component, AudioClip clip) {
            DesktopAudioSourceData sourceData = m_audioSources.Find(s => s.Component.Id == component.Id);
            DesktopAudioClipData clipData = m_audioClips.Find(c => c.Clip.Id == clip.Id);

            if (sourceData == null) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, "Cannot set audio clip of audio source as the given source is invalid");
                return;
            }

            if (clipData == null) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, "Cannot set the audio clip of the audio source as the given clip is invalid");
                return;
            }

            AL.Source(sourceData.OpenALSourceId, ALSourcei.Buffer, clipData.OpenALBufferId);
        }

        internal override void CreateAudioClip(AudioClip clip) {
            if (m_audioClips.FindIndex(s => s.Clip.Id == clip.Id) != -1) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, "The AudioManager already contains data for the AudioClip with Id: {0}", clip.Id);
                return;
            }

            GCHandle handle = GCHandle.Alloc(clip.Data, GCHandleType.Pinned);

            int alId = AL.GenBuffer();

            AL.BufferData(alId, GetSoundFormat(clip.NumberOfChannels, clip.BitsPerSample), handle.AddrOfPinnedObject(), clip.Data.Length, clip.NumberOfSamples);

            m_audioClips.Add(new DesktopAudioClipData(clip, handle, alId));
        }

        internal override void DeleteAudioClip(AudioClip clip) {
            DesktopAudioClipData clipData = m_audioClips.Find(c => c.Clip.Id == clip.Id);

            if (clipData == null) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, "Cannot delete AudioClip because there is no data associated with an AudioClip with Id: {0}", clip.Id);
                return;
            }

            clipData.DataHandle.Free();

            AL.DeleteBuffer(clipData.OpenALBufferId);

            m_audioClips.Remove(clipData);
        }

        internal override AudioClip LoadWavFile(string path) {
            try {
                FileStream stream = File.Open(path, FileMode.Open);

                if (stream == null) {
                    throw new Exception("Cannot read a null stream");
                }

                using (BinaryReader reader = new BinaryReader(stream)) {
                    // RIFF header
                    string signature = new string(reader.ReadChars(4));
                    if (signature != "RIFF") {
                        throw new Exception("Specified stream is not a wave file.");
                    }

                    // Read the RIFF chunk size and do nothing with it
                    reader.ReadInt32();

                    string format = new string(reader.ReadChars(4));
                    // if here we have skipped over the junk chunk
                    if (format != "WAVE") {
                        throw new Exception("Specified wave file is not supported");
                    }

                    // WAVE header
                    string formatSignature = new string(reader.ReadChars(4));
                    if (formatSignature == "JUNK") {
                        SkipJunkWaveChunk(reader);

                        formatSignature = new string(reader.ReadChars(4));
                    }

                    if (formatSignature != "fmt ") {
                        throw new Exception("Specified wave file is not supported.");
                    }

                    int format_chunk_size = reader.ReadInt32();
                    int audio_format = reader.ReadInt16();
                    int num_channels = reader.ReadInt16();
                    int sample_rate = reader.ReadInt32();
                    int byte_rate = reader.ReadInt32();
                    int block_align = reader.ReadInt16();
                    int bits_per_sample = reader.ReadInt16();

                    string data_signature = new string(reader.ReadChars(4));
                    if (data_signature != "data") {
                        throw new Exception("Specified wave file is not supported.");
                    }

                    byte[] byteData = reader.ReadBytes((int)reader.BaseStream.Length);

                    AudioClip newClip = new AudioClip(num_channels, bits_per_sample, sample_rate, byteData);

                    stream.Close();

                    return newClip;
                }
            } catch (Exception e) {
                TackConsole.EngineLog(TackConsole.LogType.Error, e.Message);
                return null;
            }
        }

        internal override AudioClip LoadOggFile(string path) {
            try {
                FileStream fs = File.OpenRead(path);
                using (NVorbis.VorbisReader reader = new NVorbis.VorbisReader(fs)) {
                    float[] sampleData = new float[reader.Channels * reader.SampleRate];

                    using (MemoryStream ms = new MemoryStream()) {
                        using (BinaryWriter writer = new BinaryWriter(ms)) {
                            //Keep reading the Vorbis file until we've got nothing left to read.
                            while (reader.SamplePosition < reader.TotalSamples - 1) {
                                //Read data from the Vorbis stream into our sample buffer. ReadSamples() returns how many samples were written to the buffer.
                                //We'll use that to know how many samples to convert to PCM instead of blindly writing to the PCM stream causing audible hitches at the end of the sound effect due to duplicate sample data being written.
                                int read = reader.ReadSamples(sampleData, 0, sampleData.Length);

                                for (int i = 0; i < read; i++) {
                                    writer.Write((short)(sampleData[i] * CONVERT_TO_SC16));
                                }
                            }
                        }

                        // Null the read buffer.
                        // This will be cleaned by the garbarge collector in the future
                        sampleData = null;

                        return new AudioClip(reader.Channels, 0, reader.SampleRate, ms.GetBuffer());
                    }
                }
            } catch (Exception e) {
                TackConsole.EngineLog(TackConsole.LogType.Error, e.Message);

                return null;
            }
        }

        private AudioComponent.AudioComponentState GetStateFromALState(ALSourceState state) {
            switch (state) {
                case ALSourceState.Initial:
                    return AudioComponent.AudioComponentState.Stopped;
                case ALSourceState.Playing:
                    return AudioComponent.AudioComponentState.Playing;
                case ALSourceState.Paused:
                    return AudioComponent.AudioComponentState.Paused;
                case ALSourceState.Stopped:
                    return AudioComponent.AudioComponentState.Stopped;
                default:
                    return AudioComponent.AudioComponentState.Stopped;
            }
        }

        internal static ALFormat GetSoundFormat(int channels, int bits) {
            switch (channels) {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }

        internal void SkipJunkWaveChunk(BinaryReader reader) {
            int junkChunkSize = reader.ReadInt32();

            // read the junk bytes and do nothing with them
            reader.ReadBytes(junkChunkSize);
        }
    }
}
