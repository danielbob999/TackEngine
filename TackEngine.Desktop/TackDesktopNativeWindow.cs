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
using TackEngine.Core.Audio;
using TackEngine.Desktop.Audio;
using TackEngine.Core.SceneManagement;
using OpenTK.Core;

namespace TackEngine.Desktop {
    public class TackDesktopNativeWindow : NativeWindow, IBaseTackWindow {
        public static TackDesktopNativeWindow Instance { get; private set; } = null;

        public Vector2f WindowSize { get { return new Vector2f(base.ClientSize.X, base.ClientSize.Y); } }
        public ulong CurrentUpdateLoopIndex { get { return m_currentUpdateLoopIndex; } }
        public ulong CurrentRenderLoopIndex { get { return m_currentRenderLoopIndex; } }

        private int m_targetUpdateFrequency = 60;

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
        private AudioManager m_audioManager;

        private CancellationTokenSource m_cancelTokenSource;

        public TackDesktopNativeWindow(TackEngine.Core.Engine.TackEngineInstance.InitalisationSettings settings)
            : base(new NativeWindowSettings() { Size = new OpenTK.Mathematics.Vector2i(settings.WindowSize.X, settings.WindowSize.Y) , NumberOfSamples = settings.MSAASampleCount, Title = settings.WindowTitle, WindowBorder = (OpenTK.Windowing.Common.WindowBorder)settings.WindowBorder, WindowState = (OpenTK.Windowing.Common.WindowState)settings.WindowState }) {

            // We must initialise a TackConsole instance before doing anything
            mTackConsole = new TackConsole();

            TackEngine.Core.Engine.TackEngineInstance.Initialise(this, settings, TackEngineInstance.TackEnginePlatform.Windows);

            // Set all implementations
            TackFont.FontLoadingImplementation = new DesktopTackFontLoadingImpl();

            m_currentUpdateLoopIndex = 0;
            m_currentRenderLoopIndex = 0;

            m_engineTimer = new EngineTimer();
            m_engineTimer.OnStart();

            // If VSync is enabled, set the target update/render frequencies to the VSync value
            VSync = (settings.VSync == true ? VSyncMode.On : VSyncMode.Off);

            if (!settings.VSync) {
                m_targetUpdateFrequency = settings.TargetUpdateRenderFrequency;
            } else {
                m_targetUpdateFrequency = 0;
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

            mTackPhysics = new TackPhysics(1);
            mTackPhysics.Start();

            m_tackInput = new TackInput();
            m_tackInput.OnStart();

            m_audioManager = new DesktopAudioManagerImpl();
            m_audioManager.OnStart();

            TackEngineInstance.Instance.SceneManager.LoadFirstScene();

            mTackObjectManager.OnStart();

            while (GLFW.WindowShouldClose(WindowPtr) == false) {
                double updateFrequency = m_targetUpdateFrequency == 0 ? 0 : 1 / (float)m_targetUpdateFrequency;

                double elapsed = loopWatch.Elapsed.TotalSeconds;

                if (elapsed > updateFrequency) {
                    loopWatch.Restart();

                    // Read input devices, and let the OS update the window.
                    ProcessWindowEvents(false);

                    /*
                     * Update
                     */
                    m_engineTimer.OnUpdate();

                    // All OnUpdate here
                    m_tackProfiler.OnUpdate();
                    mTackPhysics.Update();      // If issues arise, try running this below RunTackObjectUpdateMethods()
                    mTackObjectManager.OnUpdate();
                    mTackLightingSystem.OnUpdate();
                    m_audioManager.OnUpdate();

                    mTackConsole.OnUpdate();
                    mTackRender.OnUpdate();
                    m_tackInput.OnUpdate();

                    /*
                     * Render
                     */
                    m_engineTimer.OnRender();

                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    GL.ClearColor(TackRenderer.BackgroundColour.R / 255f, TackRenderer.BackgroundColour.G / 255f, TackRenderer.BackgroundColour.B / 255f, TackRenderer.BackgroundColour.A / 255f);

                    mTackConsole.OnGUIRender(); // TackConsole should be rendered above everything else, including the onGUIRenderFunction

                    // All OnRender here
                    mTackRender.OnRender(1f);

                    Context.SwapBuffers();

                    // The time we have left to the next update.
                    double timeToNextUpdate = updateFrequency - loopWatch.Elapsed.TotalSeconds;

                    if (timeToNextUpdate > 0) {
                        AccurateSleep(timeToNextUpdate, 8);
                    }

                    m_currentUpdateLoopIndex++;
                    m_currentRenderLoopIndex++;
                }
            }

            m_engineTimer.OnClose();

            mTackPhysics.Close();
            m_audioManager.OnClose();

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

        //
        private void AccurateSleep(double seconds, int expectedSchedulerPeriod) {
            const double TOLERANCE = 0.02;

            long t0 = Stopwatch.GetTimestamp();
            long target = t0 + (long)(seconds * Stopwatch.Frequency);

            double ms = (seconds * 1000) - (expectedSchedulerPeriod * TOLERANCE);
            int ticks = (int)(ms / expectedSchedulerPeriod);
            if (ticks > 0) {
                Thread.Sleep(ticks * expectedSchedulerPeriod);
            }

            while (Stopwatch.GetTimestamp() < target) {
                Thread.Yield();
            }
        }
    }
}
