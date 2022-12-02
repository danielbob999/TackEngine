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
        protected List<BaseShader> m_shaders;

        public abstract BaseShader DefaultWorldShader { get; }

        public RenderingBehaviour(Type childType) {
            m_shaders = new List<BaseShader>();
            TackConsole.EngineLog(TackConsole.LogType.Message, "Initialised a new RenderingBehaviour of type: " + childType.Name);
        }

        public abstract void RenderToScreen(out int drawCallCount);

        public void AddShader(BaseShader shader) {
            if (!shader.CompiledAndLinked) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "Error: Cannot add a shader that is not compiled and linked");
                return;
            }

            if (m_shaders.Count(x => x.Name == shader.Name) == 0) {
                m_shaders.Add(shader);

                TackConsole.EngineLog(TackConsole.LogType.Message, "Successfully added a new Shader to the current renderer with the name '" + shader.Name + "'");
                return;
            }

            TackConsole.EngineLog(TackConsole.LogType.Error, "Error: Failed to add a new Shader to the current renderer. There is already a Shader with the name '" + shader.Name + "'");
        }

        public BaseShader GetShader(string shaderName) {
            BaseShader shader = m_shaders.Find(x => x.Name == shaderName);

            if (shader == null) {
                return DefaultWorldShader;
            }

            return shader;
        }
    }
}
