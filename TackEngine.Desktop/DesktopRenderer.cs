using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TackEngine.Core.Main;

using TackEngine.Core.Renderer;
using TackEngine.Core.Engine;
using TackEngine.Core.Objects;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Physics;
using OpenTK;
using TackEngine.Core.GUI;
using OpenTK.Graphics.OpenGL;
using TackEngine.Desktop.Renderer;
using TackEngine.Core.Source.Renderer.LineRendering;

namespace TackEngine.Desktop {
    internal class DesktopRenderer : TackRenderer {

        internal DesktopRenderer() {
            Instance = this;
            mRenderFpsCounter = true;
        }

        internal override void OnStart() {
            m_shaderImplementation = new DesktopShaderImpl();

            GUIInstance = new TackGUI();
            GUIInstance.OnStart();

            LoadShadersForSplitScreenMode();

            m_currentRenderer = new DesktopRenderingBehaviour();
            m_lineRenderer = new Core.Source.Renderer.LineRendering.LineRenderer(new DesktopLineRenderingBehaviour());
            m_lineRenderer.Initialise();

            m_fpsCounterTextArea = new GUITextArea();
            m_fpsCounterTextArea.Position = new Vector2f(TackEngineInstance.Instance.Window.WindowSize.X - 150, 5);
            m_fpsCounterTextArea.Size = new Vector2f(145, 65);
            m_fpsCounterTextArea.LinkedSceneType = null;

            GUITextArea.GUITextAreaStyle style = new GUITextArea.GUITextAreaStyle() {
                Border = null,
                Colour = new Colour4b(0, 0, 0, 100),
                Font = GUIInstance.DefaultFont,
                FontColour = Colour4b.White,
                FontSize = 8f,
            };

            m_fpsCounterTextArea.NormalStyle = style;
        }

        internal override void OnUpdate() {
            if (mRenderFpsCounter) {
                m_fpsCounterTextArea.Active = mRenderFpsCounter;
                m_fpsCounterTextArea.Position = new Vector2f(TackEngineInstance.Instance.Window.WindowSize.X - 150, 5);
                m_fpsCounterTextArea.Size = new Vector2f(145, 65);

                m_fpsCounterTextArea.Text = "U: " + (1f / EngineTimer.Instance.UpdateTimeAverageLastSecond).ToString("0") + "(" + (EngineTimer.Instance.UpdateTimeAverageLastSecond * 1000f).ToString("0.00") + "ms)\n" +
                    "R: " + (1f / EngineTimer.Instance.RenderTimeAverageLastSecond).ToString("0") + " (" + (EngineTimer.Instance.RenderTimeAverageLastSecond * 1000f).ToString("0.00") + "ms)\n" +
                    "DC: " + m_previousDrawCallCount + "\n" +
                    "Lines: " + LineRenderer.Instance.ItemsRenderedLastFrame;
            }

            GUIInstance.OnUpdate();
        }

        internal override void OnRender(double timeSinceLastRender) {
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

            // Render TackPhysics debug objects
            //TackPhysics.GetInstance().Render();
            PhysicsDebugDraw();

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

        internal override void OnClose() {
            for (int i = 0; i < m_shaders.Count; i++) {
                m_shaders[i].Destroy();
            }

            GUIInstance.OnClose();

            m_lineRenderer.Close();
        }

        internal void PhysicsDebugDraw() {
            TackObject[] objs = TackObject.Get();

            List<TackObject> physObjs = new List<TackObject>();

            for (int i = 0; i < objs.Length; i++) {
                if (objs[i].GetComponent<BasePhysicsComponent>() != null) {
                    physObjs.Add(objs[i]);
                }
            }

            for (int i = 0; i < physObjs.Count; i++) {
                BasePhysicsComponent comp = physObjs[i].GetComponent<BasePhysicsComponent>();
            }
        }

        internal override void LoadShadersForSplitScreenMode() {
            base.LoadShadersForSplitScreenMode();

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
    }
}
