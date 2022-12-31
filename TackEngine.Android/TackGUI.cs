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
using System.Reflection.Metadata;

namespace TackEngine.Android {
    /// <summary>
    /// The main class for rendering GUI elements to the screen
    /// </summary>
    public class TackGUI : BaseTackGUI {
        private int m_vao;
        private int m_vbo;
        private int m_ebo;
        private float[] m_vertexData;
        private int[] m_indexData;
        private int m_posHandle;
        private int m_uvHandle;

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

            m_posHandle = GL.GetAttribLocation(m_defaultGUIShader.Id, "aPos");
            m_uvHandle = GL.GetAttribLocation(m_defaultGUIShader.Id, "aTexCoord");

            m_vertexData = new float[20] {
                    //       Position (XYZ)                                                                                                      Colours (RGB)                                                                                  TexCoords (XY)
                    /* v1 */  1f, -1f, 1.0f,       1.0f, 1.0f,
                    /* v2 */  1f,  1f, 1.0f,       1.0f, 0.0f,
                    /* v3 */ -1f,  1f, 1.0f,       0.0f, 0.0f,
                    /* v4 */ -1f, -1f, 1.0f,       0.0f, 1.0f
            };

            m_indexData = new int[] {
                    0, 1, 3, // first triangle 
                    1, 2, 3  // second triangle - 1, 2, 3
            };

            GL.GenBuffers(1, out m_vbo);
            /*
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * 20), m_vertexData, BufferUsage.StaticDraw);

            // position attribute
            GL.VertexAttribPointer(m_posHandle, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(m_posHandle);

            // uv attribute
            GL.VertexAttribPointer(m_uvHandle, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(m_uvHandle);
            */
            TackConsole.EngineLog(TackConsole.LogType.Message, "Generated TackGUI buffers");
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

            GL.DeleteBuffers(1, ref m_vbo);
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

            /*
            int posHandle = GL.GetAttribLocation(m_defaultGUIShader.Id, "aPos");
            int uvHandle = GL.GetAttribLocation(m_defaultGUIShader.Id, "aTexCoord");
            */

            GL.UseProgram(m_defaultGUIShader.Id);

            /*
            GL.GenBuffers(1, out int VBO);
            */
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * 20), m_vertexData, BufferUsage.StaticDraw);

            // position attribute
            GL.VertexAttribPointer(m_posHandle, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(m_posHandle);

            // uv attribute
            GL.VertexAttribPointer(m_uvHandle, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(m_uvHandle);

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
                fixed (void* ptr = m_indexData) {
                    GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, new IntPtr(ptr));
                }
            }
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

            GL.UseProgram(m_defaultGUIShader.Id);

            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * 20), m_vertexData, BufferUsage.StaticDraw);

            // position attribute
            GL.VertexAttribPointer(m_posHandle, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(m_posHandle);

            // uv attribute
            GL.VertexAttribPointer(m_uvHandle, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(m_uvHandle);

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

            Vector2f charPos = new Vector2f(minPos.X, minPos.Y + scrollPosition.Y);

            for (int i = 0; i < text.Length; i++) {
                if (text[i] == '\n') {
                    charPos.Y += style.Font.GetFontCharacter('B').size.Y;
                    charPos.X = minPos.X;
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
                    float lengthOfNextWord = MeasureStringLength(subStr, font, style.FontSize);


                    if (charPos.X + lengthOfNextWord > (rect.X + rect.Width)) {
                        charPos.Y += TackFont.GetFontCharacterScaledSize(font.GetFontCharacter('B').size, style.FontSize).Y;

                        charPos.X = minPos.X;
                        continue;
                    }
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

                Vector2f finalScale = TackFont.GetFontCharacterScaledSize(fchar.size, style.FontSize);

                // Generate translation matrix
                OpenTK.Matrix4 transMat = OpenTK.Matrix4.CreateTranslation(charPos.X + (finalScale.X / 2.0f), -charPos.Y - (finalScale.Y / 2.0f), 0);

                // Generate scale matrix
                OpenTK.Matrix4 scaleMat = MatrixUtility.CreateScaleMatrix((finalScale.X / 2.0f), (finalScale.Y / 2.0f), 1);

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
                    fixed (void* ptr = m_indexData) {
                        GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, new IntPtr(ptr));
                    }
                }

                charPos.X += finalScale.X;
            }
        }

        /// <summary>
        /// Measures the length of a string with the given font and font size. 
        /// Note: This method returns the length of a string up to either the end of the string OR an \n OR \r character
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        internal override float MeasureStringLength(string text, TackFont font, float fontSize) {
            float length = 0;

            for (int i = 0; i < text.Length; i++) {
                if (text[i] == '\n' || text[i] == '\r') {
                    return length;
                }

                TackFont.FontCharacter ch = font.GetFontCharacter(text[i]);

                length += TackFont.GetFontCharacterScaledSize(ch.size, fontSize).Length;
            }

            return length;
        }

        internal override Vector2f MeasureStringSize(string text, TackFont font, float fontSize, RectangleShape rect) {
            Vector2f maxSize = new Vector2f();

            float currentLength = 0;

            for (int i = 0; i < text.Length; i++) {
                if (text[i] == '\n') {
                    maxSize.Y += TackFont.GetFontCharacterScaledSize(font.GetFontCharacter('B').size, fontSize).Y;
                    currentLength = 0;
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
                    float lengthOfNextWord = MeasureStringLength(subStr, font, fontSize);


                    if (currentLength + lengthOfNextWord > rect.Width) {
                        maxSize.Y += TackFont.GetFontCharacterScaledSize(font.GetFontCharacter('B').size, fontSize).Y;

                        if (maxSize.X < currentLength) {
                            maxSize.X = currentLength;
                        }

                        currentLength = 0;
                        continue;
                    }
                }

                TackFont.FontCharacter ch = font.GetFontCharacter(text[i]);

                Vector2f finalSize = TackFont.GetFontCharacterScaledSize(ch.size, fontSize);

                if (ch.texId == -1) {
                    continue;
                }

                bool wordSplit = false;

                // If true, the word is too long to fit on a single line so we must break it up with a hyphen
                if (currentLength + finalSize.X > (rect.Width)) {
                    maxSize.X += TackFont.GetFontCharacterScaledSize(font.GetFontCharacter('-').size, fontSize).X;
                    maxSize.Y += TackFont.GetFontCharacterScaledSize(font.GetFontCharacter('B').size, fontSize).Y;

                    wordSplit = true;
                }

                if (!wordSplit) {
                    currentLength += finalSize.X;
                } else {
                    maxSize.Y += TackFont.GetFontCharacterScaledSize(font.GetFontCharacter('B').size, fontSize).Y;
                    currentLength = 0;
                    i--;
                }

                if (maxSize.X < currentLength) {
                    maxSize.X = currentLength;
                }
            }

            if (!string.IsNullOrEmpty(text) && maxSize.Y == 0) {
                maxSize.Y += TackFont.GetFontCharacterScaledSize(font.GetFontCharacter('B').size, fontSize).Y;
            }

            return new Vector2f(maxSize.X, maxSize.Y);
        }
    }
}
