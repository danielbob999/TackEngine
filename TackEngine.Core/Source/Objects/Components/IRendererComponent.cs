﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.Main;
using TackEngine.Core.Renderer;

namespace TackEngine.Core.Objects.Components {
    interface IRendererComponent {
        BaseShader Shader { get; }
        Sprite Sprite { get; }
        Colour4b Colour { get; set; }
        int RenderLayer { get; set; }
        Dictionary<string, object> ShaderUniformValues { get; }

        void SetShader(BaseShader shader);
    }
}
