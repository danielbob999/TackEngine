using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TackEngine.Core.Engine;
using TackEngine.Core.Main;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Objects;
using TackEngine.Core.Renderer;
using OpenTK.Graphics.ES30;
using TackEngine.Core.Physics;
using Android.Opengl;
using Java.Security.Cert;
using Java.Lang;
using tainicom.Aether.Physics2D.Fluids;
using TackEngine.Core.Source.Renderer;
using TackEngine.Core.Math;

namespace TackEngine.Android.Renderer
{
    public class AndroidRenderingBehaviour : RenderingBehaviour
    {
        private int posHandle;
        private int uvHandle;

        public AndroidRenderingBehaviour()
        {

            m_vertexData = new float[20] {
                    //       Position (XYZ)                                                                                                      Colours (RGB)                                                                                  TexCoords (XY)
                    /* v1 */  1f, -1f, 1.0f,       1.0f, 1.0f,
                    /* v2 */  1f,  1f, 1.0f,       1.0f, 0.0f,
                    /* v3 */ -1f,  1f, 1.0f,       0.0f, 0.0f,
                    /* v4 */ -1f, -1f, 1.0f,       0.0f, 1.0f
            };

            m_indiceData = new int[] {
                    0, 1, 3, // first triangle 
                    1, 2, 3  // second triangle - 1, 2, 3
            };
        }

        public override void OnStart()
        {
            posHandle = GL.GetAttribLocation(TackRenderer.Instance.DefaultLitWorldShader.Id, "aPos");
            uvHandle = GL.GetAttribLocation(TackRenderer.Instance.DefaultLitWorldShader.Id, "aTexCoord");
        }

        public override void PreRender()
        {
        }

        public override void RenderToScreen(out int drawCallCount)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            int lastBoundSpriteId = -1;

            /*
             *    v4 ------ v1
             *    |         |
             *    |         |
             *    |         |
             *    v3 ------ v2
             * 
             */

            TackProfiler.Instance.StartTimer("Renderer.CreateBindBuffers");

            CheckForErrors(true);

            GL.GenBuffers(1, out int VBO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * 20), m_vertexData, BufferUsage.StaticDraw);

            // position attribute
            GL.VertexAttribPointer(posHandle, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(posHandle);

            // position attribute
            GL.VertexAttribPointer(uvHandle, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(uvHandle);

            // set default (4 byte) pixel alignment 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

            TackProfiler.Instance.StopTimer("Renderer.CreateBindBuffers");
            TackProfiler.Instance.StartTimer("Renderer.GetObjectData");

            // Get a list of lights in the scene
            LightComponent[] lights = TackLightingSystem.Instance.GetLightComponents();

            // The tackobjects that have some type of renderer component attached
            TackObject[] tackObjects = TackObject.Get();

            //List<TackObject> sortedObjects = tackObjects.ToList().FindAll(x => x.GetComponent<SpriteRendererComponent>() != null);
            List<TackObject> sortedObjects = tackObjects.ToList().FindAll(x => x.HasRendererComponent());

            sortedObjects = sortedObjects.OrderBy(x => x.GetRendererComponent().RenderLayer).ToList();
            TackProfiler.Instance.StopTimer("Renderer.GetObjectData");

            List<int> shadersLightingDataSet = new List<int>();
            List<int> shadersCamDataSet = new List<int>();

            // Master variables
            Camera camera = Camera.MainCamera;

            int localDrawCallCount = 0;

            // Run the main draw loop
            for (int i = 0; i < sortedObjects.Count; i++)
            {
                if (!TackObject.IsActiveInHierarchy(sortedObjects[i]))
                {
                    continue;
                }

                IRendererComponent rendererComp = sortedObjects[i].GetRendererComponent();

                //SpriteRendererComponent rendererComp = sortedObjects[i].GetComponent<SpriteRendererComponent>();

                if (rendererComp == null)
                {
                    continue;
                }

                if (!((TackComponent)rendererComp).Active)
                {
                    continue;
                }

                Camera[] cameras = TackRenderer.Instance.GetCameraListForCurrentSplitScreenMode();

                for (int camIndex = 0; camIndex < cameras.Length; camIndex++)
                {

                    if (!rendererComp.DisableRenderingBoundsCheck)
                    {
                        // Check if the camera view bounding box and the object bounding box intersect
                        // If they don't, skip this object because it is entirely outside the screen
                        AABB camAABB = camera.BoundingBoxInWorld;
                        AABB objAABB = sortedObjects[i].BoundingBoxInWorld;

                        if (!AABB.CheckForAABBCollision(objAABB, camAABB))
                        {
                            continue;
                        }
                    }

                    // Reset the current texture unit index back to 0
                    TackRenderer.Instance.ResetCurrentTextureUnitIndex();

                    Shader connectedShader = rendererComp.Shader;

                    connectedShader.Use();

                    TackProfiler.Instance.StartTimer("Renderer.Loop.BuildMatrix");

                    // Generate model matrix
                    OpenTK.Matrix4 modelMatrix = GenerateModelMatrix(sortedObjects[i], camera, TackRenderer.Instance.CurrentSplitScreenMode, rendererComp.RenderLayer);

                    TackProfiler.Instance.StopTimer("Renderer.Loop.BuildMatrix");

                    connectedShader.SetUniformValue("uModelMat", modelMatrix.ToTEMat4());
                    //GL.UniformMatrix4(GL.GetUniformLocation(m_defaultWorldShader.Id, "uModelMat"), false, ref modelMatrix);

                    TackProfiler.Instance.StartTimer("Renderer.Loop.BindTextureData");

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, rendererComp.Sprite.Id);

                    TackRenderer.Instance.IncrementCurrentTextureUnitIndex();

                    if (rendererComp.Sprite.IsDynamic && rendererComp.Sprite.IsDirty)
                    {
                        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, rendererComp.Sprite.Width, rendererComp.Sprite.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, rendererComp.Sprite.Data);

                        rendererComp.Sprite.IsDirty = false;
                    }

                    TackProfiler.Instance.StopTimer("Renderer.Loop.BindTextureData");
                    TackProfiler.Instance.StartTimer("Renderer.Loop.SetUniformData");

                    connectedShader.SetUniformValue("uTexture", 0);

                    Vector4 colourVector = new Vector4(rendererComp.Colour.R / 255.0f, rendererComp.Colour.G / 255.0f, rendererComp.Colour.B / 255.0f, rendererComp.Colour.A / 255.0f);
                    connectedShader.SetUniformValue("uColour", colourVector);

                    foreach (KeyValuePair<string, object> uniformValue in rendererComp.ShaderUniformValues)
                    {
                        if (uniformValue.Value == null)
                        {
                            continue;
                        }

                        Type valueType = uniformValue.Value.GetType();

                        if (valueType == typeof(int))
                        {
                            connectedShader.SetUniformValue(uniformValue.Key, (int)uniformValue.Value);
                        }
                        else if (valueType == typeof(double))
                        {
                            connectedShader.SetUniformValue(uniformValue.Key, (double)uniformValue.Value);
                        }
                        else if (valueType == typeof(float))
                        {
                            connectedShader.SetUniformValue(uniformValue.Key, (float)uniformValue.Value);
                        }
                        else if (valueType == typeof(uint))
                        {
                            connectedShader.SetUniformValue(uniformValue.Key, (uint)uniformValue.Value);
                        }
                        else if (valueType == typeof(Matrix2))
                        {
                            connectedShader.SetUniformValue(uniformValue.Key, (Matrix2)uniformValue.Value);
                        }
                        else if (valueType == typeof(Matrix3))
                        {
                            connectedShader.SetUniformValue(uniformValue.Key, (Matrix3)uniformValue.Value);
                        }
                        else if (valueType == typeof(Matrix4))
                        {
                            connectedShader.SetUniformValue(uniformValue.Key, (Matrix4)uniformValue.Value);
                        }
                        else if (valueType == typeof(Vector2f))
                        {
                            connectedShader.SetUniformValue(uniformValue.Key, (Vector2f)uniformValue.Value);
                        }
                        else if (valueType == typeof(Vector3))
                        {
                            connectedShader.SetUniformValue(uniformValue.Key, (Vector3)uniformValue.Value);
                        }
                        else if (valueType == typeof(Vector4))
                        {
                            connectedShader.SetUniformValue(uniformValue.Key, (Vector4)uniformValue.Value);
                        }
                        else if (valueType == typeof(Sprite))
                        {
                            connectedShader.SetUniformValue(uniformValue.Key, (Sprite)uniformValue.Value);
                        }
                        else
                        {
                            TackConsole.EngineLog(TackConsole.LogType.Error, "Error: Type '" + valueType.Name + "' is not supported as a Shader Uniform Variable");
                        }
                    }

                    TackProfiler.Instance.StopTimer("Renderer.Loop.SetUniformData");
                    TackProfiler.Instance.StartTimer("Renderer.Loop.SetLightingData");

                    TackLightingSystem.Instance.Enabled = true;

                    if (TackLightingSystem.Instance.Enabled)
                    {
                        if (connectedShader.SupportsLighting)
                        {
                            if (!shadersLightingDataSet.Contains(connectedShader.Id))
                            {
                                GL.Uniform1(GL.GetUniformLocation(connectedShader.Id, connectedShader.LightingFragVariableName + ".ambientColourIntensity"), TackLightingSystem.Instance.AmbientLightIntensity);
                                GL.Uniform4(GL.GetUniformLocation(connectedShader.Id, connectedShader.LightingFragVariableName + ".ambientColour"), TackLightingSystem.Instance.AmbientLightColour.ToOpenTKVec4());

                                int indx = 0;

                                foreach (LightComponent light in lights)
                                {
                                    if (indx >= TackLightingSystem.Instance.MaxLights)
                                    {
                                        break;
                                    }

                                    GL.Uniform2(GL.GetUniformLocation(connectedShader.Id, connectedShader.LightingFragVariableName + ".lights" + "[" + indx + "].position"), light.GetParent().Position.ToOpenTKVec2());
                                    GL.Uniform4(GL.GetUniformLocation(connectedShader.Id, connectedShader.LightingFragVariableName + ".lights" + "[" + indx + "].colour"), light.Colour.ToOpenTKVec4());
                                    GL.Uniform1(GL.GetUniformLocation(connectedShader.Id, connectedShader.LightingFragVariableName + ".lights" + "[" + indx + "].intensity"), light.Intensity);
                                    GL.Uniform1(GL.GetUniformLocation(connectedShader.Id, connectedShader.LightingFragVariableName + ".lights" + "[" + indx + "].radius"), light.Radius);

                                    indx++;
                                }

                                GL.Uniform1(GL.GetUniformLocation(connectedShader.Id, connectedShader.LightingFragVariableName + ".lightCount"), lights.Length);

                                shadersLightingDataSet.Add(connectedShader.Id);
                            }
                        }
                    }

                    if (!shadersCamDataSet.Contains(connectedShader.Id))
                    {
                        // Set camera info
                        GL.Uniform2(GL.GetUniformLocation(connectedShader.Id, "uCameraInfo.position"), camera.GetParent().Position.ToOpenTKVec2());
                        GL.Uniform1(GL.GetUniformLocation(connectedShader.Id, "uCameraInfo.zoomFactor"), camera.ZoomFactor);
                        GL.Uniform2(GL.GetUniformLocation(connectedShader.Id, "uCameraInfo.size"), new OpenTK.Vector2(camera.RenderTarget.Width, camera.RenderTarget.Height));

                        shadersCamDataSet.Add(connectedShader.Id);
                    }

                    TackProfiler.Instance.StopTimer("Renderer.Loop.SetLightingData");

                    if (TackRenderer.Instance.CurrentSplitScreenMode != SplitScreenMode.Single)
                    {
                        connectedShader.SetUniformValue("uSplitScreenCamIndex", camIndex);
                    }

                    TackProfiler.Instance.StartTimer("Renderer.Loop.DrawCall");

                    unsafe
                    {
                        fixed (void* ptr = m_indiceData)
                        {
                            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, new IntPtr(ptr));
                        }
                    }

                    localDrawCallCount++;
                    TackProfiler.Instance.StopTimer("Renderer.Loop.DrawCall");

                    for (int cti = 0; cti < TackRenderer.Instance.CurrentTextureUnitIndex; cti++)
                    {
                        GL.ActiveTexture(TextureUnit.Texture0 + cti);
                        GL.BindTexture(TextureTarget.Texture2D, 0);
                    }
                }
            }

            GL.DeleteBuffers(1, ref VBO);

            drawCallCount = localDrawCallCount;

            GL.Disable(EnableCap.DepthTest);
        }

        public override void PostRender()
        {
        }

        private OpenTK.Matrix4 GenerateModelMatrix(TackObject obj, Camera camera, SplitScreenMode splitScreenMode, int renderLayer)
        {
            TackObject cameraObject = camera.GetParent();

            // Generate translation matrix
            OpenTK.Matrix4 transMat = OpenTK.Matrix4.CreateTranslation(obj.Position.X - cameraObject.Position.X, obj.Position.Y - cameraObject.Position.Y, TackMath.Clamp(renderLayer, 0, TackRenderer.MAX_RENDER_LAYER) / TackRenderer.MAX_RENDER_LAYER * -1f);

            // Generate scale matrix
            OpenTK.Matrix4 scaleMat = MatrixUtility.CreateScaleMatrix(obj.Size.X / 2.0f, obj.Size.Y / 2.0f, 1);

            // Generate rotation matrix
            OpenTK.Matrix4 rotationMat = OpenTK.Matrix4.CreateRotationZ((float)TackMath.DegToRad(obj.Rotation + cameraObject.Rotation));

            float camSizeModX = 2f;
            float camSizeModY = 2f;

            if (splitScreenMode == SplitScreenMode.DualScreen)
            {
                camSizeModX = 4f;
            }

            if (splitScreenMode == SplitScreenMode.QuadScreen)
            {
                camSizeModX = 4f;
                camSizeModY = 4f;
            }

            OpenTK.Matrix4 orthoView = new OpenTK.Matrix4(
                new OpenTK.Vector4(1 * Camera.MainCamera.ZoomFactor * (1f / (TackEngineInstance.Instance.Window.WindowSize.X / camSizeModX)), 0, 0, 0),
                new OpenTK.Vector4(0, 1 * Camera.MainCamera.ZoomFactor * (1f / (TackEngineInstance.Instance.Window.WindowSize.Y / camSizeModY)), 0, 0),
                new OpenTK.Vector4(0, 0, 1, 0),
                new OpenTK.Vector4(0, 0, 0, 1)
                );

            // Multiply the above matrices to generate the final model matrix
            OpenTK.Matrix4 modelMatrix = scaleMat * rotationMat * transMat * orthoView;

            return modelMatrix;
        }

        private void CheckForErrors(bool clear = false)
        {
            ErrorCode errorCode = GL.GetErrorCode();
            if (errorCode != ErrorCode.NoError)
            {
                if (!clear)
                {
                    throw new System.Exception(errorCode.ToString());
                }
            }
        }
    }
}
