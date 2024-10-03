using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using Vector3 = TackEngine.Core.Main.Vector3;

namespace TackEngine.Core.Renderer {
    public class Shader {
        public enum TackShaderType {
            FragmentShader = 35632,
            VertexShader
        }

        public enum ShaderContext {
            World,
            GUI,
            Line
        }

        public int Id { get; protected set; }
        public string Name { get; }
        public ShaderContext Context { get; protected set; }
        public bool CompiledAndLinked { get; protected set; }
        public List<string> UniformVariables { get; protected set; }
        public bool SupportsLighting { get; internal set; }
        internal string LightingFragVariableName { get; set; }
        internal string CameraInfoFragVariableName { get; set; }
        internal int MaxLightCount { get; set; }

        internal Shader(string shaderName, ShaderContext context, string vertexSoure, string fragmentSource) {
            Name = shaderName;
            Context = context;
            SupportsLighting = false;

            TackConsole.EngineLog(TackConsole.LogType.Message, "Starting compilation and linking of shader with name: " + Name);

            // Create shader program
            int shaderProgram = TackRenderer.Instance.ShaderImplementation.CreateProgram();

            // Generate subshader ids
            int vertShaderId = CompileSubShader(vertexSoure, TackShaderType.VertexShader);
            int fragShaderId = CompileSubShader(fragmentSource, TackShaderType.FragmentShader);

            if (vertShaderId == -1 || fragShaderId == -1) {
                CompiledAndLinked = false;
                Id = -1;
                throw new Exception("Failed to compile one or both of the sub shaders");
            }

            // Link shaders to the shader program
            TackRenderer.Instance.ShaderImplementation.AttachShader(shaderProgram, vertShaderId);
            TackRenderer.Instance.ShaderImplementation.AttachShader(shaderProgram, fragShaderId);
            TackRenderer.Instance.ShaderImplementation.LinkProgram(shaderProgram);

            TackRenderer.Instance.ShaderImplementation.GetProgramInfoLog(shaderProgram, out string progLogStr);

            if (progLogStr != "") {
                TackConsole.EngineLog(TackConsole.LogType.Error, progLogStr);
                Id = -1;
                CompiledAndLinked = false;
                return;
            }

            TackRenderer.Instance.ShaderImplementation.DeleteShader(vertShaderId);
            TackRenderer.Instance.ShaderImplementation.DeleteShader(fragShaderId);

            Id = shaderProgram;
            CompiledAndLinked = true;

            List<string> uniforms = GetUniforms(Id);
            UniformVariables = new List<string>();

            foreach (string s in uniforms) {
                if (ShaderParser.IsUserDefinedShaderUniformName(s)) {
                    UniformVariables.Add(s);
                }
            }

            TackConsole.EngineLog(TackConsole.LogType.Message, "Successfully created shader program with Id: '{0}' and Name: '{1}'", Id, Name);
        }

        protected List<string> GetUniforms(int id) {
            return TackRenderer.Instance.ShaderImplementation.GetUniforms(id);
        }

        protected int CompileSubShader(string source, TackShaderType type) {
            return TackRenderer.Instance.ShaderImplementation.CompileSubShader(source, (int)type);
        }

        public void Destroy() {
            TackRenderer.Instance.ShaderImplementation.Destroy(Id);
        }

        internal void SetUniformValue(string name, int value) {
            TackRenderer.Instance.ShaderImplementation.SetUniformValueInt(Id, name, value);
        }

        internal void SetUniformValue(string name, double value) {
            TackRenderer.Instance.ShaderImplementation.SetUniformValueDouble(Id, name, value);
        }

        internal void SetUniformValue(string name, float value) {
            TackRenderer.Instance.ShaderImplementation.SetUniformValueFloat(Id, name, value);
        }

        internal void SetUniformValue(string name, uint value) {
            TackRenderer.Instance.ShaderImplementation.SetUniformValueUInt(Id, name, value);
        }

        internal void SetUniformValue(string name, Matrix2 mat2, bool transpose = false) {
            TackRenderer.Instance.ShaderImplementation.SetUniformValueMat2(Id, name, mat2, transpose);
        }

        internal void SetUniformValue(string name, Matrix3 mat3, bool transpose = false) {
            TackRenderer.Instance.ShaderImplementation.SetUniformValueMat3(Id, name, mat3, transpose);
        }

        internal void SetUniformValue(string name, Matrix4 mat4, bool transpose = false) {
            TackRenderer.Instance.ShaderImplementation.SetUniformValueMat4(Id, name, mat4, transpose);
        }

        internal void SetUniformValue(string name, Vector2f vec2) {
            TackRenderer.Instance.ShaderImplementation.SetUniformValueVec2(Id, name, vec2);
        }

        internal void SetUniformValue(string name, Vector3 vec3) {
            TackRenderer.Instance.ShaderImplementation.SetUniformValueVec3(Id, name, vec3);
        }

        internal void SetUniformValue(string name, Vector4 vec4) {
            TackRenderer.Instance.ShaderImplementation.SetUniformValueVec4(Id, name, vec4);
        }

        internal void SetUniformValue(string name, Sprite sprite) {
            TackRenderer.Instance.ShaderImplementation.SetUniformValueSprite(Id, name, sprite);
        }

        internal void SetUniformValue(string name, Colour4b colour) {
            TackRenderer.Instance.ShaderImplementation.SetUniformValueColour(Id, name, colour);
        }

        internal void Use() {
            TackRenderer.Instance.ShaderImplementation.Use(Id);
        }

        public static Shader LoadFromFile(string shaderName, ShaderContext context, string vertPath, string fragPath) {
            return TackRenderer.Instance.ShaderImplementation.LoadShaderFromFile(shaderName, context, vertPath, fragPath);         
        }

    }
}
