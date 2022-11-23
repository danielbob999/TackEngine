using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Input;

namespace TackEngineLib.GUI.Events {
    public class GUIEventArgs {
        public static readonly GUIEventArgs Empty = new GUIEventArgs();

        internal GUIEventArgs() {

        }
    }
}
