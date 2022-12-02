using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;

namespace TackEngine.Core.GUI.Events {
    public class GUIMouseEvent {
        public GUIMouseEventArgs Args { get; private set; }

        public GUIMouseEvent(Vector2i pos, TackEngine.Core.Input.MouseButtonKey mouseButton, Input.MouseButtonAction action) {
            Args = new GUIMouseEventArgs(pos, mouseButton, action);
        }
    }
}
