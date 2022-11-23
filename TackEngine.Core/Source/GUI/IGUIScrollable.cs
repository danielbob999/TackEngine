using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.GUI {
    internal interface IGUIScrollable {
        float HorizontalScrollPosition { get; set; }
        float VerticalScrollPosition { get; set; }
        float ScrollSensitivity { get; set; }
        bool CanScroll { get; set; }
    }
}
