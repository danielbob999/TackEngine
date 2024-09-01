﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.Main;
using TackEngine.Core.Math;
using TackEngine.Core.Renderer;
using TackEngine.Core.Renderer.LineRendering;
using TackEngine.Core.Source.Renderer.LineRendering;
using OpenTK.Graphics.ES30;
using TackEngine.Core.Objects.Components;

namespace TackEngine.Android.Renderer {
    internal class AndroidLineRenderingBehaviour : LineRenderingBehaviour {
        private Shader m_lineShader;

        public override void Initialise() {
            m_lineShader = Shader.LoadFromFile("shaders.line", TackShaderType.Line, "tackresources/shaders/linerenderer/line_vertex_shader.vs",
                                                                                     "tackresources/shaders/linerenderer/line_fragment_shader.fs");
        }

        public override void RenderLineToScreen(Line line, LineRenderer.LineContext context) {
            float[] vertexData = new float[20] {
                    //       Position (XYZ)                                                                                                      Colours (RGB)                                                                                  TexCoords (XY)
                    /* v1 */  1f, -1f, 1.0f,       1.0f, 1.0f,
                    /* v2 */  1f,  1f, 1.0f,       1.0f, 0.0f,
                    /* v3 */ -1f,  1f, 1.0f,       0.0f, 0.0f,
                    /* v4 */ -1f, -1f, 1.0f,       0.0f, 1.0f
            };

            int[] indiceData = new int[] {
                    0, 1, 3, // first triangle
                    1, 2, 3  // second triangle
            };

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            /*
             *    v4 ------ v1
             *    |         |
             *    |         |
             *    |         |
             *    v3 ------ v2
             * 
             */

            GL.GenBuffers(1, out int VBO);

            int posHandle = GL.GetAttribLocation(m_lineShader.Id, "aPos");
            int uvHandle = GL.GetAttribLocation(m_lineShader.Id, "aTexCoord");

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * 20), vertexData, BufferUsage.StaticDraw);

            // position attribute
            GL.VertexAttribPointer(posHandle, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(posHandle);

            // position attribute
            GL.VertexAttribPointer(uvHandle, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(uvHandle);

            // position attribute
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // texture coords attribute
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // set default (4 byte) pixel alignment 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

            m_lineShader.Use();

            OpenTK.Matrix4 modelMatrix = new OpenTK.Matrix4();

            if (context == LineRenderer.LineContext.World) {
                Vector2f neg = line.PointB - line.PointA;
                Vector2f centerPosition = line.PointA + (neg.Normalized() * (neg.Length / 2f));

                // Generate model matrix
                modelMatrix = GenerateWorldModelMatrix(centerPosition, new Vector2f(line.Width, neg.Length), -1 * (float)Math.Acos(Vector2f.Dot(new Vector2f(0, 1), neg.Normalized())));

            }

            if (context == LineRenderer.LineContext.GUI) {
                Vector2f neg = line.PointB - line.PointA;
                Vector2f centerPosition = line.PointA + (neg.Normalized() * (neg.Length / 2f));

                // Generate model matrix
                modelMatrix = GenerateGUIModelMatrix(centerPosition, new Vector2f(neg.Length, line.Width), (float)Math.Acos(Vector2f.Dot(new Vector2f(0, 1), neg.Normalized())));
            }

            m_lineShader.SetUniformValue("uModelMat", false, modelMatrix.ToTEMat4());
            m_lineShader.SetUniformValue("uLineContext", (int)context);

            Vector4 colourVector = new Vector4(line.Colour.R / 255.0f, line.Colour.G / 255.0f, line.Colour.B / 255.0f, line.Colour.A / 255.0f);
            m_lineShader.SetUniformValue("uColour", colourVector);

            unsafe {
                fixed (void* ptr = indiceData) {
                    GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, new IntPtr(ptr));
                }
            }

            GL.DeleteBuffers(1, ref VBO);

        }

        public override void Close() {
        }

        private OpenTK.Matrix4 GenerateWorldModelMatrix(Vector2f position, Vector2f scale, float rotation) {
            Vector2f cameraPosition = Camera.MainCamera.GetParent().Position;

            // Generate translation matrix
            OpenTK.Matrix4 transMat = OpenTK.Matrix4.CreateTranslation(position.X - cameraPosition.X, position.Y - cameraPosition.Y, 0);

            // Generate scale matrix
            OpenTK.Matrix4 scaleMat = MatrixUtility.CreateScaleMatrix(scale.X / 2.0f, scale.Y / 2.0f, 1);

            // Generate rotation matrix
            OpenTK.Matrix4 rotationMat = OpenTK.Matrix4.CreateRotationZ(rotation);

            // Generate the view matrix
            float widthToHeightRatio = TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.Y / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.X;

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

        private OpenTK.Matrix4 GenerateGUIModelMatrix(Vector2f position, Vector2f scale, float rotation) {
            // Generate translation matrix
            OpenTK.Matrix4 transMat = OpenTK.Matrix4.CreateTranslation(position.X, -position.Y, 0);

            // Generate scale matrix
            OpenTK.Matrix4 scaleMat = MatrixUtility.CreateScaleMatrix((scale.X / 2.0f), (scale.Y / 2.0f), 1);

            // Generate rotation matrix
            OpenTK.Matrix4 rotationMat = OpenTK.Matrix4.CreateRotationZ(rotation + TackMath.DegToRad(90));

            // Generate the view matrix
            float widthToHeightRatio = TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.Y / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.X;

            OpenTK.Matrix4 orthoView = new OpenTK.Matrix4(
                new OpenTK.Vector4(2.0f / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.X, 0, 0, 0),
                new OpenTK.Vector4(0, 2.0f / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.Y, 0, 0),
                new OpenTK.Vector4(0, 0, 1, 0),
                new OpenTK.Vector4(0, 0, 0, 1)
                );

            // Multiply the above matrices to generate the final model matrix
            OpenTK.Matrix4 modelMatrix = ((scaleMat * rotationMat) * transMat) * orthoView;

            return modelMatrix;
        }
    }
}