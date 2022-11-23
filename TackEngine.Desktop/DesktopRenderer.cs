using OpenTK.Graphics.ES30;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TackEngineLib.Main;

using TackEngineLib.Renderer;
using TackEngineLib.Engine;
using TackEngineLib.Objects;
using TackEngineLib.Objects.Components;
using TackEngineLib.Physics;
using OpenTK;
using TackEngineLib.GUI;

namespace TackEngine.Desktop {
    internal class DesktopRenderer : TackRenderer {

        public DesktopRenderer() {
            Instance = this;
        }

        public override void OnStart() {
            GUIInstance = new TackGUI();
            GUIInstance.OnStart();

            m_currentRenderer = new DesktopRenderingBehaviour();
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

            TackProfiler.Instance.StartTimer("Renderer.RenderToScreen");
            // Render everything in the world using the current renderer
            m_currentRenderer.RenderToScreen(out m_previousDrawCallCount);
            TackProfiler.Instance.StopTimer("Renderer.RenderToScreen");

            // Render TackPhysics debug objects
            TackPhysics.GetInstance().Render();

            TackProfiler.Instance.StartTimer("Renderer.GUIRender");
            // Render GUI
            GUIInstance.OnGUIRender();
            TackProfiler.Instance.StopTimer("Renderer.GUIRender");
        }

        public override void OnClose() {
            for (int i = 0; i < m_loadedShaders.Count; i++) {
                m_loadedShaders[i].Destroy();
            }

            GUIInstance.OnClose();
        }
    }
}
