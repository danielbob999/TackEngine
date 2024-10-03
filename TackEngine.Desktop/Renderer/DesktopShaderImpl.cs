using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.Main;
using TackEngine.Core.Renderer;
using OpenTK.Graphics.OpenGL;
using static TackEngine.Core.Renderer.Shader;

namespace TackEngine.Desktop.Renderer {
    internal class DesktopShaderImpl : IShaderImplementation {
        public int CreateProgram() {
            return GL.CreateProgram();
        }

        public int CreateShader(int shaderType) {
            return GL.CreateShader((ShaderType)shaderType);
        }

        public void DeleteShader(int id) {
            GL.DeleteShader(id);
        }

        public void AttachShader(int programId, int shaderId) {
            GL.AttachShader(programId, shaderId);
        }

        public void LinkProgram(int id) {
            GL.LinkProgram(id);
        }

        public void GetProgramInfoLog(int id, out string str) {
            GL.GetProgramInfoLog(id, out str);
        }

        public void Destroy(int id) {
            GL.DeleteProgram(id);
        }

        public int CompileSubShader(string source, int type) {
            if ((ShaderType)type != ShaderType.VertexShader && (ShaderType)type != ShaderType.FragmentShader) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "Cannot compile sub-shader of unknown type.");
                throw new Exception("Failed to compile sub shader, incorrect shader type. Type: " + type);
            }

            // Compile shader
            int subShaderId = GL.CreateShader((ShaderType)type == ShaderType.VertexShader ? OpenTK.Graphics.OpenGL.ShaderType.VertexShader : OpenTK.Graphics.OpenGL.ShaderType.FragmentShader);

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

        public List<string> GetUniforms(int id) {
            List<string> uniforms = new List<string>();

            GL.GetProgram(id, GetProgramParameterName.ActiveUniforms, out int uniformCount);

            // for loop that iterates through 0-count, getting the uniform name/type at pos i
            for (int i = 0; i < uniformCount; i++) {
                string uniformName = GL.GetActiveUniform(id, i, out int size, out ActiveUniformType type);

                uniforms.Add(uniformName);
            }

            return uniforms;
        }

        public void SetUniformValueInt(int id, string name, int value) {
            GL.Uniform1(GL.GetUniformLocation(id, name), value);
        }

        public void SetUniformValueDouble(int id, string name, double value) {
            GL.Uniform1(GL.GetUniformLocation(id, name), value);
        }

        public void SetUniformValueFloat(int id, string name, float value) {
            GL.Uniform1(GL.GetUniformLocation(id, name), value);
        }

        public void SetUniformValueUInt(int id, string name, uint value) {
            GL.Uniform1(GL.GetUniformLocation(id, name), value);
        }

        public void SetUniformValueMat2(int id, string name, Matrix2 mat2, bool transpose) {
            OpenTK.Mathematics.Matrix2 mat2tk = mat2.ToOpenTKMat2();
            GL.UniformMatrix2(GL.GetUniformLocation(id, name), transpose, ref mat2tk);
        }

        public void SetUniformValueMat3(int id, string name, Matrix3 mat3, bool transpose) {
            OpenTK.Mathematics.Matrix3 mat3tk = mat3.ToOpenTKMat3();
            GL.UniformMatrix3(GL.GetUniformLocation(id, name), transpose, ref mat3tk);
        }

        public void SetUniformValueMat4(int id, string name, Matrix4 mat4, bool transpose) {
            OpenTK.Mathematics.Matrix4 mat4tk = mat4.ToOpenTKMat4();
            GL.UniformMatrix4(GL.GetUniformLocation(id, name), transpose, ref mat4tk);
        }

        public void SetUniformValueVec2(int id, string name, Vector2f vec2) {
            OpenTK.Mathematics.Vector2 vec2tk = vec2.ToOpenTKVec2();
            GL.Uniform2(GL.GetUniformLocation(id, name), ref vec2tk);
        }

        public void SetUniformValueVec3(int id, string name, TackEngine.Core.Main.Vector3 vec3) {
            OpenTK.Mathematics.Vector3 vec3tk = vec3.ToOpenTKVec3();
            GL.Uniform3(GL.GetUniformLocation(id, name), ref vec3tk);
        }

        public void SetUniformValueVec4(int id, string name, Vector4 vec4) {
            OpenTK.Mathematics.Vector4 vec4tk = vec4.ToOpenTKVec4();
            GL.Uniform4(GL.GetUniformLocation(id, name), ref vec4tk);
        }

        public void SetUniformValueColour(int id, string name, Colour4b colour) {
            OpenTK.Mathematics.Vector4 vec4tk = new OpenTK.Mathematics.Vector4(colour.R, colour.G, colour.B, colour.A);
            GL.Uniform4(GL.GetUniformLocation(id, name), ref vec4tk);
        }

        public void SetUniformValueSprite(int id, string name, Sprite sprite) {
            GL.ActiveTexture(TextureUnit.Texture0 + TackRenderer.Instance.CurrentTextureUnitIndex);
            GL.BindTexture(TextureTarget.Texture2D, sprite.Id);

            if (sprite.IsDynamic && sprite.IsDirty) {
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, sprite.Width, sprite.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, sprite.Data);

                sprite.IsDirty = false;
            }

            SetUniformValueInt(id, name, TackRenderer.Instance.CurrentTextureUnitIndex);

            TackRenderer.Instance.IncrementCurrentTextureUnitIndex();
        }

        public void Use(int id) {
            GL.UseProgram(id);
        }

        public Shader LoadShaderFromFile(string shaderName, Shader.ShaderContext context, string vertPath, string fragPath) {
            try {
                string vertSource = System.IO.File.ReadAllText(vertPath);
                string fragSource = System.IO.File.ReadAllText(fragPath);

                bool isVerified = ShaderParser.VerifyShader(vertSource, fragSource, out ShaderParser.ParsedShaderDetails details);

                if (context == ShaderContext.Line) {
                    isVerified = true;
                }

                if (!isVerified) {
                    throw new Exception("Failed to verify shader source");
                }

                Shader shader = new Shader(shaderName, context, vertSource, fragSource);
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
