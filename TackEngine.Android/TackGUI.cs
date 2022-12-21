/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using TackEngine.Core.Main;
using TackEngine.Core.Engine;
using TackEngine.Core.Renderer;
using TackEngine.Core.Input;
using TackEngine.Core.Renderer.Operations;
using TackEngine.Core.Objects.Components;
using OpenTK.Graphics.ES30;
using TackEngine.Core.GUI;
using TackEngine.Core.Physics;
using Android.Content.Res;
using Java.Security.Cert;
using System.Diagnostics.Tracing;
using Android.Graphics;
using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using System.Runtime.InteropServices;

namespace TackEngine.Android {
    /// <summary>
    /// The main class for rendering GUI elements to the screen
    /// </summary>
    public class TackGUI : BaseTackGUI {
        public TackGUI() {
            /*
            if (Instance != null) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "There is already an active instance of TackGUI.");
                return;
            }*/

            Instance = this;
        }

        internal override void OnStart() {

            //m_fontLibrary = new Library();
            m_mouseEventQueue = new List<TackEngine.Core.GUI.Events.GUIMouseEvent>();
            m_keyboardEventQueue = new List<TackEngine.Core.GUI.Events.GUIKeyboardEvent>();
            m_guiObjectsToRemove = new List<int>();
            m_preRenderQueue = new List<GUIObject>();
            m_preRenderCheckList = new List<GUIObject>();

            m_defaultFont = TackFont.LoadFromFile("tackresources/fonts/Roboto/Roboto-Regular.ttf");

            //TackConsole.EngineLog(TackConsole.LogType.Message, string.Format("Added default font file from: {0}\\Arial.ttf", Environment.GetFolderPath(Environment.SpecialFolder.Fonts)));

            m_guiOperations = new List<GUIOperation>();

            /*
            m_defaultGUIShader = new Shader("shaders.default_gui_shader", TackShaderType.GUI, System.IO.File.ReadAllText("tackresources/shaders/gui/default_gui_vertex_shader.vs"),
                                                                                              System.IO.File.ReadAllText("tackresources/shaders/gui/default_gui_fragment_shader.fs"));
            */

            // Load default shader
            using (StreamReader vertReader = new StreamReader(AndroidContext.CurrentAssetManager.Open("tackresources/shaders/gui/new/gui_vertex_shader.vs"))) {
                string vertSource = vertReader.ReadToEnd();

                using (StreamReader fragReader = new StreamReader(AndroidContext.CurrentAssetManager.Open("tackresources/shaders/gui/new/gui_fragment_shader.fs"))) {
                    string fragSource = fragReader.ReadToEnd();

                    m_defaultGUIShader = new Shader("shaders.new_gui_shader", TackShaderType.GUI, vertSource, fragSource);
                }
            }

            // Load default text shader
            using (StreamReader vertReader = new StreamReader(AndroidContext.CurrentAssetManager.Open("tackresources/shaders/gui/text/text_vertex_shader.vs"))) {
                string vertSource = vertReader.ReadToEnd();

                using (StreamReader fragReader = new StreamReader(AndroidContext.CurrentAssetManager.Open("tackresources/shaders/gui/text/text_fragment_shader.fs"))) {
                    string fragSource = fragReader.ReadToEnd();

                    m_defaultTextShader = new Shader("shaders.text_gui_shader", TackShaderType.GUI, vertSource, fragSource);
                }
            }
        }

        internal override void OnUpdate() {
            MouseClickDetectedThisFrame = false;

            // Remove all GUIObjects that were marked for deletion last frame
            CompleteGUObjectRemoval();

            // Deal with mouse inputs and focusing logic
            for (int i = 0; i < m_mouseEventQueue.Count; i++) {
                // Loop through all gui objects and send the mouse event to it
                int blankClickCount = 0;

                for (int j = 0; j < m_guiObjects.Count; j++) {
                    if (!m_guiObjects[j].Active) {
                        blankClickCount++;

                        if (blankClickCount >= m_guiObjects.Count) {
                            FocusGUIObject(null);
                        }

                        continue;
                    }

                    // Debugging
                    GUIObject debugObject = m_guiObjects[j];

                    RectangleShape shape = m_guiObjects[j].GetShapeWithMask();

                    TackEngine.Core.Physics.AABB guiObjAABB = new TackEngine.Core.Physics.AABB(new Vector2f(shape.X, shape.Y + shape.Height), new Vector2f(shape.X + shape.Width, shape.Y));

                    if (TackEngine.Core.Physics.AABB.IsPointInAABB(guiObjAABB, m_mouseEventQueue[i].Args.MousePosition.ToVector2f())) {
                        // If the action is down or held, only send the action if its inside a selectable area
                        m_guiObjects[j].OnMouseEvent(m_mouseEventQueue[i].Args);

                        if (m_mouseEventQueue[i].Args.MouseAction == MouseButtonAction.Up) {
                            FocusGUIObject(m_guiObjects[j]);
                        }

                        MouseClickDetectedThisFrame = true;
                    } else {
                        // If we are here that means a mouse event has occured inside a GUIObject
                        if (m_mouseEventQueue[i].Args.MouseAction == MouseButtonAction.Up) {
                            m_guiObjects[j].OnMouseEvent(m_mouseEventQueue[i].Args);

                            blankClickCount++;

                            // If we have checked every gui object and we haven't mouse up-ed on any of them,
                            //      de-focus all objects
                            if (blankClickCount >= m_guiObjects.Count) {
                                FocusGUIObject(null);
                            }
                        }
                    }

                }
            }

            m_mouseEventQueue.Clear();

            // Deal with keyboard key presses
            for (int i = 0; i < m_keyboardEventQueue.Count; i++) {
                for (int j = 0; j < m_guiObjects.Count; j++) {
                    if (m_guiObjects[j].Active) {
                        if (m_guiObjects[j].IsFocused) {
                            m_guiObjects[j].OnKeyboardEvent(m_keyboardEventQueue[i].Args);
                        }
                    }
                }
            }

            m_keyboardEventQueue.Clear();


            // Loop through the current GUIObject List calling the OnUpdate function on each
            for (int i = 0; i < m_guiObjects.Count; i++) {
                if (m_guiObjects[i].Active) {
                    m_guiObjects[i].OnUpdate();
                }
            }
        }

        internal override void OnGUIRender() {
            for (int i = 0; i < m_guiObjects.Count; i++) {
                if (m_guiObjects[i].ParentId == null) {
                    if (m_guiObjects[i].Active) {
                        m_guiObjects[i].OnRender(new GUIMaskData(new List<RectangleShape>()));
                    }
                }
            }
        }

        internal override void OnClose() {
            // Loop through the current GUIObject List calling the OnUpdate function on each
            for (int i = 0; i < m_guiObjects.Count; i++) {
                ((GUIObject)m_guiObjects[i]).OnClose();
            }
        }

        private void FocusGUIObject(GUIObject obj) {
            if (obj == null) {
                for (int i = 0; i < m_guiObjects.Count; i++) {
                    if (m_guiObjects[i].IsFocused) {
                        m_guiObjects[i].OnFocusLost();
                    }
                }

                return;
            }

            for (int i = 0; i < m_guiObjects.Count; i++) {
                if (obj.Id == m_guiObjects[i].Id) {
                    if (obj.Active) {
                        if (!m_guiObjects[i].IsFocused) {
                            m_guiObjects[i].OnFocusGained();
                        }
                    }
                } else {
                    if (m_guiObjects[i].IsFocused) {
                        m_guiObjects[i].OnFocusLost();
                    }
                }
            }
        }

        private void CompleteGUObjectRemoval() {
            m_guiObjects.RemoveAll(o => m_guiObjectsToRemove.Contains(o.Id));

            m_guiObjectsToRemove.Clear();
        }

        /// <summary>
        /// Loads a font file into the font collection. Returns the position of the new font family.
        /// </summary>
        /// <param name="_fileName"></param>
        /// <returns></returns>
        public static int LoadFontFromFile(string _fileName) {
            if (!File.Exists(_fileName)) {
                TackConsole.EngineLog(TackConsole.LogType.Error, string.Format("Could not locate file at path: {0}", _fileName));
                return -1;
            }

            //Instance.m_fontCollection.AddFontFile(_fileName);

            //TackConsole.EngineLog(TackConsole.LogType.Message, string.Format("Added new font with name: {0} to the TackGUI font collection at index: {1}", Instance.m_fontCollection.Families[Instance.m_fontCollection.Families.Length - 1].Name, Instance.m_fontCollection.Families.Length - 1));
            // return Instance.m_fontCollection.Families.Length - 1;
            return 0;
        }

        /// <summary>
        /// Sets the active FontFamily
        /// </summary>
        /// <param name="_familyName">the Name of the FontFamily</param>
        public static void SetActiveFontFamily(string _familyName) {
            /*
            for (int i = 0; i < Instance.m_fontCollection.Families.Length; i++) {
                if (Instance.m_fontCollection.Families[i].Name == _familyName) {
                    SetActiveFontFamily(i);
                    return;
                }
            }

            TackConsole.EngineLog(TackConsole.LogType.Error, string.Format("No FontFamily with name: {0} was found in the font collection", _familyName));
            */
        }

        /// <summary>
        /// Sets the active FontFamily
        /// </summary>
        /// <param name="_familyIndex">The index of the FontFamily</param>
        public static void SetActiveFontFamily(int _familyIndex) {
            /*
            if (_familyIndex < Instance.m_fontCollection.Families.Length) {
                Instance.m_activeFontFamily = Instance.m_fontCollection.Families[_familyIndex];

                TackConsole.EngineLog(TackConsole.LogType.Message, "Set the active FontFamily. Name: {0}, FamilyIndex: {1}", Instance.m_activeFontFamily.Name, _familyIndex);
            }

            TackConsole.EngineLog(TackConsole.LogType.Error, "The specfied family index is outside the bounds of the font collection Families array");
            */
        }

        internal override void InternalBorder(RectangleShape objectBounds, GUIBorder border) {

            // todo
        }

        /// <summary>
        /// Draws a box on the screen
        /// </summary>
        /// <param name="rect">The shape (Position and size) of the box</param>
        /// <param name="style">The BoxStyle used to render this box</param>
        /// <param name="negMaskRect">The area that pixels are allowed to rendered in. If pixels are outside of this bounds, they are not rendered</param>
        internal override void InternalBox(RectangleShape rect, GUIBox.GUIBoxStyle style, GUIMaskData maskData) {
            if (style.Border != null) {
                InternalBorder(rect, style.Border);
            }

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            int posHandle = GL.GetAttribLocation(m_defaultGUIShader.Id, "aPos");
            int uvHandle = GL.GetAttribLocation(m_defaultGUIShader.Id, "aTexCoord");

            GL.UseProgram(m_defaultGUIShader.Id);

            //GL.EnableVertexAttribArray(0);

            // Call 1 or more draw element calls depending on a maxGUIDrawCallAmount variable 

            float[] vertexData = new float[20] {
                    //       Position (XYZ)                                                                                                      Colours (RGB)                                                                                  TexCoords (XY)
                    /* v1 */  1f, -1f, 1.0f,       1.0f, 1.0f,
                    /* v2 */  1f,  1f, 1.0f,       1.0f, 0.0f,
                    /* v3 */ -1f,  1f, 1.0f,       0.0f, 0.0f,
                    /* v4 */ -1f, -1f, 1.0f,       0.0f, 1.0f
            };

            int[] indiceData = new int[] {
                    0, 1, 3, // first triangle 
                    1, 2, 3  // second triangle - 1, 2, 3
            };

            GL.GenBuffers(1, out int VBO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * 20), vertexData, BufferUsage.StaticDraw);

            // position attribute
            GL.VertexAttribPointer(posHandle, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(posHandle);

            // uv attribute
            GL.VertexAttribPointer(uvHandle, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(uvHandle);

            // Set texture attributes
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, style.Texture.Id);

            // set texture filtering parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // set the texture wrapping parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, style.Texture.Width, style.Texture.Height, 0, OpenTK.Graphics.ES30.PixelFormat.Rgba, PixelType.UnsignedByte, style.Texture.Data);
            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            m_defaultGUIShader.Use();

            // Generate translation matrix
            OpenTK.Matrix4 transMat = OpenTK.Matrix4.CreateTranslation(rect.X + (rect.Width / 2.0f), -rect.Y - (rect.Height / 2.0f), 0);

            // Generate scale matrix
            OpenTK.Matrix4 scaleMat = MatrixUtility.CreateScaleMatrix((rect.Width / 2.0f), (rect.Height / 2.0f), 1);

            // Generate the view matrix
            float widthToHeightRatio = TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.Y / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.X;

            OpenTK.Matrix4 orthoView = new OpenTK.Matrix4(
                new OpenTK.Vector4(2.0f / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.X, 0, 0, 0),
                new OpenTK.Vector4(0, 2.0f / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.Y, 0, 0),
                new OpenTK.Vector4(0, 0, 1, 0),
                new OpenTK.Vector4(0, 0, 0, 1)
                );

            // Multiply the above matrices to generate the final model matrix
            OpenTK.Matrix4 modelMatrix = ((scaleMat) * transMat) * orthoView;

            GL.UniformMatrix4(GL.GetUniformLocation(TackGUI.Instance.DefaultGUIShader.Id, "uModelMat"), false, ref modelMatrix);
            GL.Uniform1(GL.GetUniformLocation(TackGUI.Instance.DefaultGUIShader.Id, "uTexture"), 0);
            GL.Uniform4(GL.GetUniformLocation(TackGUI.Instance.DefaultGUIShader.Id, "uColour"), style.Colour.R / 255f, style.Colour.G / 255f, style.Colour.B / 255f, style.Colour.A / 255f); ;

            if (style.Texture.IsNineSliced && style.Texture.NineSlicedData != null) {
                float slice = style.Texture.NineSlicedData.BorderSize;

                GL.Uniform2(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uSliceDimensions"), new Vector2f(slice / rect.Width, slice / rect.Height).ToOpenTKVec2());
                GL.Uniform2(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uSliceBorder"), new Vector2f(slice / style.Texture.Width, slice / style.Texture.Height).ToOpenTKVec2());
                GL.Uniform1(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uIsNineSlicedTexture"), 1);
            } else {
                GL.Uniform1(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uIsNineSlicedTexture"), 0);
            }

            // Set camera info
            GL.Uniform2(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uCameraInfo.position"), TackEngine.Core.Objects.Components.Camera.MainCamera.GetParent().Position.ToOpenTKVec2());
            GL.Uniform1(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uCameraInfo.zoomFactor"), TackEngine.Core.Objects.Components.Camera.MainCamera.ZoomFactor);
            GL.Uniform2(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uCameraInfo.size"), new OpenTK.Vector2(TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Width, TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Height));

            for (int m = 0; m < maskData.Masks.Count; m++) {
                OpenTK.Vector2 topRight = new OpenTK.Vector2(
                    ((maskData.Masks[m].X + maskData.Masks[m].Width) / (TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Width / 2f)) - 1f,
                    (((maskData.Masks[m].Y) / (TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Height / 2f)) - 1f));

                OpenTK.Vector2 bottomLeft = new OpenTK.Vector2(
                    ((maskData.Masks[m].X) / (TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Width / 2f)) - 1f,
                    ((maskData.Masks[m].Y + maskData.Masks[m].Height) / (TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Height / 2f)) - 1f);

                GL.Uniform2(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uMaskList[" + m + "].topRight"), topRight);
                GL.Uniform2(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uMaskList[" + m + "].bottomLeft"), bottomLeft);
            }

            GL.Uniform1(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uMaskCount"), maskData.Masks.Count);

            unsafe {
                fixed (void* ptr = indiceData) {
                    GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, new IntPtr(ptr));
                }
            }

            GL.DeleteBuffers(1, ref VBO);
        }

        /// <summary>
        /// Renders text to the screen
        /// </summary>
        /// <param name="rect">The shape of box to render the text in</param>
        /// <param name="text">The text to render</param>
        /// <param name="style">The TextAreaStyle used to render this text</param>
        internal override void InternalTextArea(RectangleShape rect, string text, GUITextArea.GUITextAreaStyle style, Vector2f scrollPosition, GUIMaskData maskData, int caretCharacterIndex) {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            int posHandle = GL.GetAttribLocation(m_defaultGUIShader.Id, "aPos");
            int uvHandle = GL.GetAttribLocation(m_defaultGUIShader.Id, "aTexCoord");

            GL.UseProgram(m_defaultGUIShader.Id);

            //GL.EnableVertexAttribArray(0);

            // Call 1 or more draw element calls depending on a maxGUIDrawCallAmount variable 

            float[] vertexData = new float[20] {
                    //       Position (XYZ)                                                                                                      Colours (RGB)                                                                                  TexCoords (XY)
                    /* v1 */  1f, -1f, 1.0f,       1.0f, 1.0f,
                    /* v2 */  1f,  1f, 1.0f,       1.0f, 0.0f,
                    /* v3 */ -1f,  1f, 1.0f,       0.0f, 0.0f,
                    /* v4 */ -1f, -1f, 1.0f,       0.0f, 1.0f
            };

            int[] indiceData = new int[] {
                    0, 1, 3, // first triangle 
                    1, 2, 3  // second triangle - 1, 2, 3
            };

            GL.GenBuffers(1, out int VBO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * 20), vertexData, BufferUsage.StaticDraw);

            // position attribute
            GL.VertexAttribPointer(posHandle, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(posHandle);

            // uv attribute
            GL.VertexAttribPointer(uvHandle, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(uvHandle);

            float padding = 5f;

            TackFont font = style.Font;

            // If a font hasn't been set, use the default GUI font
            if (font == null) {
                font = Instance.DefaultFont;
            }

            Vector2f totalTextSize = MeasureStringSize(text, font, style.FontSize, rect);

            Vector2f minPos = new Vector2f(rect.X + padding, rect.Y + padding);

            // If the horizontal alignment is in the middle, set the char_x offset
            if (style.HorizontalAlignment == HorizontalAlignment.Middle) {
                float widthDelta = rect.Width - totalTextSize.X;

                minPos.X = rect.X + (widthDelta / 2.0f);
            }

            // If the horizontal alignment is on the right, set the char_x offset
            if (style.HorizontalAlignment == HorizontalAlignment.Right) {
                float widthDelta = rect.Width - totalTextSize.X;

                minPos.X = rect.X + widthDelta;
            }

            // If the vertical alignment is in the middle, set the char_y offset
            if (style.VerticalAlignment == VerticalAlignment.Middle) {
                float heightDelta = rect.Height - totalTextSize.Y;

                minPos.Y = rect.Y + (heightDelta / 2f);
            }

            // If the vertical alignment is at the bottom, set the char_y offset
            if (style.VerticalAlignment == VerticalAlignment.Bottom) {
                float heightDelta = rect.Height - totalTextSize.Y;

                minPos.Y = rect.Y + heightDelta;
            }

            maskData.AddMask(rect);

            Vector2f charPos = new Vector2f(minPos.X, minPos.Y);

            for (int i = 0; i < text.Length; i++) {
                if (text[i] == '\n') {
                    charPos.Y += style.Font.GetFontCharacter('B').size.Y;
                    charPos.X = minPos.X;
                    continue;
                }

                TackFont.FontCharacter fchar = style.Font.GetFontCharacter(text[i]);

                // set default (4 byte) pixel alignment 
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.ActiveTexture(TextureUnit.Texture0);

                // Set texture attributes
                //GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, fchar.texId);
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

                m_defaultGUIShader.Use();

                // Generate translation matrix
                OpenTK.Matrix4 transMat = OpenTK.Matrix4.CreateTranslation(charPos.X + (fchar.size.X / 2.0f), -charPos.Y - (fchar.size.Y / 2.0f), 0);

                // Generate scale matrix
                OpenTK.Matrix4 scaleMat = MatrixUtility.CreateScaleMatrix((fchar.size.X / 2.0f), (fchar.size.Y / 2.0f), 1);

                // Generate the view matrix
                float widthToHeightRatio = TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.Y / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.X;

                OpenTK.Matrix4 orthoView = new OpenTK.Matrix4(
                    new OpenTK.Vector4(2.0f / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.X, 0, 0, 0),
                    new OpenTK.Vector4(0, 2.0f / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.Y, 0, 0),
                    new OpenTK.Vector4(0, 0, 1, 0),
                    new OpenTK.Vector4(0, 0, 0, 1)
                    );

                // Multiply the above matrices to generate the final model matrix
                OpenTK.Matrix4 modelMatrix = ((scaleMat) * transMat) * orthoView;

                GL.UniformMatrix4(GL.GetUniformLocation(TackGUI.Instance.DefaultGUIShader.Id, "uModelMat"), false, ref modelMatrix);
                GL.Uniform1(GL.GetUniformLocation(TackGUI.Instance.DefaultGUIShader.Id, "uTexture"), 0);
                GL.Uniform4(GL.GetUniformLocation(TackGUI.Instance.DefaultGUIShader.Id, "uColour"), style.FontColour.R / 255f, style.FontColour.G / 255f, style.FontColour.B / 255f, style.FontColour.A / 255f); ;

                if (style.Texture.IsNineSliced && style.Texture.NineSlicedData != null) {
                    float slice = style.Texture.NineSlicedData.BorderSize;

                    GL.Uniform2(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uSliceDimensions"), new Vector2f(slice / rect.Width, slice / rect.Height).ToOpenTKVec2());
                    GL.Uniform2(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uSliceBorder"), new Vector2f(slice / style.Texture.Width, slice / style.Texture.Height).ToOpenTKVec2());
                    GL.Uniform1(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uIsNineSlicedTexture"), 1);
                } else {
                    GL.Uniform1(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uIsNineSlicedTexture"), 0);
                }

                // Set camera info
                GL.Uniform2(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uCameraInfo.position"), TackEngine.Core.Objects.Components.Camera.MainCamera.GetParent().Position.ToOpenTKVec2());
                GL.Uniform1(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uCameraInfo.zoomFactor"), TackEngine.Core.Objects.Components.Camera.MainCamera.ZoomFactor);
                GL.Uniform2(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uCameraInfo.size"), new OpenTK.Vector2(TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Width, TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Height));

                for (int m = 0; m < maskData.Masks.Count; m++) {
                    OpenTK.Vector2 topRight = new OpenTK.Vector2(
                        ((maskData.Masks[m].X + maskData.Masks[m].Width) / (TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Width / 2f)) - 1f,
                        (((maskData.Masks[m].Y) / (TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Height / 2f)) - 1f));

                    OpenTK.Vector2 bottomLeft = new OpenTK.Vector2(
                        ((maskData.Masks[m].X) / (TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Width / 2f)) - 1f,
                        ((maskData.Masks[m].Y + maskData.Masks[m].Height) / (TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Height / 2f)) - 1f);

                    GL.Uniform2(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uMaskList[" + m + "].topRight"), topRight);
                    GL.Uniform2(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uMaskList[" + m + "].bottomLeft"), bottomLeft);
                }

                GL.Uniform1(GL.GetUniformLocation(Instance.DefaultGUIShader.Id, "uMaskCount"), maskData.Masks.Count);

                unsafe {
                    fixed (void* ptr = indiceData) {
                        GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, new IntPtr(ptr));
                    }
                }

                charPos.X += fchar.size.X;
            }

            GL.DeleteBuffers(1, ref VBO);

            /*
             * --------------------------------- old
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            int posHandle = GL.GetAttribLocation(m_defaultTextShader.Id, "aPos");
            int uvHandle = GL.GetAttribLocation(m_defaultTextShader.Id, "aTexCoord");

            GL.UseProgram(m_defaultTextShader.Id);

            //GL.EnableVertexAttribArray(0);

            // Call 1 or more draw element calls depending on a maxGUIDrawCallAmount variable 

            float[] vertexData = new float[20] {
                    //       Position (XYZ)                                                                                                      Colours (RGB)                                                                                  TexCoords (XY)
                      1f, -1f, 1.0f,       1.0f, 1.0f,
                      1f,  1f, 1.0f,       1.0f, 0.0f,
                    -1f,  1f, 1.0f,       0.0f, 0.0f,
                     -1f, -1f, 1.0f,       0.0f, 1.0f
            };

            int[] indiceData = new int[] {
                    0, 1, 3, // first triangle 
                    1, 2, 3  // second triangle - 1, 2, 3
            };

            GL.GenBuffers(1, out int VBO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * 20), vertexData, BufferUsage.StaticDraw);

            // position attribute
            GL.VertexAttribPointer(posHandle, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(posHandle);

            // uv attribute
            GL.VertexAttribPointer(uvHandle, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(uvHandle);

            float padding = 5f;
            float char_x = padding;

            float finalFontSize = style.FontSize / 30.0f;

            float lineSpacing = 50 * finalFontSize;
            float char_y = lineSpacing + padding + (scrollPosition.Y);

            TackFont font = style.Font;

            // If a font hasn't been set, use the default GUI font
            if (font == null) {
                font = Instance.DefaultFont;
            }

            Vector2f totalTextSize = MeasureStringSize(text, font, style.FontSize, rect);

            // If the horizontal alignment is in the middle, set the char_x offset
            if (style.HorizontalAlignment == HorizontalAlignment.Middle) {
                float widthDelta = rect.Width - totalTextSize.X;

                char_x += (widthDelta / 2.0f);
            }

            // If the horizontal alignment is on the right, set the char_x offset
            if (style.HorizontalAlignment == HorizontalAlignment.Right) {
                float widthDelta = rect.Width - totalTextSize.X;

                char_x += widthDelta - (padding * 2);
            }

            // If the vertical alignment is in the middle, set the char_y offset
            if (style.VerticalAlignment == VerticalAlignment.Middle) {
                float heightDelta = rect.Height - totalTextSize.Y;

                char_y += ((heightDelta / 2.0f) - (lineSpacing / 2) + (padding / 2.0f));
            }

            // If the vertical alignment is at the bottom, set the char_y offset
            if (style.VerticalAlignment == VerticalAlignment.Bottom) {
                float heightDelta = rect.Height - totalTextSize.Y;

                char_y += heightDelta - ((lineSpacing + padding) / 1.0f);
            }

            maskData.AddMask(rect);

            for (int i = 0; i < text.Length; i++) {
                if (text[i] == '\n') {
                    char_y += lineSpacing;
                    char_x = padding;
                    continue;
                }

                // If the current character is a space, detect if we can fit the next word on the current line
                // If not, drop a line
                if (text[i] == ' ') {
                    int indexOfNextSpace = text.IndexOfAny(new char[] { ' ' }, i + 1);

                    if (indexOfNextSpace == -1) {
                        indexOfNextSpace = text.Length - 1;
                    }

                    string subStr = text.Substring(i, indexOfNextSpace - i);
                    float lengthOfNextWord = MeasureStringLength(subStr, font, finalFontSize);


                    if (char_x + lengthOfNextWord > (rect.Width - (padding * 2))) {
                        char_y += lineSpacing;
                        char_x = padding;
                        continue;
                    }
                }

                if (caretCharacterIndex == i) {
                    int prevIndx = i - 1;
                    RectangleShape shape;

                    float caretY = rect.Y + char_y - (lineSpacing * 0.85f);

                    if (prevIndx > -1) {
                        float prevCharAdvance = ((int)font.GetFontCharacter(text[prevIndx]).advance >> 6) * finalFontSize;

                        //float nextCharBearing = 
                        shape = new RectangleShape(
                               rect.X + char_x,
                               caretY,
                               0.3f / finalFontSize,
                               font.GetFontCharacter((uint)'B').size.Y * finalFontSize * 1.4f);
                    } else {
                        shape = new RectangleShape(
                              rect.X + char_x,
                              caretY,
                              0.3f / finalFontSize,
                              font.GetFontCharacter((uint)'B').size.Y * finalFontSize * 1.4f);
                    }

                    InternalBox(shape, new GUIBox.GUIBoxStyle() { Border = null, Colour = Colour4b.Black, Texture = Sprite.DefaultSprite }, maskData);
                }

                TackFont.FontCharacter ch = font.GetFontCharacter(text[i]);

                if (ch.texId == -1) {
                    continue;
                }

                float w = ch.size.X * finalFontSize;
                float h = ch.size.Y * finalFontSize;
                float xrel = char_x + ch.bearing.X * finalFontSize;
                float yrel = ch.bearing.Y * finalFontSize;
                float characterSpacing = 5 * finalFontSize;

                bool wordSplit = false;

                // If true, the word is too long to fit on a single line so we must break it up with a hyphen
                if (char_x + (((int)ch.advance >> 6) * finalFontSize) > (rect.Width - (padding * 2))){
                    // If here and i == 0 (first letter of the string), then nothing will fit in text area
                    // We set i to a huge number, this will exit out of the for loop and not attempt to render any more letters
                    if (i == 0) {
                        i = int.MaxValue;
                        break;
                    }

                    ch = font.GetFontCharacter('-');

                    w = ch.size.X * finalFontSize;
                    h = ch.size.Y * finalFontSize;
                    xrel = char_x + ch.bearing.X * finalFontSize;
                    yrel = ch.bearing.Y * finalFontSize;

                    wordSplit = true;
                }

                // set default (4 byte) pixel alignment 
                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.ActiveTexture(TextureUnit.Texture0);

                // Set texture attributes
                //GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, style.Font.GetFontCharacter(text[i]).texId);
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

                //GetInstance().DefaultGUIShader.Use();
                Instance.DefaultTextShader.Use();

                // Generate translation matrix
                OpenTK.Matrix4 transMat = OpenTK.Matrix4.CreateTranslation(rect.X + xrel + (w / 2.0f), -rect.Y + yrel - char_y - (h / 2.0f), 0);

                // Generate scale matrix
                OpenTK.Matrix4 scaleMat = MatrixUtility.CreateScaleMatrix((w / 2.0f), (h / 2.0f), 1);

                // Generate the view matrix
                OpenTK.Matrix4 orthoView = new OpenTK.Matrix4(
                    new OpenTK.Vector4(2.0f / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.X, 0, 0, 0),
                    new OpenTK.Vector4(0, 2.0f / (float)TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.Y, 0, 0),
                    new OpenTK.Vector4(0, 0, 1, 0),
                    new OpenTK.Vector4(0, 0, 0, 1)
                    );

                // Multiply the above matrices to generate the final model matrix
                OpenTK.Matrix4 modelMatrix = ((scaleMat) * transMat) * orthoView;

                GL.UniformMatrix4(GL.GetUniformLocation(TackGUI.Instance.DefaultTextShader.Id, "uModelMat"), false, ref modelMatrix);
                GL.Uniform1(GL.GetUniformLocation(TackGUI.Instance.DefaultTextShader.Id, "uTexture"), 0);
                GL.Uniform4(GL.GetUniformLocation(TackGUI.Instance.DefaultTextShader.Id, "uColour"), style.FontColour.R / 255f, style.FontColour.G / 255f, style.FontColour.B / 255f, style.FontColour.A / 255f); ;

                // Set camera info
                GL.Uniform2(GL.GetUniformLocation(TackGUI.Instance.DefaultTextShader.Id, "uCameraInfo.position"), TackEngine.Core.Objects.Components.Camera.MainCamera.GetParent().Position.ToOpenTKVec2());
                GL.Uniform1(GL.GetUniformLocation(TackGUI.Instance.DefaultTextShader.Id, "uCameraInfo.zoomFactor"), TackEngine.Core.Objects.Components.Camera.MainCamera.ZoomFactor);
                GL.Uniform2(GL.GetUniformLocation(TackGUI.Instance.DefaultTextShader.Id, "uCameraInfo.size"), new OpenTK.Vector2(TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Width, TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Height));

                for (int m = 0; m < maskData.Masks.Count; m++) {
                    OpenTK.Vector2 topRight = new OpenTK.Vector2(
                        ((maskData.Masks[m].X + maskData.Masks[m].Width) / (TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Width / 2f)) - 1f,
                        (((maskData.Masks[m].Y) / (TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Height / 2f)) - 1f));

                    OpenTK.Vector2 bottomLeft = new OpenTK.Vector2(
                        ((maskData.Masks[m].X) / (TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Width / 2f)) - 1f,
                        ((maskData.Masks[m].Y + maskData.Masks[m].Height) / (TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget.Height / 2f)) - 1f);

                    GL.Uniform2(GL.GetUniformLocation(TackGUI.Instance.DefaultTextShader.Id, "uMaskList[" + m + "].topRight"), topRight);
                    GL.Uniform2(GL.GetUniformLocation(TackGUI.Instance.DefaultTextShader.Id, "uMaskList[" + m + "].bottomLeft"), bottomLeft);
                }

                GL.Uniform1(GL.GetUniformLocation(TackGUI.Instance.DefaultTextShader.Id, "uMaskCount"), maskData.Masks.Count);

                unsafe {
                    fixed (void* ptr = indiceData) {
                        GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, new IntPtr(ptr));
                    }
                }

                if (!wordSplit) {
                    char_x += (((int)ch.advance >> 6) * finalFontSize) * 1.05f; // Bitshift by 6 to get value in pixels (2^6 = 64 (divide amount of 1/64th pixels by 64 to get amount of pixels))
                } else {
                    char_y += lineSpacing;
                    char_x = padding;
                    i--;
                }
            }

            if (caretCharacterIndex == text.Length) {
                int prevIndx = text.Length - 1;
                RectangleShape shape;

                float caretY = rect.Y + char_y - (lineSpacing * 0.85f);

                shape = new RectangleShape(
                          rect.X + char_x,
                          caretY,
                          0.3f / finalFontSize,
                          font.GetFontCharacter((uint)'B').size.Y * finalFontSize * 1.4f);

                InternalBox(shape, new GUIBox.GUIBoxStyle() { Border = null, Colour = Colour4b.Black, Texture = Sprite.DefaultSprite }, maskData);
            }

            GL.DeleteBuffers(1, ref VBO);
            */
        }

        private static float MeasureStringLength(string text, TackFont font, float finalFontSize) {
            float length = 0;

            for (int i = 0; i < text.Length; i++) {
                if (text[i] == '\n' || text[i] == '\r') {
                    return length;
                }

                TackFont.FontCharacter ch = font.GetFontCharacter(text[i]);

                length += ch.size.X;
            }

            return length;
        }

        internal static Vector2f MeasureStringSize(string text, TackFont font, float fontSize, RectangleShape rect) {
            float padding = 5f;
            float char_x = padding;
            float largestCharX = 0;

            float finalFontSize = fontSize / 30.0f;

            float lineSpacing = 50 * finalFontSize;
            float char_y = lineSpacing + padding;

            for (int i = 0; i < text.Length; i++) {
                if (text[i] == '\n') {
                    char_y += lineSpacing;

                    if (largestCharX < char_x) {
                        largestCharX = char_x;
                    }

                    char_x = padding;
                    continue;
                }

                // If the current character is a space, detect if we can fit the next word on the current line
                // If not, drop a line
                if (text[i] == ' ') {
                    int indexOfNextSpace = text.IndexOfAny(new char[] { ' ' }, i + 1);

                    if (indexOfNextSpace == -1) {
                        indexOfNextSpace = text.Length - 1;
                    }

                    string subStr = text.Substring(i, indexOfNextSpace - i);
                    float lengthOfNextWord = MeasureStringLength(subStr, font, finalFontSize);


                    if (char_x + lengthOfNextWord > (rect.Width - (padding * 4))) {
                        char_y += lineSpacing;

                        if (largestCharX < char_x) {
                            largestCharX = char_x;
                        }

                        char_x = padding;
                        continue;
                    }
                }

                TackFont.FontCharacter ch = font.GetFontCharacter(text[i]);

                if (ch.texId == -1) {
                    continue;
                }

                float w = ch.size.X * finalFontSize;
                float h = ch.size.Y * finalFontSize;
                float xrel = char_x + ch.bearing.X * finalFontSize;
                float yrel = ch.bearing.Y * finalFontSize;
                float characterSpacing = 5 * finalFontSize;

                bool wordSplit = false;

                // If true, the word is too long to fit on a single line so we must break it up with a hyphen
                if (char_x + (((int)ch.advance >> 6) * finalFontSize) > (rect.Width - (padding * 4))) {
                    ch = font.GetFontCharacter('-');

                    w = ch.size.X * finalFontSize;
                    h = ch.size.Y * finalFontSize;
                    xrel = char_x + ch.bearing.X * finalFontSize;
                    yrel = ch.bearing.Y * finalFontSize;

                    wordSplit = true;
                }

                if (!wordSplit) {
                    char_x += (((int)ch.advance >> 6) * finalFontSize) * 1.05f; // Bitshift by 6 to get value in pixels (2^6 = 64 (divide amount of 1/64th pixels by 64 to get amount of pixels))
                } else {
                    char_y += lineSpacing;

                    char_x = padding;
                    i--;
                }

                if (largestCharX < char_x) {
                    largestCharX = char_x;
                }
            }

            return new Vector2f(largestCharX, char_y);
        }

        /*
        public static string InputField(RectangleShape rect, string textToRender, ref InputFieldStyle style) {
            if (style == null) {
                style = new InputFieldStyle();
            }

            // Instead of calling TextArea(), just run the dup code from the method, cause lazy
            //TextArea(rect, textToRender, style.GetTextStyle());

            Bitmap textBitmap = new Bitmap((int)rect.Width, (int)rect.Height);
            Graphics g = Graphics.FromImage(textBitmap);
            g.FillRectangle(ActiveInstance.GetColouredBrush(style.BackgroundColour), 0, 0, rect.Width, rect.Height);
            g.DrawString(textToRender, new Font(GetFontFamily(style.FontFamilyId), style.FontSize, FontStyle.Regular), ActiveInstance.GetColouredBrush(style.FontColour), new Rectangle(0, 0, (int)rect.Width, (int)rect.Height), ActiveInstance.GenerateTextFormat(style.HorizontalAlignment, style.VerticalAlignment));

            Sprite textSprite = Sprite.LoadFromBitmap(textBitmap);
            textSprite.Create(false);

            GUIOperation operation = new GUIOperation(1, 2);
            operation.Bounds = rect;
            operation.DrawLevel = 1;
            operation.Sprite = textSprite;
            operation.Colour = Colour4b.White;

            g.Dispose();
            textBitmap.Dispose();

            ActiveInstance.m_guiOperations.Add(operation);
            // Finished creating text operation

            bool registeredADownClick = false;

            //Console.WriteLine("{0} mouse events on ({1})", ActiveInstance.m_currentMouseEvents.Count, TackEngine.RenderCycleCount);

            for (int i = 0; i < ActiveInstance.m_currentMouseEvents.Count; i++) {
                //Console.WriteLine("Pos: {0}, X: {1}, X + Width: {2}", ActiveInstance.m_currentMouseEvents[i].Position.X, rect.X, (rect.X + rect.Width));
                if (ActiveInstance.m_currentMouseEvents[i].Position.X >= rect.X && ActiveInstance.m_currentMouseEvents[i].Position.X <= (rect.X + rect.Width)) {
                    //Console.WriteLine("Is X");
                    if (ActiveInstance.m_currentMouseEvents[i].Position.Y >= rect.Y && ActiveInstance.m_currentMouseEvents[i].Position.Y <= (rect.Y + rect.Height)) {
                        if (ActiveInstance.m_currentMouseEvents[i].EventType == 0) {
                            registeredADownClick = true;
                            Console.WriteLine("Found a GUI mouse event of type: {0} involving the InputField", ActiveInstance.m_currentMouseEvents[i].EventType);
                        }
                    }
                }
            }

            if (registeredADownClick) {
                TackInput.GUIInputRequired = true;
            } else if (!registeredADownClick && ActiveInstance.m_currentMouseEvents.Count(x => x.EventType == 0) > 0) {
                Console.WriteLine("Found that a mouse down happened outside this inputfield");
                TackInput.GUIInputRequired = false;
            }

            KeyboardKey[] bufferOperations = TackInput.GetInputBufferArray();
            string newString;
            
            if (textToRender == null) {
                newString = "";
            } else {
                newString = textToRender;
            }

            for (int i = 0; i < bufferOperations.Length; i++) {
                if (bufferOperations[i] == KeyboardKey.Left) {
                    if (style.CaretPosition > 0) {
                        style.CaretPosition -= 1;
                    }
                } else if (bufferOperations[i] == KeyboardKey.Right) {
                    if (style.CaretPosition < newString.Length) {
                        style.CaretPosition += 1;
                    }
                } else if (bufferOperations[i] == KeyboardKey.BackSpace) {
                    if (style.CaretPosition > 0) {
                        newString = newString.Remove((int)style.CaretPosition - 1, 1);
                    }

                    if (style.CaretPosition > 0) {
                        style.CaretPosition -= 1;
                    }
                } else if (bufferOperations[i] == KeyboardKey.Delete) {
                    
                    if (style.CaretPosition < newString.Length) {
                        newString = newString.Remove((int)style.CaretPosition, 1);
                    }
                } else if (bufferOperations[i] == KeyboardKey.Space) {
                    newString = newString.Insert((int)style.CaretPosition, " ");

                    if (style.CaretPosition < newString.Length) {
                        style.CaretPosition += 1;
                    }
                } else if (bufferOperations[i] == KeyboardKey.Period) {
                    newString = newString.Insert((int)style.CaretPosition, ".");

                    if (style.CaretPosition < newString.Length) {
                        style.CaretPosition += 1;
                    }
                } else if (bufferOperations[i] == KeyboardKey.Quote) {
                    newString = newString.Insert((int)style.CaretPosition, "\"");

                    if (style.CaretPosition < newString.Length) {
                        style.CaretPosition += 1;
                    }
                } else if (bufferOperations[i] == KeyboardKey.Minus) {
                    if (TackInput.InputBufferShift) {
                        newString = newString.Insert((int)style.CaretPosition, "_");
                    } else {
                        newString = newString.Insert((int)style.CaretPosition, "-");
                    }

                    if (style.CaretPosition < newString.Length) {
                        style.CaretPosition += 1;
                    }
                }
                
                else if (bufferOperations[i] >= KeyboardKey.Number0 && bufferOperations[i] <= KeyboardKey.Number9) {
                    newString = newString.Insert((int)style.CaretPosition, ((char)((int)bufferOperations[i] - 61)).ToString());

                    if (style.CaretPosition < newString.Length) {
                        style.CaretPosition += 1;
                    }
                } else if (bufferOperations[i] >= KeyboardKey.A && bufferOperations[i] <= KeyboardKey.Z) {
                    if (TackInput.InputBufferCapsLock || TackInput.InputBufferShift) {
                        newString = newString.Insert((int)style.CaretPosition, ((char)((int)bufferOperations[i] - 18)).ToString());
                    } else {
                        newString = newString.Insert((int)style.CaretPosition, ((char)((int)bufferOperations[i] + 14)).ToString());
                    }

                    if (style.CaretPosition < newString.Length) {
                        style.CaretPosition += 1;
                    }
                }
            }

            TackInput.ClearInputBuffer();

            return newString;
        }*/

        public static int GetFontFamilyId(string familyName) {
            if (Instance == null) {
                return 0;
            }
            
            /*
            for (int i = 0; i < Instance.m_fontCollection.Families.Length; i++)
            {
                if (Instance.m_fontCollection.Families[i].Name == familyName) {
                    return i;
                }
            }*/

            TackConsole.EngineLog(TackConsole.LogType.Error, string.Format("No FontFamily with name: {0} was found in the font collection", familyName));
            return -1;
        }

        /// <summary>
        /// Gets the FontFamily at a specified index
        /// </summary>
        /// <param name="fontId">The index of the FontFamily in the collection</param>
        /// <returns></returns>
        /*
        public static FontFamily GetFontFamily(int fontId) {
            
            if (fontId < Instance.m_fontCollection.Families.Length) {
                return Instance.m_fontCollection.Families[fontId];
            }

            return Instance.m_fontCollection.Families[0];
            

            return null;
        }*/

        private static string GetRenderableString(string aString, float aScrollPos, float aHeightPerLine, float aTextAreaHeight) {
            if (string.IsNullOrEmpty(aString)) {
                return "";
            }

            // Split strings by lines
            string[] splitString = aString.Split(new char[] { '\n', '\r' });

            int maxLines = (int)(aTextAreaHeight / aHeightPerLine);

            Console.WriteLine("Height: {0}, HeightPerLine: {1}, Lines: {2}", aTextAreaHeight, aHeightPerLine, maxLines);

            if (splitString.Length <= maxLines) {
                return aString;
            }

            string returnString = "";

            for (int i = (int)aScrollPos; i < (aScrollPos + maxLines); i++) {
                if (i < splitString.Length) {
                    returnString += splitString[i] + "\n";
                } else {
                    continue;
                }
            }

            return returnString;
        }
    }
}
