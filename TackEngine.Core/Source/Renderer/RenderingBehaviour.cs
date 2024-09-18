using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.Main;
using TackEngine.Core.Engine;
using TackEngine.Core.Renderer;

namespace TackEngine.Core.Renderer {
    public abstract class RenderingBehaviour {

        protected float[] m_vertexData;
        protected int[] m_indiceData;

        public RenderingBehaviour(Type childType) {
            TackConsole.EngineLog(TackConsole.LogType.Message, "Initialised a new RenderingBehaviour of type: " + childType.Name);
        }

        public abstract void PreRender();

        public abstract void RenderToScreen(out int drawCallCount);

        public abstract void PostRender();
    }
}
