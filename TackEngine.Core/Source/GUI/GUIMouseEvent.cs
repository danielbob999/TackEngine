using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.GUI {
    internal class GUIMouseEvent {

        public int EventType { get; }       // The type of event that this instance represents
                                            // 0 : MouseButtonDown event
                                            // 1 : MouseButtonUp event

        public Input.MouseButtonKey MouseButtonKey { get; }

        public Main.Vector2f Position { get; }

        public GUIMouseEvent(int type, Main.Vector2f pos, Input.MouseButtonKey key) {
            EventType = type;
            Position = pos;
            MouseButtonKey = key;
        }
    }
}
