using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Main;
using static TackEngine.Core.Renderer.Shader;

namespace TackEngine.Core.Renderer {
    internal interface IShaderImplementation {

        public int CreateProgram();

        public int CreateShader(int type);

        public void DeleteShader(int id);

        public void AttachShader(int programId, int shaderId);

        public void LinkProgram(int id);

        public void GetProgramInfoLog(int id, out string str);

        public List<string> GetUniforms(int id);

        public int CompileSubShader(string source, int type);

        public void Destroy(int id);

        public void SetUniformValueInt(int id, string name, int value);

        public void SetUniformValueDouble(int id, string name, double value);

        public void SetUniformValueFloat(int id, string name, float value);

        public void SetUniformValueUInt(int id, string name, uint value);

        public void SetUniformValueMat2(int id, string name, Matrix2 mat2, bool transpose);

        public void SetUniformValueMat3(int id, string name, Matrix3 mat3, bool transpose);

        public void SetUniformValueMat4(int id, string name, Matrix4 mat4, bool transpose);

        public void SetUniformValueVec2(int id, string name, Vector2f vec2);

        public void SetUniformValueVec3(int id, string name, Vector3 vec3);

        public void SetUniformValueVec4(int id, string name, Vector4 vec4);

        public void SetUniformValueSprite(int id, string name, Sprite sprite);

        public void SetUniformValueColour(int id, string name, Colour4b colour);

        public void Use(int id);

        public Shader LoadShaderFromFile(string shaderName, ShaderContext context, string vertPath, string fragPath);
    }
}
