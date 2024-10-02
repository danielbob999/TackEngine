using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Main;
using TackEngine.Core.Source.Renderer.LineRendering;

namespace TackEngine.Core.Renderer.LineRendering {
    internal abstract class LineRenderingBehaviour {

        public abstract void Initialise();

        public abstract void OnPreRender();

        public abstract void RenderLineToScreen(Line line, LineRenderer.LineContext context);

        public abstract void OnPostRender();

        public abstract void Close();
    }
}
