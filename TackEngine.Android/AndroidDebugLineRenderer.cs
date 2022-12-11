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
using OpenTK.Graphics.ES30;

namespace TackEngine.Android {
    internal class AndroidDebugLineRenderer : DebugLineRenderer {

        private int posHandle;

        public AndroidDebugLineRenderer() : base() {
            Instance = this;
        }

        public override void OnStart() {
            base.OnStart();

            using (StreamReader vertReader = new StreamReader(AndroidContext.CurrentAssetManager.Open("tackresources/shaders/debuglinerenderer/debug_line_renderer_vert.vs"))) {
                string vertSource = vertReader.ReadToEnd();

                using (StreamReader fragReader = new StreamReader(AndroidContext.CurrentAssetManager.Open("tackresources/shaders/debuglinerenderer/debug_line_renderer_frag.fs"))) {
                    string fragSource = fragReader.ReadToEnd();

                    m_shader = new Shader("shaders.debug_line_renderer_shader", TackShaderType.World, vertSource, fragSource);
                }
            }

            posHandle = GL.GetAttribLocation(m_shader.Id, "aPos");
        }

        public override void OnRender() {
            TackProfiler.Instance.StartTimer("AndroidDebugLineRenderer.OnRender");

            /*
            //int VAO = GL.GenVertexArray();
            //int VBO = GL.GenBuffer();
            //int EBO = GL.GenBuffer();

            // DO NOT USE THIS CRAP
            GL.GenBuffers(1, out int VAO);
            GL.GenBuffers(1, out int VBO);
            GL.GenBuffers(1, out int EBO);

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 20, m_vertexData, BufferUsage.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * 6, m_indiceData, BufferUsage.StaticDraw);

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
                OpenTK.Matrix4 modelMatrix = GenerateModelMatrix(m_lines[i].Position, new Vector2f(m_lines[i].Length, m_lines[i].Width), m_lines[i].Rotation + camera.GetParent().Rotation);

                connectedShader.SetUniformValue("uModelMat", false, modelMatrix.ToTEMat4());
                //GL.UniformMatrix4(GL.GetUniformLocation(connectedShader.Id, "uModelMat"), false, ref modelMatrix);

                Vector4 colourVector = new Vector4(m_lines[i].Colour.R / 255.0f, m_lines[i].Colour.G / 255.0f, m_lines[i].Colour.B / 255.0f, m_lines[i].Colour.A / 255.0f);
                //GL.Uniform4(GL.GetUniformLocation(m_defaultWorldShader.Id, "uColour"), ref colourVector);
                connectedShader.SetUniformValue("uColour", colourVector);

                GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
            }

            GL.DeleteBuffers(1, ref EBO);
            GL.DeleteBuffers(1, ref VBO);
            GL.DeleteBuffers(1, ref VAO);
            */

            //GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            /*
            GL.EnableVertexAttribArray(posHandle);

            //GL.VertexAttribPointer(posHandle, 3, All.Float, false, 0, m_vData1); // this may need to be converted to a IntPtr
            GL.VertexAttribPointer(posHandle, 3, VertexAttribPointerType.Float, false, 0, m_vData1);
            */

            GL.GenBuffers(1, out int VBO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * 20), m_vertexData, BufferUsage.StaticDraw);

            // position attribute
            GL.VertexAttribPointer(posHandle, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(posHandle);

            // set default (4 byte) pixel alignment 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

            // Master variables
            Camera camera = Camera.MainCamera;

            for (int i = 0; i < m_lines.Count; i++) {
                BaseShader connectedShader = m_shader;

                connectedShader.Use();

                // Generate model matrix
                OpenTK.Matrix4 modelMatrix = GenerateModelMatrix(m_lines[i].Position, new Vector2f(m_lines[i].Length, m_lines[i].Width), m_lines[i].Rotation + camera.GetParent().Rotation);

                connectedShader.SetUniformValue("uModelMat", false, modelMatrix.ToTEMat4());

                Vector4 colourVector = new Vector4(m_lines[i].Colour.R / 255.0f, m_lines[i].Colour.G / 255.0f, m_lines[i].Colour.B / 255.0f, m_lines[i].Colour.A / 255.0f);
                //GL.Uniform4(GL.GetUniformLocation(m_defaultWorldShader.Id, "uColour"), ref colourVector);
                connectedShader.SetUniformValue("uColour", colourVector);

                unsafe {
                    fixed (void* ptr = m_indiceData) {
                        GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, new IntPtr(ptr));
                    }
                }
            }

            GL.DeleteBuffers(1, ref VBO);

            base.OnRender();
            TackProfiler.Instance.StopTimer("AndroidDebugLineRenderer.OnRender");
        }

        public override void OnClose() {
            base.OnClose();
        }

        private OpenTK.Matrix4 GenerateModelMatrix(Vector2f position, Vector2f scale, float rotation) {
            Vector2f cameraPosition = Camera.MainCamera.GetParent().Position;

            // Generate translation matrix
            OpenTK.Matrix4 transMat = OpenTK.Matrix4.CreateTranslation(position.X - cameraPosition.X, position.Y - cameraPosition.Y, 0);

            // Generate scale matrix
            OpenTK.Matrix4 scaleMat = MatrixUtility.CreateScaleMatrix(scale.X / 2.0f, scale.Y / 2.0f, 1);

            // Generate rotation matrix
            OpenTK.Matrix4 rotationMat = OpenTK.Matrix4.CreateRotationZ((float)TackEngine.Core.Math.TackMath.DegToRad(rotation));

            // Generate the view matrix
            float widthToHeightRatio = TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.Y / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.X;

            Vector2f s = TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize;

            OpenTK.Matrix4 orthoView = new OpenTK.Matrix4(
                new OpenTK.Vector4((1 * Camera.MainCamera.ZoomFactor) * (1f / ((float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.X / 2.0f)), 0, 0, 0),
                new OpenTK.Vector4(0, (1 * Camera.MainCamera.ZoomFactor) * (1f / ((float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.Y / 2.0f)), 0, 0),
                new OpenTK.Vector4(0, 0, 1, 0),
                new OpenTK.Vector4(0, 0, 0, 1)
                );

            // Multiply the above matrices to generate the final model matrix
            OpenTK.Matrix4 modelMatrix = ((scaleMat * rotationMat) * transMat) * orthoView;

            return modelMatrix;
        }
    }
}
