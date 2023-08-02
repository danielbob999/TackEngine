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

namespace TackEngine.Desktop {
    internal class DesktopRenderer : TackRenderer {

        public DesktopRenderer() {
            Instance = this;
            mRenderFpsCounter = true;
        }

        public override void OnStart() {
            GUIInstance = new TackGUI();
            GUIInstance.OnStart();

            m_currentRenderer = new DesktopRenderingBehaviour();

            m_fpsCounterTextArea = new GUITextArea();
            m_fpsCounterTextArea.Position = new Vector2f(Camera.MainCamera.RenderTarget.Width - 150, 5);
            m_fpsCounterTextArea.Size = new Vector2f(145, 53);

            GUITextArea.GUITextAreaStyle style = new GUITextArea.GUITextAreaStyle() {
                Border = null,
                Colour = new Colour4b(0, 0, 0, 100),
                Font = GUIInstance.DefaultFont,
                FontColour = Colour4b.White,
                FontSize = 8f,
            };

            m_fpsCounterTextArea.NormalStyle = style;
        }

        public override void OnUpdate() {
            if (mRenderFpsCounter) {
                m_fpsCounterTextArea.Active = mRenderFpsCounter;
                m_fpsCounterTextArea.Position = new Vector2f(Camera.MainCamera.RenderTarget.Width - 150, 5);
                m_fpsCounterTextArea.Size = new Vector2f(145, 53);

                m_fpsCounterTextArea.Text = "U: " + (1f / EngineTimer.Instance.UpdateTimeAverageLastSecond).ToString("0") + "(" + (EngineTimer.Instance.UpdateTimeAverageLastSecond * 1000f).ToString("0.00") + "ms)\n" +
                    "R: " + (1f / EngineTimer.Instance.RenderTimeAverageLastSecond).ToString("0") + " (" + (EngineTimer.Instance.RenderTimeAverageLastSecond * 1000f).ToString("0.00") + "ms)\n" +
                    "DC: " + m_previousDrawCallCount;
            }

            GUIInstance.OnUpdate();
        }

        public override void OnRender(double timeSinceLastRender) {
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

            TackProfiler.Instance.StartTimer("Renderer.GUIRender");
            // Render GUI
            GUIInstance.OnGUIPreRender();
            GUIInstance.OnGUIRender();
            GUIInstance.OnGUIPostRender();
            TackProfiler.Instance.StopTimer("Renderer.GUIRender");
        }

        public override void OnClose() {
            for (int i = 0; i < m_loadedShaders.Count; i++) {
                m_loadedShaders[i].Destroy();
            }

            GUIInstance.OnClose();
        }

        public void PhysicsDebugDraw() {
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
    }
}
