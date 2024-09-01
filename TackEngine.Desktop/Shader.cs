using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using TackEngine.Desktop;
using TackEngine.Core.Main;

namespace TackEngine.Core.Renderer {
    public class Shader : BaseShader {
        internal Shader(string shaderName, TackShaderType type, string vertexSoure, string fragmentSource) : 
            base(shaderName, type, vertexSoure, fragmentSource) {
            Type = type;
            SupportsLighting = false;

            TackConsole.EngineLog(TackConsole.LogType.Message, "Starting compilation and linking of shader with name: " + Name);

            // Create shader program
            int shaderProgram = GL.CreateProgram();

            // Generate subshader ids
            int vertShaderId = CompileSubShader(vertexSoure, ShaderType.VertexShader);
            int fragShaderId = CompileSubShader(fragmentSource, ShaderType.FragmentShader);

            if (vertShaderId == -1 || fragShaderId == -1) {
                CompiledAndLinked = false;
                Id = -1;
                throw new Exception("Failed to compile one or both of the sub shaders");
            }

            // Link shaders to the shader program
            GL.AttachShader(shaderProgram, vertShaderId);
            GL.AttachShader(shaderProgram, fragShaderId);
            GL.LinkProgram(shaderProgram);

            GL.GetProgramInfoLog(shaderProgram, out string progLogStr);

            if (progLogStr != "") {
                TackConsole.EngineLog(TackConsole.LogType.Error, progLogStr);
                Id = -1;
                CompiledAndLinked = false;
                return;
            }

            GL.DeleteShader(vertShaderId);
            GL.DeleteShader(fragShaderId);

            Id = shaderProgram;
            CompiledAndLinked = true;

            EvaluateUniforms();

            TackConsole.EngineLog(TackConsole.LogType.Message, "Successfully created shader program with Id: '{0}' and Name: '{1}'", Id, Name);
        }

        protected override void EvaluateUniforms() {
            UniformVariables = new List<string>();

            GL.GetProgram(Id, GetProgramParameterName.ActiveUniforms, out int uniformCount);

            // for loop that iterates through 0-count, getting the uniform name/type at pos i
            for (int i = 0; i < uniformCount; i++) {
                string uniformName = GL.GetActiveUniform(Id, i, out int size, out ActiveUniformType type);

                if (ShaderParser.IsUserDefinedShaderUniformName(uniformName)) {
                    UniformVariables.Add(uniformName);
                }
            }
        }

        protected override int CompileSubShader(string source, ShaderType type) {
            if (type != ShaderType.VertexShader && type != ShaderType.FragmentShader) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "Cannot compile sub-shader of unknown type.");
                throw new Exception("Failed to compile sub shader, incorrect shader type. Type: " + type);
            }

            // Compile shader
            int subShaderId = GL.CreateShader(type == ShaderType.VertexShader ? OpenTK.Graphics.OpenGL.ShaderType.VertexShader : OpenTK.Graphics.OpenGL.ShaderType.FragmentShader);

            // Set the shader source
            GL.ShaderSource(subShaderId, source);

            // Compile shader
            GL.CompileShader(subShaderId);

            GL.GetShaderInfoLog(subShaderId, out string logStr);

            if (logStr != "") {
                throw new Exception("Failed to compile sub shader. Type: " + type.ToString() + " Msg: " + logStr);
            }

            TackConsole.EngineLog(TackConsole.LogType.Message, "Successfully complied sub-shader. Type: {0}, Lines: {1}.", type.ToString(), source.Count(x => x == '\n'));
            return subShaderId;
        }

        public override void Destroy() {
            GL.DeleteProgram(Id);
        }

        internal override void SetUniformValue(string name, int value) {
            GL.Uniform1(GL.GetUniformLocation(Id, name), value);
        }

        internal override void SetUniformValue(string name, double value) {
            GL.Uniform1(GL.GetUniformLocation(Id, name), value);
        }

        internal override void SetUniformValue(string name, float value) {
            GL.Uniform1(GL.GetUniformLocation(Id, name), value);
        }

        internal override void SetUniformValue(string name, uint value) {
            GL.Uniform1(GL.GetUniformLocation(Id, name), value);
        }

        internal override void SetUniformValue(string name, bool transpose, Matrix2 mat2) {
            OpenTK.Mathematics.Matrix2 mat2tk = mat2.ToOpenTKMat2();
            GL.UniformMatrix2(GL.GetUniformLocation(Id, name), transpose, ref mat2tk);
        }

        internal override void SetUniformValue(string name, bool transpose, Matrix3 mat3) {
            OpenTK.Mathematics.Matrix3 mat3tk = mat3.ToOpenTKMat3();
            GL.UniformMatrix3(GL.GetUniformLocation(Id, name), transpose, ref mat3tk);
        }

        internal override void SetUniformValue(string name, bool transpose, Matrix4 mat4) {
            OpenTK.Mathematics.Matrix4 mat4tk = mat4.ToOpenTKMat4();
            GL.UniformMatrix4(GL.GetUniformLocation(Id, name), transpose, ref mat4tk);
        }

        internal override void SetUniformValue(string name, Vector2f vec2) {
            OpenTK.Mathematics.Vector2 vec2tk = vec2.ToOpenTKVec2();
            GL.Uniform2(GL.GetUniformLocation(Id, name), ref vec2tk);
        }

        internal override void SetUniformValue(string name, TackEngine.Core.Main.Vector3 vec3) {
            OpenTK.Mathematics.Vector3 vec3tk = vec3.ToOpenTKVec3();
            GL.Uniform3(GL.GetUniformLocation(Id, name), ref vec3tk);
        }

        internal override void SetUniformValue(string name, Vector4 vec4) {
            OpenTK.Mathematics.Vector4 vec4tk = vec4.ToOpenTKVec4();
            GL.Uniform4(GL.GetUniformLocation(Id, name), ref vec4tk);
        }

        internal override void SetUniformValue(string name, Sprite sprite) {
            GL.ActiveTexture(TextureUnit.Texture0 + TackRenderer.Instance.CurrentTextureUnitIndex);
            GL.BindTexture(TextureTarget.Texture2D, sprite.Id);

            if (sprite.IsDynamic && sprite.IsDirty) {
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, sprite.Width, sprite.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, sprite.Data);

                sprite.IsDirty = false;
            }

            SetUniformValue(name, TackRenderer.Instance.CurrentTextureUnitIndex);

            TackRenderer.Instance.IncrementCurrentTextureUnitIndex();
        }

        internal override void Use() {
            GL.UseProgram(Id);
        }

        public static Shader LoadFromFile(string shaderName, TackShaderType type, string vertPath, string fragPath) {
            try {
                string vertSource = System.IO.File.ReadAllText(vertPath);
                string fragSource = System.IO.File.ReadAllText(fragPath);

                bool isVerified = ShaderParser.VerifyShader(vertSource, fragSource, out ShaderParser.ParsedShaderDetails details);

                if (type == TackShaderType.Line) {
                    isVerified = true;
                }

                if (!isVerified) {
                    throw new Exception("Failed to verify shader source");
                }

                Shader shader = new Shader(shaderName, type, vertSource, fragSource);
                shader.CameraInfoFragVariableName = details.m_cameraInfoVarName;
                shader.MaxLightCount = details.m_maxLightAmount;
                shader.LightingFragVariableName = details.m_lightArrayVarName;
                shader.SupportsLighting = details.m_supportsLighting;

                if (shader.CompiledAndLinked) {
                    return shader;
                }

                return null;
            } catch (Exception e) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "Error: Failed to load Shader from file");
                TackConsole.EngineLog(TackConsole.LogType.Error, "Error message: " + e.Message);
                return null;
            }
        }
    }
}
