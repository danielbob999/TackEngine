using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.Main;

namespace TackEngine.Core.GUI {
    internal interface IGUIScrollable {
        float HorizontalScrollPosition { get; set; }
        float VerticalScrollPosition { get; set; }
        float ScrollSensitivity { get; set; }
        bool CanScroll { get; set; }

        public Vector2f GetContentSize();
    }
}
