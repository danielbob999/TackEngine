using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Input;

namespace TackEngine.Core.GUI.Events {
    public class GUIEventArgs {
        public static readonly GUIEventArgs Empty = new GUIEventArgs();

        internal GUIEventArgs() {

        }
    }
}
