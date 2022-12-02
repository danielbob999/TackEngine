using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.Input;

namespace TackEngine.Core.GUI.Events {
    public class GUIKeyboardEventArgs : GUIEventArgs {

        public KeyboardKey Key { get; internal set; }
        public KeyboardKeyAction KeyAction { get; internal set; }
        public bool ShiftModifier { get; internal set; }
        public bool CapsLock { get; internal set; }

        internal GUIKeyboardEventArgs(KeyboardKey key, KeyboardKeyAction action, bool shiftMod, bool capsLock) {
            Key = key;
            KeyAction = action;
            ShiftModifier = shiftMod;
            CapsLock = capsLock;
        }
    }
}
