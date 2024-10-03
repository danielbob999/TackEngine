using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.Main;
using TackEngine.Core.Renderer;

namespace TackEngine.Core.Objects.Components {
    interface IRendererComponent {
        Shader Shader { get; }
        Sprite Sprite { get; }
        Colour4b Colour { get; set; }
        int RenderLayer { get; set; }
        Dictionary<string, object> ShaderUniformValues { get; }
        bool DisableRenderingBoundsCheck { get; set; }

        void SetShader(Shader shader);
    }
}
