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

namespace TackEngine.Core.Renderer
{
    public abstract class TackRenderer {
        public static TackRenderer Instance { get; protected set; }

        protected List<Shader> m_shaders;
        protected float[] mVertexData;
        protected bool mRenderFpsCounter;
        protected Colour4b mBackgroundColour;
        protected RenderingBehaviour m_currentRenderer;
        protected GUITextArea m_fpsCounterTextArea;
        protected float m_previousRenderTime;
        protected int m_previousDrawCallCount;
        private int m_currentTextureUnitIndex = 0;
        protected LineRenderer m_lineRenderer;
        protected SplitScreenMode m_splitScreenMode = SplitScreenMode.Single;
        internal IShaderImplementation m_shaderImplementation;

        public static Colour4b BackgroundColour {
            get { return Instance.mBackgroundColour; }
            set { Instance.mBackgroundColour = value; }
        }

        public static int MaxTextureUnits { get; set; }
        internal BaseTackGUI GUIInstance { get; set; }
        internal int CurrentTextureUnitIndex { get { return m_currentTextureUnitIndex; } }
        internal SplitScreenMode CurrentSplitScreenMode { get { return m_splitScreenMode; } }
        internal Camera[] Cameras { get; }
        internal IShaderImplementation ShaderImplementation { get { return m_shaderImplementation; } }

        public Shader DefaultWorldShader { get; protected set; }
        public Shader DefaultLitWorldShader { get; protected set; }

        internal TackRenderer() {
            mBackgroundColour = new Colour4b(150, 150, 150, 255);

            m_shaders = new List<Shader>();

            Cameras = new Camera[4];
        }

        internal abstract void OnStart();

        internal abstract void OnUpdate();

        internal abstract void OnRender(double timeSinceLastRender);

        internal abstract void OnClose();

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

        internal virtual void LoadShadersForSplitScreenMode() {
            if (DefaultWorldShader != null) {
                DefaultWorldShader.Destroy();
            }

            if (DefaultLitWorldShader != null) {
                DefaultLitWorldShader.Destroy();
            }
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
    }
}
