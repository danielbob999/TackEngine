using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.Main;
using TackEngine.Core.Input;

namespace TackEngine.Core.GUI.Events {
    public class GUIKeyboardEvent {
        public GUIKeyboardEventArgs Args { get; private set; }

        public GUIKeyboardEvent(KeyboardKey key, KeyboardKeyAction action, bool shiftMod, bool capsLock) {
            Args = new GUIKeyboardEventArgs(key, action, shiftMod, capsLock);
        }
    }
}
