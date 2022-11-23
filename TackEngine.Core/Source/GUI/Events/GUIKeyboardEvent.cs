using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngineLib.Main;
using TackEngineLib.Input;

namespace TackEngineLib.GUI.Events {
    public class GUIKeyboardEvent {
        public GUIKeyboardEventArgs Args { get; private set; }

        public GUIKeyboardEvent(KeyboardKey key, KeyboardKeyAction action, bool shiftMod, bool capsLock) {
            Args = new GUIKeyboardEventArgs(key, action, shiftMod, capsLock);
        }
    }
}
