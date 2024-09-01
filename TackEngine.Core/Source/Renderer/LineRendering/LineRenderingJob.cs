using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Main;

namespace TackEngine.Core.Source.Renderer.LineRendering {
    internal class LineRenderingJob {
        public List<Line> Lines { get; private set; }
        public LineRenderer.LineContext LineContext { get; private set; }

        public LineRenderingJob(List<Line> lines, LineRenderer.LineContext context) {
            Lines = lines;
            LineContext = context;
        }
    }
}
