using Android.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.Objects.Components;

namespace TackEngine.Android.Audio {
    internal class AndroidAudioSourceData {
        public AudioComponent Component { get; private set; }
        public MediaPlayer Player { get; private set; }

        public AndroidAudioSourceData(AudioComponent component, MediaPlayer player) {
            Component = component;
            Player = player;
        }
    }
}
