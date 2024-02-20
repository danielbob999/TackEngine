using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.Engine;
using TackEngine.Core.Main;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Objects;
using TackEngine.Core.Renderer;
using OpenTK.Graphics.OpenGL;

namespace TackEngine.Desktop {
    internal class DesktopDebugLineRenderer : DebugLineRenderer {

        public DesktopDebugLineRenderer() : base() {
            Instance = this;
        }

        public override void OnStart() {
            base.OnStart();

            m_shader = new Shader("shaders.debug_line_renderer_shader", TackShaderType.World, System.IO.File.ReadAllText("tackresources/shaders/debuglinerenderer/debug_line_renderer_vert.vs"),
                                                                                              System.IO.File.ReadAllText("tackresources/shaders/debuglinerenderer/debug_line_renderer_frag.fs"));
        }

        public override void OnRender() {
            TackProfiler.Instance.StartTimer("DesktopDebugLineRenderer.OnRender");
            int VAO = GL.GenVertexArray();
            int VBO = GL.GenBuffer();
            int EBO = GL.GenBuffer();

            // DO NOT USE THIS CRAP
            //GL.GenBuffers(1, out int VAO);
            //GL.GenBuffers(1, out int VBO);
            //GL.GenBuffers(1, out int EBO);

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 20, m_vertexData, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * 6, m_indiceData, BufferUsageHint.StaticDraw);

            // position attribute
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // set default (4 byte) pixel alignment 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

            // Master variables
            Camera camera = Camera.MainCamera;

            int localDrawCallCount = 0;

            // Run the main draw loop
            for (int i = 0; i < m_lines.Count; i++) {
                BaseShader connectedShader = m_shader;

                connectedShader.Use();

                // Generate model matrix
                OpenTK.Mathematics.Matrix4 modelMatrix = GenerateModelMatrix(m_lines[i].Position, new Vector2f(m_lines[i].Length, m_lines[i].Width), m_lines[i].Rotation + camera.GetParent().Rotation);

                connectedShader.SetUniformValue("uModelMat", false, modelMatrix.ToTEMat4());
                //GL.UniformMatrix4(GL.GetUniformLocation(connectedShader.Id, "uModelMat"), false, ref modelMatrix);

                Vector4 colourVector = new Vector4(m_lines[i].Colour.R / 255.0f, m_lines[i].Colour.G / 255.0f, m_lines[i].Colour.B / 255.0f, m_lines[i].Colour.A / 255.0f);
                //GL.Uniform4(GL.GetUniformLocation(m_defaultWorldShader.Id, "uColour"), ref colourVector);
                connectedShader.SetUniformValue("uColour", colourVector);

                GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
            }

            GL.DeleteBuffers(1, ref EBO);
            GL.DeleteBuffers(1, ref VBO);
            GL.DeleteBuffers(1, ref VAO);

            base.OnRender();
            TackProfiler.Instance.StopTimer("DesktopDebugLineRenderer.OnRender");
        }

        public override void OnClose() {
            base.OnClose();
        }

        private OpenTK.Mathematics.Matrix4 GenerateModelMatrix(Vector2f position, Vector2f scale, float rotation) {
            Vector2f cameraPosition = Camera.MainCamera.GetParent().Position;

            // Generate translation matrix
            OpenTK.Mathematics.Matrix4 transMat = OpenTK.Mathematics.Matrix4.CreateTranslation(position.X - cameraPosition.X, position.Y - cameraPosition.Y, 0);

            // Generate scale matrix
            OpenTK.Mathematics.Matrix4 scaleMat = OpenTK.Mathematics.Matrix4.CreateScale(scale.X / 2.0f, scale.Y / 2.0f, 1);

            // Generate rotation matrix
            OpenTK.Mathematics.Matrix4 rotationMat = OpenTK.Mathematics.Matrix4.CreateRotationZ((float)TackEngine.Core.Math.TackMath.DegToRad(rotation));

            // Generate the view matrix
            float widthToHeightRatio = TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.Y / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.X;

            OpenTK.Mathematics.Matrix4 orthoView = new OpenTK.Mathematics.Matrix4(
                new OpenTK.Mathematics.Vector4((1 * Camera.MainCamera.ZoomFactor) * (1f / ((float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.X / 2.0f)), 0, 0, 0),
                new OpenTK.Mathematics.Vector4(0, (1 * Camera.MainCamera.ZoomFactor) * (1f / ((float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.Y / 2.0f)), 0, 0),
                new OpenTK.Mathematics.Vector4(0, 0, 1, 0),
                new OpenTK.Mathematics.Vector4(0, 0, 0, 1)
                );

            // Multiply the above matrices to generate the final model matrix
            OpenTK.Mathematics.Matrix4 modelMatrix = ((scaleMat * rotationMat) * transMat) * orthoView;

            return modelMatrix;
        }
    }
}
