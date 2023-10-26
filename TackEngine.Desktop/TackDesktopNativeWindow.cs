using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TackEngine.Core.Engine;
using TackEngine.Core.Input;
using TackEngine.Core.Main;
using TackEngine.Core.Objects;
using TackEngine.Core.Physics;
using TackEngine.Core.Renderer;
using TackEngine.Core.GUI;
using OpenTK.Platform;
using OpenTK.Input;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using TackEngine.Desktop;

namespace TackEngine.Desktop {
    public class TackDesktopNativeWindow : NativeWindow, IBaseTackWindow {
        public static TackDesktopNativeWindow Instance { get; private set; } = null;

        public Vector2f WindowSize { get { return new Vector2f(base.ClientSize.X, base.ClientSize.Y); } }
        public ulong CurrentUpdateLoopIndex { get { return m_currentUpdateLoopIndex; } }
        public ulong CurrentRenderLoopIndex { get { return m_currentRenderLoopIndex; } }

        private float m_targetFrameTime;

        private ulong m_currentUpdateLoopIndex;
        private ulong m_currentRenderLoopIndex;

        // Modules
        private TackConsole mTackConsole;
        private TackPhysics mTackPhysics;
        private TackObjectManager mTackObjectManager;
        private TackRenderer mTackRender;
        private TackLightingSystem mTackLightingSystem;
        private TackEngine.Core.Main.EngineTimer m_engineTimer;
        private TackProfiler m_tackProfiler;
        private TackInput m_tackInput;
        private DesktopSpriteManager m_spriteManager;
        private DebugLineRenderer m_debugLineRenderer;

        private EngineDelegates.OnStart onStartFunction;
        private EngineDelegates.OnUpdate onUpdateFunction;
        private EngineDelegates.OnClose onCloseFunction;

        private CancellationTokenSource m_cancelTokenSource;

        public TackDesktopNativeWindow(TackEngine.Core.Engine.TackEngineInstance.InitalisationSettings settings, EngineDelegates.OnStart startFn, EngineDelegates.OnUpdate updateFn, EngineDelegates.OnClose closeFn)
            : base(new NativeWindowSettings() { Size = new OpenTK.Mathematics.Vector2i(settings.WindowSize.X, settings.WindowSize.Y) , NumberOfSamples = settings.MSAASampleCount, Title = settings.WindowTitle, WindowBorder = (OpenTK.Windowing.Common.WindowBorder)settings.WindowBorder, WindowState = (OpenTK.Windowing.Common.WindowState)settings.WindowState }) {

            // We must initialise a TackConsole instance before doing anything
            mTackConsole = new TackConsole();

            TackEngine.Core.Engine.TackEngineInstance.Initialise(this, settings, TackEngineInstance.TackEnginePlatform.Windows);

            // Set all implementations
            TackFont.FontLoadingImplementation = new DesktopTackFontLoadingImpl();

            onStartFunction = startFn;
            onUpdateFunction = updateFn;
            onCloseFunction = closeFn;

            m_currentUpdateLoopIndex = 0;
            m_currentRenderLoopIndex = 0;

            m_engineTimer = new EngineTimer();
            m_engineTimer.OnStart();

            // If VSync is enabled, set the target update/render frequencies to the VSync value
            VSync = (settings.VSync == true ? VSyncMode.On : VSyncMode.Off);

            if (!settings.VSync) {
                m_targetFrameTime = 1f / (float)settings.TargetUpdateRenderFrequency;
            } else {
                m_targetFrameTime = 1f / (float)Monitors.GetMonitorFromWindow(this).CurrentVideoMode.RefreshRate;
            }

            Instance = this;

        }

        public unsafe void Run() {
            System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Highest;
            System.Threading.Thread.CurrentThread.IsBackground = false;

            System.Diagnostics.Stopwatch loopWatch = new System.Diagnostics.Stopwatch();
            loopWatch.Start();

            Context.MakeCurrent();

            // --
            mTackConsole.OnStart();

            m_spriteManager = new DesktopSpriteManager();
            m_spriteManager.OnStart();

            mTackObjectManager = new TackObjectManager();

            mTackLightingSystem = new TackLightingSystem();
            mTackLightingSystem.OnStart();

            mTackRender = new DesktopRenderer();
            mTackRender.OnStart();

            m_tackProfiler = new TackProfiler();
            m_tackProfiler.OnStart();

            mTackPhysics = new TackPhysics((int)(1f / m_targetFrameTime));
            mTackPhysics.Start();

            m_tackInput = new TackInput();
            m_tackInput.OnStart();

            m_debugLineRenderer = new DesktopDebugLineRenderer();
            m_debugLineRenderer.OnStart();

            onStartFunction();

            mTackObjectManager.OnStart();
            mTackRender.CallGUIObjectStartMethods();

            while (GLFW.WindowShouldClose(WindowPtr) == false) {
                loopWatch.Stop();
                double lastCycleTime = loopWatch.Elapsed.TotalSeconds;
                loopWatch.Restart();

                // Read input devices, and let the OS update the window.
                ProcessWindowEvents(false);

                /*
                 * Update
                 */
                m_engineTimer.OnUpdate();

                TackProfiler.Instance.StartTimer("UserUpdate");
                onUpdateFunction();
                TackProfiler.Instance.StopTimer("UserUpdate");

                // All OnUpdate here
                m_tackProfiler.OnUpdate();
                mTackPhysics.Update();      // If issues arise, try running this below RunTackObjectUpdateMethods()
                mTackObjectManager.OnUpdate();
                mTackLightingSystem.OnUpdate();

                mTackConsole.OnUpdate();
                mTackRender.OnUpdate();
                m_tackInput.OnUpdate();

                /*
                 * Render
                 */
                m_engineTimer.OnRender();

                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.ClearColor(TackRenderer.BackgroundColour.R / 255f, TackRenderer.BackgroundColour.G / 255f, TackRenderer.BackgroundColour.B / 255f, TackRenderer.BackgroundColour.A / 255f);

                mTackConsole.OnGUIRender(); // TackConsole should be rendered above everything else, including the onGUIRenderFunction

                // All OnRender here
                mTackRender.OnRender(1f);

                m_debugLineRenderer.OnRender();

                Context.SwapBuffers();

                // Hold the thread for the specified time
                if (VSync == VSyncMode.Off) {
                    double loopTimeToThisPoint = loopWatch.Elapsed.TotalSeconds;

                    if (loopTimeToThisPoint < m_targetFrameTime) {
                        int sleepTime = (int)((m_targetFrameTime - loopTimeToThisPoint) * 1000f);
                        System.Threading.Thread.Sleep(sleepTime);
                    }
                }

                m_currentUpdateLoopIndex++;
                m_currentRenderLoopIndex++;
            }

            m_engineTimer.OnClose();

            onCloseFunction();

            mTackPhysics.Close();

            m_tackProfiler.OnClose();
            m_spriteManager.OnClose();
            mTackConsole.OnClose();
        }

        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);

            TackEngine.Core.Objects.Components.Camera.MainCamera.RenderTarget = new RectangleShape(0, 0, e.Width, e.Height);
            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            base.OnKeyDown(e);

            m_tackInput.KeyDownEvent((KeyboardKey)e.Key);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e) {
            base.OnKeyUp(e);

            m_tackInput.KeyUpEvent((KeyboardKey)e.Key);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseDown(e);

            m_tackInput.MouseDownEvent((MouseButtonKey)e.Button);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);

            m_tackInput.MouseUpEvent((MouseButtonKey)e.Button);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e) {
            base.OnMouseMove(e);

            m_tackInput.MouseMoveEvent((int)e.X, (int)e.Y);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            base.OnMouseWheel(e);

            m_tackInput.MouseScrollEvent((int)e.Offset.Y);
        }

        protected override void OnJoystickConnected(JoystickEventArgs e) {
            base.OnJoystickConnected(e);

            /*
            if (e.IsConnected) {
                m_tackInput.RegisterGamepad(new GamepadData(e.JoystickId));
            }
            */
        }
    }
}
