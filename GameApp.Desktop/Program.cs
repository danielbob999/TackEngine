using System;
using TackEngine.Core.GUI;
using TackEngine.Core.Input;
using TackEngine.Core.Main;
using TackEngine.Core.Math;
using TackEngine.Core.Objects;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Physics;
using TackEngine.Core.Renderer;
using TackEngine.Core.Source.Objects;
using tainicom.Aether.Physics2D.Dynamics;

namespace GameApp.Desktop {
    internal class Program {
        static float m_motorSpeed = 0;

        static bool colliding = false;

        static void Main(string[] args) {
            Console.WriteLine("Hello, World!");

            TackEngine.Core.Engine.TackEngineInstance.InitalisationSettings settings = new TackEngine.Core.Engine.TackEngineInstance.InitalisationSettings();
            settings.TargetUpdateRenderFrequency = 60;
            settings.VSync = true;

            /*
            TackEngine.Desktop.TackGameWindow window = new TackEngine.Desktop.TackGameWindow(settings,
                Start, Update, Close);
            window.Run();
            */
            

            TackEngine.Desktop.TackDesktopNativeWindow window = new TackEngine.Desktop.TackDesktopNativeWindow(settings,
                Start, Update, Close);
            window.Run();
            
        }

        public static void Start() {
            Camera.MainCamera.ZoomFactor = 1.1f;
        }

        public static void Update() {
        }

        public static void Close() {

        }
    }
}