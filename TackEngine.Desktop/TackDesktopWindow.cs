using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using TackEngineLib.Main;
using TackEngineLib.Objects;
using TackEngineLib.Input;
using TackEngineLib.Renderer;
using TackEngineLib.GUI;
using TackEngineLib.Physics;
using OpenTK.Platform;
using OpenTK.Input;
using TackEngineLib.Engine;
using System.Diagnostics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;

namespace TackEngine.Desktop {
    public class TackGameWindow : GameWindow, IBaseTackWindow {
        // Reference to the current GameWindow instance. CANNOT BE CHANGED
        private GameWindow gameWindowRef;
        internal static TackGameWindow Instance;

        private EngineDelegates.OnStart onStartFunction;
        private EngineDelegates.OnUpdate onUpdateFunction;
        private EngineDelegates.OnGUIRender onGUIRenderFunction;
        private EngineDelegates.OnClose onCloseFunction;

        // Modules
        private TackConsole mTackConsole;
        private TackPhysics mTackPhysics;
        private TackObjectManager mTackObjectManager;
        private TackRenderer mTackRender;
        private TackLightingSystem mTackLightingSystem;
        private TackEngineLib.Main.EngineTimer m_engineTimer;
        private TackProfiler m_tackProfiler;
        private TackInput m_tackInput;
        private DesktopSpriteManager m_spriteManager;

        public Vector2f WindowSize { get { return new Vector2f(base.ClientSize.X, base.ClientSize.Y); } }

        public double TimeSinceLastUpdate { get; private set; }
        public double TimeSinceLastRender { get; private set; }

        public TackGameWindow(TackEngineLib.Engine.TackEngine.InitalisationSettings settings, EngineDelegates.OnStart startFn, EngineDelegates.OnUpdate updateFn, EngineDelegates.OnClose closeFn) 
            : base(new GameWindowSettings() { RenderFrequency = settings.TargetRenderFrequency, UpdateFrequency = settings.TargetUpdateFrequency }, new NativeWindowSettings() { Size = new OpenTK.Mathematics.Vector2i(settings.WindowSize.X, settings.WindowSize.Y), NumberOfSamples = settings.MSAASampleCount, Title = settings.WindowTitle, WindowBorder = (OpenTK.Windowing.Common.WindowBorder)settings.WindowBorder, WindowState = (OpenTK.Windowing.Common.WindowState)settings.WindowState }) {

            // We must initialise a TackConsole instance before doing anything
            mTackConsole = new TackConsole();

            TackEngineLib.Engine.TackEngine.Initialise(this);

            // Set all implementations
            TackFont.FontLoadingImplementation = new DesktopTackFontLoadingImpl();
            
            onStartFunction = startFn;
            onUpdateFunction = updateFn;
            onCloseFunction = closeFn;

            m_engineTimer = new EngineTimer();
            m_engineTimer.OnStart();

            // If VSync is enabled, set the target update/render frequencies to the VSync value
            VSync = (settings.VSync == true ? VSyncMode.On : VSyncMode.Off);

            Instance = this;
        }

        protected override void OnLoad() {
            base.OnLoad();

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

            mTackPhysics = new TackPhysics();
            mTackPhysics.Start();

            m_tackInput = new TackInput();
            m_tackInput.OnStart();

            onStartFunction();

            mTackObjectManager.OnStart();
        }

        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);
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
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);
            m_engineTimer.OnRender();

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(TackRenderer.BackgroundColour.R / 255f, TackRenderer.BackgroundColour.G / 255f, TackRenderer.BackgroundColour.B / 255f, TackRenderer.BackgroundColour.A / 255f);

            mTackConsole.OnGUIRender(); // TackConsole should be rendered above everything else, including the onGUIRenderFunction

            // All OnRender here
            mTackRender.OnRender(e.Time);

            this.SwapBuffers();
        }

        protected override void OnUnload() {
            base.OnUnload();

            m_engineTimer.OnClose();

            onCloseFunction();

            mTackPhysics.Close();

            m_tackProfiler.OnClose();
            m_spriteManager.OnClose();
            mTackConsole.OnClose();
        }


        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);

            TackEngineLib.Objects.Components.Camera.MainCamera.RenderTarget = new RectangleShape(0, 0, base.Size.X, base.Size.Y);
            GL.Viewport(0, 0, base.Size.X, base.Size.Y);

            Debug.WriteLine(new Vector2f(base.Size.X, base.Size.Y).ToString());
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

            m_tackInput.MouseScrollEvent((int)e.OffsetY);
        }

        /*
        protected override void OnJoystickConnected(JoystickEventArgs e) {
            base.OnJoystickConnected(e);

            if (e.IsConnected) {
                m_tackInput.RegisterGamepad(new GamepadData(e.JoystickId));
            }
        }
        */

        public static void RestartModule(string moduleName, bool keepState) {
            if (moduleName == "TackRenderer") {
                Instance.mTackRender.OnClose();
                Instance.mTackRender = new DesktopRenderer();
                Instance.mTackRender.OnStart();
                return;
            }
        }
    }
}
