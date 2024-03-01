using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.Source.Audio;

namespace TackEngine.Desktop.Audio {
    internal class DesktopAudioClipData {
        public AudioClip Clip { get; private set; }
        public GCHandle DataHandle { get; private set; }
        public int OpenALBufferId { get; private set; }

        public DesktopAudioClipData(AudioClip clip, GCHandle handle, int alBufferId) {
            Clip = clip;
            DataHandle = handle;
            OpenALBufferId = alBufferId;
        }
    }
}
