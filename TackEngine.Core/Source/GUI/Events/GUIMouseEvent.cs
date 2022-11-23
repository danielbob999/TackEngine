using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.GUI.Events {
    public class GUIMouseEvent {
        public GUIMouseEventArgs Args { get; private set; }

        public GUIMouseEvent(Vector2i pos, TackEngineLib.Input.MouseButtonKey mouseButton, Input.MouseButtonAction action) {
            Args = new GUIMouseEventArgs(pos, mouseButton, action);
        }
    }
}
