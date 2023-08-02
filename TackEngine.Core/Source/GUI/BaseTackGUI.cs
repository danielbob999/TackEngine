using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using TackEngine.Core.GUI;
using TackEngine.Core.Input;
using TackEngine.Core.Main;
using TackEngine.Core.Renderer.Operations;
using TackEngine.Core.Renderer;
using SharpFont;

namespace TackEngine.Core.GUI {
    public abstract class BaseTackGUI {
        public static BaseTackGUI Instance { get; protected set; }

        protected List<GUIOperation> m_guiOperations;
        protected BaseShader m_defaultGUIShader;
        protected List<GUIObject> m_guiObjects = new List<GUIObject>();
        protected Library m_fontLibrary;
        protected TackFont m_defaultFont;
        protected BaseShader m_defaultTextShader;
        protected List<Events.GUIMouseEvent> m_mouseEventQueue;
        protected List<Events.GUIKeyboardEvent> m_keyboardEventQueue;
        protected List<int> m_guiObjectsToRemove;
        protected List<GUIObject> m_preRenderQueue;
        protected List<GUIObject> m_preRenderCheckList;

        internal static List<GUIInputField> inputFields = new List<GUIInputField>();

        public BaseShader DefaultGUIShader { get { return m_defaultGUIShader; } }
        public BaseShader DefaultTextShader { get { return m_defaultTextShader; } }
        public Library FontLibrary { get { return m_fontLibrary; } }
        public TackFont DefaultFont { get { return m_defaultFont; } }
        public GUIObject FocusedGUIObject {
            get {
                int indx = m_guiObjects.FindIndex(x => x.IsFocused);

                return (indx > 0 ? m_guiObjects[indx] : null);
            }
        }
        public bool MouseClickDetectedThisFrame { get; protected set; }

        internal virtual void OnStart() {
            m_fontLibrary = new Library();

            /*
            if (Instance != null) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "There is already an active instance of TackGUI.");
                return;
            }

            Instance = this;

            m_mouseEventQueue = new List<Events.GUIMouseEvent>();
            m_keyboardEventQueue = new List<Events.GUIKeyboardEvent>();
            m_guiObjectsToRemove = new List<int>();
            m_preRenderQueue = new List<GUIObject>();
            m_preRenderCheckList = new List<GUIObject>();

            m_defaultFont = TackFont.LoadFromFile(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\Arial.ttf");

            m_fontCollection = new PrivateFontCollection();
            m_activeFontFamily = new FontFamily("Arial");
            m_fontCollection.AddFontFile(Environment.GetFolderPath(Environment.SpecialFolder.Fonts) + "\\Arial.ttf");
            TackConsole.EngineLog(TackConsole.LogType.Message, string.Format("Added default font file from: {0}\\Arial.ttf", Environment.GetFolderPath(Environment.SpecialFolder.Fonts)));

            m_guiOperations = new List<GUIOperation>();
            */

            /*
            m_defaultGUIShader = new Shader("shaders.default_gui_shader", TackShaderType.GUI, System.IO.File.ReadAllText("tackresources/shaders/gui/default_gui_vertex_shader.vs"),
                                                                                              System.IO.File.ReadAllText("tackresources/shaders/gui/default_gui_fragment_shader.fs"));
            */

            /*
            m_defaultGUIShader = new BaseShader("shaders.new_gui_shader", TackShaderType.GUI, System.IO.File.ReadAllText("tackresources/shaders/gui/new/gui_vertex_shader.vs"),
                                                                                          System.IO.File.ReadAllText("tackresources/shaders/gui/new/gui_fragment_shader.fs"));

            m_defaultTextShader = new BaseShader("shaders.text_shader", TackShaderType.GUI, System.IO.File.ReadAllText("tackresources/shaders/gui/text/text_vertex_shader.vs"),
                                                                                        System.IO.File.ReadAllText("tackresources/shaders/gui/text/text_fragment_shader.fs"));
            */
        }

        internal virtual void OnUpdate() {
            /*
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

                    RectangleShape shape = m_guiObjects[j].GetShapeWithMask();

                    if (Physics.AABB.IsPointInAABB(new Physics.AABB(new Vector2f(shape.X, shape.Y + shape.Height), new Vector2f(shape.X + shape.Width, shape.Y)), m_mouseEventQueue[i].Args.MousePosition.ToVector2f())) {
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
            */
        }

        internal virtual void OnGUIPreRender() {

        }

        internal virtual void OnGUIRender() {
            /*
            for (int i = 0; i < m_guiObjects.Count; i++) {
                if (m_guiObjects[i].ParentId == null) {
                    if (m_guiObjects[i].Active) {
                        m_guiObjects[i].OnRender(new GUIMaskData(new List<RectangleShape>()));
                    }
                }
            }
            */
        }

        internal virtual void OnGUIPostRender() {

        }

        internal virtual void OnClose() {
            /*
            // Loop through the current GUIObject List calling the OnUpdate function on each
            for (int i = 0; i < m_guiObjects.Count; i++) {
                ((GUIObject)m_guiObjects[i]).OnClose();
            }
            */
        }

        public void RegisterGUIObject(GUIObject guiObject) {
            m_guiObjects.Add(guiObject);
        }

        public void DeregisterGUIObject(GUIObject guiObject) {
            // Mark the object for deletion
            m_guiObjectsToRemove.Add(guiObject.Id);

            // Disable
            guiObject.Active = false;
        }

        internal void RegisterGUIObjectPreRenderCheck(GUIObject obj) {
            m_preRenderCheckList.Add(obj);
        }

        internal void RegisterGUIObjectsPreRenderCheck(List<GUIObject> objs) {
            m_preRenderCheckList.AddRange(objs);
        }

        internal void AddGUIObjectToPreRenderQueue(GUIObject obj) {
            m_preRenderQueue.Add(obj);
        }

        internal GUIObject GetGUIObjectById(int? parentId) {
            if (parentId == null) {
                return null;
            }

            return m_guiObjects.Find(o => o.Id == parentId);
        }

        internal void RegisterMouseEvent(Events.GUIMouseEvent e) {
            if (m_mouseEventQueue.FindIndex(x => (x.Args.MouseAction == e.Args.MouseAction) && (x.Args.MouseButton == e.Args.MouseButton)) == -1) {
                m_mouseEventQueue.Add(e);
            }
        }

        internal void RegisterKeyboardEvent(Events.GUIKeyboardEvent e) {
            if (m_keyboardEventQueue.FindIndex(x => (x.Args.Key == e.Args.Key) && (x.Args.KeyAction == e.Args.KeyAction)) == -1) {
                m_keyboardEventQueue.Add(e);
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

        internal abstract void InternalBox(RectangleShape rect, GUIBox.GUIBoxStyle style, GUIMaskData maskData);
        internal abstract void InternalBorder(RectangleShape objectBounds, GUIBorder border);
        internal abstract void InternalTextArea(RectangleShape rect, string text, GUITextArea.GUITextAreaStyle style, Vector2f scrollPosition, GUIMaskData maskData, int caretCharacterIndex);

        internal abstract float MeasureStringLength(string text, TackFont font, float finalFontSize);

        internal abstract Vector2f MeasureStringSize(string text, TackFont font, float fontSize, RectangleShape rect);
    }
}
