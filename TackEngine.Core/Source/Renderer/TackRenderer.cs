﻿/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

using TackEngineLib.Main;
using TackEngineLib.Engine;
using TackEngineLib.Objects;
using TackEngineLib.Objects.Components;
using TackEngineLib.Renderer;
using TackEngineLib.GUI;
using TackEngineLib.Physics;

namespace TackEngineLib.Renderer
{
    internal abstract class TackRenderer
    {
        public static TackRenderer Instance { get; protected set; }

        protected List<BaseShader> m_loadedShaders;
        protected float[] mVertexData;
        protected bool mRenderFpsCounter;
        protected Colour4b mBackgroundColour;
        protected RenderingBehaviour m_currentRenderer;
        protected GUITextArea m_fpsCounterTextArea;
        protected float m_previousRenderTime;
        protected int m_previousDrawCallCount;

        public static Colour4b BackgroundColour
        {
            get { return Instance.mBackgroundColour; }
            set { Instance.mBackgroundColour = value; }
        }

        public static int MaxTextureUnits { get; set; }

        public BaseTackGUI GUIInstance { get; set; }

        public BaseShader DefaultWorldShader { get { return m_currentRenderer.DefaultWorldShader; } }

        internal TackRenderer() {
            mBackgroundColour = new Colour4b(150, 150, 150, 255);

            m_loadedShaders = new List<BaseShader>();
        }

        public abstract void OnStart();

        public abstract void OnUpdate();

        public abstract void OnRender(double timeSinceLastRender);

        public abstract void OnClose();

        public void AddShader(BaseShader shader) {
            m_currentRenderer.AddShader(shader);
        }

        public BaseShader GetShader(string shaderName) {
            return m_currentRenderer.GetShader(shaderName);
        }

        public static void SetFpsCounterState(bool state) {
        }

        public static TackRenderer GetInstance() {
            return Instance;
        }

        public static Vector2f FindScreenCoordsFromPosition(Vector2f _pos)
        {
            Vector2f vec = new Vector2f()
            {
                X = ((_pos.X - Camera.MainCamera.GetParent().Position.X) / (TackEngine.Instance.Window.WindowSize.X / 2)),
                Y = ((_pos.Y + Camera.MainCamera.GetParent().Position.Y) / (TackEngine.Instance.Window.WindowSize.Y / 2))
            };

            return vec;
        }
    }
}
