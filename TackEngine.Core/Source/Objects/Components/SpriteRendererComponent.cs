using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Renderer;

namespace TackEngineLib.Objects.Components {
    public class SpriteRendererComponent : TackComponent, IRendererComponent {
        /// <summary>
        /// The Sprite to be rendered.
        /// </summary>
        public Sprite Sprite { get; set; }

        /// <summary>
        /// The colour of this component
        /// </summary>
        public Colour4b Colour { get; set; }

        /// <summary>
        /// The shader used on this component
        /// </summary>
        public BaseShader Shader { get; private set; }

        /// <summary>
        /// The render layer of this component. Smaller values will render first (behind components with a higher value)
        /// </summary>
        public int RenderLayer { get; set; }

        /// <summary>
        /// The uniform values that will be sent to the fragment shader when rendering this component
        /// </summary>
        public Dictionary<string, object> ShaderUniformValues { get; private set; }

        /// <summary>
        /// Creates a new SpriteRendererComponent
        /// </summary>
        public SpriteRendererComponent() : base() {
            ShaderUniformValues = new Dictionary<string, object>();

            Colour = Colour4b.White;
            this.Sprite = Sprite.DefaultSprite;
            RenderLayer = 10;
            SetShader(TackRenderer.GetInstance().DefaultWorldShader);
        }

        /// <summary>
        /// Creates a new SpriteRendererComponent
        /// </summary>
        /// <param name="colour">The colour of this component</param>
        public SpriteRendererComponent(Colour4b colour) : base() {
            ShaderUniformValues = new Dictionary<string, object>();

            Colour = colour;
            this.Sprite = Sprite.DefaultSprite;
            RenderLayer = 10;
            SetShader(TackRenderer.GetInstance().DefaultWorldShader);
        }

        /// <summary>
        /// Creates a new SpriteRendererComponent
        /// </summary>
        /// <param name="colour">The colour of this component</param>
        /// <param name="sprite">The Sprite that this component will renderer</param>
        public SpriteRendererComponent(Colour4b colour, Sprite sprite) : base() {
            ShaderUniformValues = new Dictionary<string, object>();

            Colour = colour;
            this.Sprite = sprite;
            RenderLayer = 10;
            SetShader(TackRenderer.GetInstance().DefaultWorldShader);
        }

        /// <summary>
        /// Creates a new SpriteRendererComponent
        /// </summary>
        /// <param name="colour">The colour of this component</param>
        /// <param name="sprite">The Sprite that this component will renderer</param>
        /// <param name="renderLayer">The render layer of this component</param>
        public SpriteRendererComponent(Colour4b colour, Sprite sprite, int renderLayer) : base() {
            ShaderUniformValues = new Dictionary<string, object>();

            Colour = colour;
            this.Sprite = sprite;
            RenderLayer = renderLayer;
            SetShader(TackRenderer.GetInstance().DefaultWorldShader);
        }

        /// <summary>
        /// Creates a new SpriteRendererComponent
        /// </summary>
        /// <param name="colour">The colour of this component</param>
        /// <param name="sprite">The Sprite that this component will renderer</param>
        /// <param name="renderLayer">The render layer of this component</param>
        /// <param name="shader">The shader used when renderering this component</param>
        public SpriteRendererComponent(Colour4b colour, Sprite sprite, int renderLayer, BaseShader shader) : base() {
            ShaderUniformValues = new Dictionary<string, object>();

            Colour = colour;
            this.Sprite = sprite;
            RenderLayer = renderLayer;
            SetShader(shader);
        }

        public override void OnStart() {
            base.OnStart();
        }

        public override void OnUpdate() {
            base.OnUpdate();
        }

        public override void OnRender() {
            base.OnRender();
        }

        public override void OnAttachedToTackObject() {
            base.OnAttachedToTackObject();
        }


        public override void OnDetachedFromTackObject() {
            base.OnDetachedFromTackObject();
        }

        /// <summary>
        /// Sets the value of a shader uniform variable
        /// </summary>
        /// <param name="name">The name of the uniform variable</param>
        /// <param name="value">The new value of the uniform variable</param>
        public void SetUniformVariable(string name, object value) {
            if (ShaderUniformValues.ContainsKey(name)) {
                ShaderUniformValues[name] = value;
            }
        }

        /// <summary>
        /// Sets the shader to be used when rendering this component
        /// Note: Calling this method will reset the uniform variable values attached to this component
        /// </summary>
        /// <param name="shader"></param>
        public void SetShader(BaseShader shader) {
            if (shader == null) {
                return;
            }

            Shader = shader;

            ShaderUniformValues.Clear();

            foreach (string str in shader.UniformVariables) {
                ShaderUniformValues.Add(str, null);
            }
        }
    }
}
