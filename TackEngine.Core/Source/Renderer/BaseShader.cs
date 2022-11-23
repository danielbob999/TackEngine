using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.Renderer {
    public abstract class BaseShader {
        public enum ShaderType {
            VertexShader,
            FragmentShader
        }

        public int Id { get; protected set; }
        public string Name { get; }
        public TackShaderType Type { get; protected set; }
        public bool CompiledAndLinked { get; protected set; }
        public List<string> UniformVariables { get; protected set; }
        public bool SupportsLighting { get; internal set; }
        internal string LightingFragVariableName { get; set; }
        internal string CameraInfoFragVariableName { get; set; }
        internal int MaxLightCount { get; set; }

        internal BaseShader(string shaderName, TackShaderType type, string vertexSoure, string fragmentSource) {
            Name = shaderName;
            UniformVariables = new List<string>();
        }

        protected abstract void EvaluateUniforms();

        protected abstract int CompileSubShader(string source, ShaderType type);

        public abstract void Destroy();

        internal abstract void SetUniformValue(string name, int value);

        internal abstract void SetUniformValue(string name, double value);

        internal abstract void SetUniformValue(string name, float value);

        internal abstract void SetUniformValue(string name, uint value);

        internal abstract void SetUniformValue(string name, bool transpose, Matrix2 mat2);

        internal abstract void SetUniformValue(string name, bool transpose, Matrix3 mat3);

        internal abstract void SetUniformValue(string name, bool transpose, Matrix4 mat4);

        internal abstract void SetUniformValue(string name, Vector2f vec2);

        internal abstract void SetUniformValue(string name, Vector3 vec3);

        internal abstract void SetUniformValue(string name, Vector4 vec4);

        internal abstract void Use();
    }
}
