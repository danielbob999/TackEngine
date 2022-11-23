using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngineLib.Main;
using TackEngineLib.Renderer;

namespace TackEngineLib.Objects.Components {
    interface IRendererComponent {
        BaseShader Shader { get; }
        Sprite Sprite { get; }
        Colour4b Colour { get; set; }
        int RenderLayer { get; set; }
        Dictionary<string, object> ShaderUniformValues { get; }

        void SetShader(BaseShader shader);
    }
}
