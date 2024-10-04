/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

using TackEngine.Core.Main;
using TackEngine.Core.Engine;
using TackEngine.Core.Objects;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Renderer;
using TackEngine.Core.GUI;
using TackEngine.Core.Physics;
using TackEngine.Core.Source.Renderer.LineRendering;
using TackEngine.Core.Source.Renderer;
using TackEngine.Core.Renderer.LineRendering;

namespace TackEngine.Core.Renderer
{
    public class TackRenderer {
        public static TackRenderer Instance { get; protected set; }

        public static readonly int MIN_RENDER_LAYER = 0;
        public static readonly int MAX_RENDER_LAYER = 10000;

        private List<Shader> m_shaders;
        private bool m_fpsCounterActive;
        private RenderingBehaviour m_currentRenderer;
        private GUITextArea m_fpsCounterTextArea;
        private float m_previousRenderTime;
        private int m_previousDrawCallCount;
        private int m_currentTextureUnitIndex = 0;
        private LineRenderer m_lineRenderer;
        private SplitScreenMode m_splitScreenMode = SplitScreenMode.Single;
        private IShaderImplementation m_shaderImplementation;

        public Colour4b BackgroundColour { get; set; }

        public bool FpsCounterActive { 
            get { return m_fpsCounterActive; } 
            set {
                bool oldValue = m_fpsCounterActive;
                m_fpsCounterActive = value;
                OnFpsCounterActiveChanged(oldValue, m_fpsCounterActive);
            }
        }

        internal BaseTackGUI GUIInstance { get; set; }
        internal int CurrentTextureUnitIndex { get { return m_currentTextureUnitIndex; } }
        internal SplitScreenMode CurrentSplitScreenMode { get { return m_splitScreenMode; } }
        internal Camera[] Cameras { get; }
        internal IShaderImplementation ShaderImplementation { get { return m_shaderImplementation; } }

        public Shader DefaultWorldShader { get; private set; }
        public Shader DefaultLitWorldShader { get; private set; }

        internal TackRenderer(RenderingBehaviour renderingBehaviour, LineRenderingBehaviour lineRenderingBehaviour, BaseTackGUI guiInstance, IShaderImplementation shaderImpl) {
            Instance = this;

            BackgroundColour = new Colour4b(150, 150, 150, 255);

            m_shaders = new List<Shader>();

            Cameras = new Camera[4];

            m_currentRenderer = renderingBehaviour;
            m_lineRenderer = new LineRenderer(lineRenderingBehaviour);
            GUIInstance = guiInstance;
            m_shaderImplementation = shaderImpl;
        }

        internal void OnStart() {
            GUIInstance.OnStart();

            LoadShadersForSplitScreenMode();

            m_currentRenderer.OnStart();

            m_lineRenderer.Initialise();

            FpsCounterActive = true;
        }

        internal void OnUpdate() {
            if (m_fpsCounterActive) {
                if (TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.Windows ||
                    TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.Linux ||
                    TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.MacOS) {

                    m_fpsCounterTextArea.Position = new Vector2f(TackEngineInstance.Instance.Window.WindowSize.X - 150, 5);
                    m_fpsCounterTextArea.Size = new Vector2f(145, 65);
                }

                if (TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.Android) {
                    m_fpsCounterTextArea.Position = new Vector2f(TackEngineInstance.Instance.Window.WindowSize.X - 205, 5);
                    m_fpsCounterTextArea.Size = new Vector2f(200, 150);
                }

                m_fpsCounterTextArea.Text = "U: " + (1f / EngineTimer.Instance.UpdateTimeAverageLastSecond).ToString("0") + "(" + (EngineTimer.Instance.UpdateTimeAverageLastSecond * 1000f).ToString("0.00") + "ms)\n" +
                    "R: " + (1f / EngineTimer.Instance.RenderTimeAverageLastSecond).ToString("0") + " (" + (EngineTimer.Instance.RenderTimeAverageLastSecond * 1000f).ToString("0.00") + "ms)\n" +
                    "DC: " + m_previousDrawCallCount + "\n" +
                    "Lines: " + LineRenderer.Instance.ItemsRenderedLastFrame;
            }

            GUIInstance.OnUpdate();
        }

        internal void OnRender(double timeSinceLastRender) {
            m_previousRenderTime = (float)timeSinceLastRender;

            TackProfiler.Instance.StartTimer("Renderer.PreRender");
            m_currentRenderer.PreRender();
            TackProfiler.Instance.StopTimer("Renderer.PreRender");

            TackProfiler.Instance.StartTimer("Renderer.RenderToScreen");
            // Render everything in the world using the current renderer
            m_currentRenderer.RenderToScreen(out m_previousDrawCallCount);
            TackProfiler.Instance.StopTimer("Renderer.RenderToScreen");

            TackProfiler.Instance.StartTimer("Renderer.PostRender");
            m_currentRenderer.PostRender();
            TackProfiler.Instance.StopTimer("Renderer.PostRender");

            m_lineRenderer.DrawLinesToScreen(Core.Source.Renderer.LineRendering.LineRenderer.LineContext.World);

            TackProfiler.Instance.StartTimer("Renderer.GUIRender");
            // Render GUI
            GUIInstance.OnGUIPreRender();
            GUIInstance.OnGUIRender();
            GUIInstance.OnGUIPostRender();
            TackProfiler.Instance.StopTimer("Renderer.GUIRender");

            m_lineRenderer.DrawLinesToScreen(Core.Source.Renderer.LineRendering.LineRenderer.LineContext.GUI);

            m_lineRenderer.ClearLineJobQueue();
        }

        internal void OnClose() {
            for (int i = 0; i < m_shaders.Count; i++) {
                m_shaders[i].Destroy();
            }

            GUIInstance.OnClose();
        }

        internal void IncrementCurrentTextureUnitIndex() {
            m_currentTextureUnitIndex++;
        }

        internal void ResetCurrentTextureUnitIndex() {
            m_currentTextureUnitIndex = 0;
        }

        public virtual void SetSplitScreenMode(SplitScreenMode mode) {
            m_splitScreenMode = mode;

            LoadShadersForSplitScreenMode();
        }

        internal void LoadShadersForSplitScreenMode() {
            if (DefaultWorldShader != null) {
                DefaultWorldShader.Destroy();
            }

            if (DefaultLitWorldShader != null) {
                DefaultLitWorldShader.Destroy();
            }

            string vertShaderNamePref = "";

            if (CurrentSplitScreenMode == Core.Source.Renderer.SplitScreenMode.DualScreen) {
                vertShaderNamePref = "dual_screen_";
            }

            if (CurrentSplitScreenMode == Core.Source.Renderer.SplitScreenMode.QuadScreen) {
                vertShaderNamePref = "quad_screen_";
            }

            DefaultWorldShader = Shader.LoadFromFile("shaders.default_world_shader", Shader.ShaderContext.World, "tackresources/shaders/world/" + vertShaderNamePref + "world_vertex_shader.vs",
                                                                                             "tackresources/shaders/world/world_fragment_shader.fs");

            DefaultLitWorldShader = Shader.LoadFromFile("shaders.default_world_shader_lit", Shader.ShaderContext.World, "tackresources/shaders/world/" + vertShaderNamePref + "world_vertex_shader.vs",
                                                                                                          "tackresources/shaders/world/world_fragment_shader_lit.fs");
        }

        internal void AddShader(Shader shader) {
            if (!shader.CompiledAndLinked) {
                TackConsole.EngineLog(TackConsole.LogType.Error, "Error: Cannot add a shader that is not compiled and linked");
                return;
            }

            if (m_shaders.Count(x => x.Name == shader.Name) == 0) {
                m_shaders.Add(shader);

                TackConsole.EngineLog(TackConsole.LogType.Message, "Successfully added a new Shader to the current renderer with the name '" + shader.Name + "'");
                return;
            }

            TackConsole.EngineLog(TackConsole.LogType.Error, "Error: Failed to add a new Shader to the current renderer. There is already a Shader with the name '" + shader.Name + "'");
        }

        internal Shader GetShader(string shaderName) {
            Shader shader = m_shaders.Find(x => x.Name == shaderName);

            if (shader == null) {
                return DefaultWorldShader;
            }

            return shader;
        }

        internal Camera[] GetCameraListForCurrentSplitScreenMode() {
            if (m_splitScreenMode == SplitScreenMode.DualScreen) {
                return new Camera[] { Cameras[0], Cameras[1] };
            }

            if (CurrentSplitScreenMode == SplitScreenMode.QuadScreen) {
                return Cameras;
            }

            return new Camera[] { Cameras[0] };
        }

        public static Vector2f FindScreenCoordsFromPosition(Vector2f _pos)
        {
            Vector2f vec = new Vector2f()
            {
                X = ((_pos.X - Camera.MainCamera.GetParent().Position.X) / (TackEngineInstance.Instance.Window.WindowSize.X / 2)),
                Y = ((_pos.Y + Camera.MainCamera.GetParent().Position.Y) / (TackEngineInstance.Instance.Window.WindowSize.Y / 2))
            };

            return vec;
        }

        private void OnFpsCounterActiveChanged(bool oldValue, bool newValue) {
            if (newValue) {
                if (m_fpsCounterTextArea == null) {
                    m_fpsCounterTextArea = new GUITextArea();
                    m_fpsCounterTextArea.Position = new Vector2f(TackEngineInstance.Instance.Window.WindowSize.X - 300, 5);
                    m_fpsCounterTextArea.Size = new Vector2f(295, 175);
                    m_fpsCounterTextArea.LinkedSceneType = null;

                    GUITextArea.GUITextAreaStyle style = new GUITextArea.GUITextAreaStyle() {
                        Border = null,
                        Colour = new Colour4b(0, 0, 0, 100),
                        Font = GUIInstance.DefaultFont,
                        FontColour = Colour4b.White,
                        FontSize = BaseTackGUI.GetDefaultPlatformFontSize(TackEngineInstance.Instance.Platform),
                    };

                    m_fpsCounterTextArea.NormalStyle = style;
                }
            }

            m_fpsCounterTextArea.Active = newValue;
        }
    }
}
