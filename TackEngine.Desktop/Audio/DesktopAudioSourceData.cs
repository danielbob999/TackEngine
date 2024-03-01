using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.Objects.Components;

namespace TackEngine.Desktop.Audio {
    internal class DesktopAudioSourceData {
        public AudioComponent Component { get; private set; }
        public int OpenALSourceId { get; private set; }

        public DesktopAudioSourceData(AudioComponent component, int alSourceId) {
            Component = component;
            OpenALSourceId = alSourceId;
        }
    }
}
