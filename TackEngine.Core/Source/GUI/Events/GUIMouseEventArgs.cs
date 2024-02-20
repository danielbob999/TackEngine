using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Input;

namespace TackEngine.Core.GUI.Events {
    public class GUIMouseEventArgs :  GUIEventArgs {
        
        public Vector2i MousePosition { get; internal set; }
        public MouseButtonKey MouseButton { get; internal set; }
        public MouseButtonAction MouseAction { get; internal set; }

        internal GUIMouseEventArgs(Vector2i position, MouseButtonKey button, MouseButtonAction action) {
            MousePosition = position;
            MouseButton = button;
            MouseAction = action;
        }
    }
}
