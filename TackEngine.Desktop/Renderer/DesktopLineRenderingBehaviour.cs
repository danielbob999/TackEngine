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
using TackEngine.Core.Renderer.LineRendering;
using TackEngine.Core.Source.Renderer.LineRendering;
using OpenTK.Graphics.OpenGL;
using TackEngine.Core.Physics;
using TackEngine.Core.Math;

namespace TackEngine.Desktop.Renderer {
    internal class DesktopLineRenderingBehaviour : LineRenderingBehaviour {
        private Shader m_lineShader;
        private float[] m_vertexData;
        private int[] m_indiceData;
        private int m_vao;
        private int m_vbo;
        private int m_ebo;

        public override void Initialise() {
            m_lineShader = Shader.LoadFromFile("shaders.line", Shader.ShaderContext.Line, "tackresources/shaders/linerenderer/line_vertex_shader.vs",
                                                                                     "tackresources/shaders/linerenderer/line_fragment_shader.fs");

            m_vertexData = new float[20] {
                    //       Position (XYZ)                                                                                                      Colours (RGB)                                                                                  TexCoords (XY)
                    /* v1 */  1f, -1f, 1.0f,       1.0f, 1.0f,
                    /* v2 */  1f,  1f, 1.0f,       1.0f, 0.0f,
                    /* v3 */ -1f,  1f, 1.0f,       0.0f, 0.0f,
                    /* v4 */ -1f, -1f, 1.0f,       0.0f, 1.0f
            };

            m_indiceData = new int[] {
                    0, 1, 3, // first triangle
                    1, 2, 3  // second triangle
            };
        }

        public override void OnPreRender() {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            /*
             *    v4 ------ v1
             *    |         |
             *    |         |
             *    |         |
             *    v3 ------ v2
             * 
             */

            m_vao = GL.GenVertexArray();
            m_vbo = GL.GenBuffer();
            m_ebo = GL.GenBuffer();

            GL.BindVertexArray(m_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 20, m_vertexData, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * 6, m_indiceData, BufferUsageHint.StaticDraw);

            // position attribute
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // texture coords attribute
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        public override void RenderLineToScreen(Line line, LineRenderer.LineContext context) {
            // set default (4 byte) pixel alignment 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

            // Master variables
            Camera camera = Camera.MainCamera;

            m_lineShader.Use();

            OpenTK.Mathematics.Matrix4 modelMatrix = new OpenTK.Mathematics.Matrix4();

            Vector2f pointA = line.PointA;
            Vector2f pointB = line.PointB;

            if (pointA.X > pointB.X) {
                pointA = line.PointB;
                pointB = line.PointA;
            }

            Vector2f neg = pointB - pointA;
            Vector2f centerPosition = pointA + (neg.Normalized() * (neg.Length / 2f));

            if (context == LineRenderer.LineContext.World) {
                // Generate model matrix
                modelMatrix = GenerateWorldModelMatrix(centerPosition, new Vector2f(line.Width, neg.Length), -1 * (float)Math.Acos(Vector2f.Dot(new Vector2f(0, 1), neg.Normalized())));

            }

            if (context == LineRenderer.LineContext.GUI) {
                // Generate model matrix
                modelMatrix = GenerateGUIModelMatrix(centerPosition, new Vector2f(neg.Length, line.Width), (float)Math.Acos(Vector2f.Dot(new Vector2f(0, 1), neg.Normalized())));
            }

            m_lineShader.SetUniformValue("uModelMat", modelMatrix.ToTEMat4());
            m_lineShader.SetUniformValue("uLineContext", (int)context);

            Vector4 colourVector = new Vector4(line.Colour.R / 255.0f, line.Colour.G / 255.0f, line.Colour.B / 255.0f, line.Colour.A / 255.0f);
            m_lineShader.SetUniformValue("uColour", colourVector);

            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public override void OnPostRender() {
            GL.DeleteBuffers(1, ref m_ebo);
            GL.DeleteBuffers(1, ref m_vbo);
            GL.DeleteBuffers(1, ref m_vao);
        }

        public override void Close() {
        }

        private OpenTK.Mathematics.Matrix4 GenerateWorldModelMatrix(Vector2f position, Vector2f scale, float rotation) {
            Vector2f cameraPosition = Camera.MainCamera.GetParent().Position;

            // Generate translation matrix
            OpenTK.Mathematics.Matrix4 transMat = OpenTK.Mathematics.Matrix4.CreateTranslation(position.X - cameraPosition.X, position.Y - cameraPosition.Y, 0);

            // Generate scale matrix
            OpenTK.Mathematics.Matrix4 scaleMat = OpenTK.Mathematics.Matrix4.CreateScale(scale.X / 2.0f, scale.Y / 2.0f, 1);

            // Generate rotation matrix
            OpenTK.Mathematics.Matrix4 rotationMat = OpenTK.Mathematics.Matrix4.CreateRotationZ(rotation);

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

        private OpenTK.Mathematics.Matrix4 GenerateGUIModelMatrix(Vector2f position, Vector2f scale, float rotation) {
            // Generate translation matrix
            OpenTK.Mathematics.Matrix4 transMat = OpenTK.Mathematics.Matrix4.CreateTranslation(position.X, -position.Y, 0);

            // Generate scale matrix
            OpenTK.Mathematics.Matrix4 scaleMat = OpenTK.Mathematics.Matrix4.CreateScale((scale.X / 2.0f), (scale.Y / 2.0f), 1);

            // Generate rotation matrix
            OpenTK.Mathematics.Matrix4 rotationMat = OpenTK.Mathematics.Matrix4.CreateRotationZ(rotation + TackMath.DegToRad(90));

            // Generate the view matrix
            float widthToHeightRatio = TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.Y / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.X;

            OpenTK.Mathematics.Matrix4 orthoView = new OpenTK.Mathematics.Matrix4(
                new OpenTK.Mathematics.Vector4(2.0f / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.X, 0, 0, 0),
                new OpenTK.Mathematics.Vector4(0, 2.0f / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.Y, 0, 0),
                new OpenTK.Mathematics.Vector4(0, 0, 1, 0),
                new OpenTK.Mathematics.Vector4(0, 0, 0, 1)
                );

            // Multiply the above matrices to generate the final model matrix
            OpenTK.Mathematics.Matrix4 modelMatrix = ((scaleMat * rotationMat) * transMat) * orthoView;

            return modelMatrix;
        }
    }
}
