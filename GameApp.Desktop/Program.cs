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
            Camera.MainCamera.ZoomFactor = 1.2f;

            // ground rendering object
            Sprite groundSprite = Sprite.LoadFromFile("mapGround.png");
            groundSprite.Create();


            TackObject.Create("O", new Vector2f(0, 0), new Vector2f(15000, 2500)).AddComponent(new SpriteRendererComponent() { Colour = Colour4b.White, Sprite = groundSprite });
            //TackObject.Create("O", new Vector2f(0, 0), new Vector2f(150, 150)).AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green });
        }

        public static void Update() {
            //DebugLineRenderer.DrawLine(new Vector2f(0, 0), new Vector2f(100, -100), Colour4b.Blue);

            //TackObject.Get("TackObject1").Rotation = TackObject.Get("TackObject2").Rotation = TackObject.Get("TackObject3").Rotation += ((float)EngineTimer.Instance.LastUpdateTime * 25f);

            Vector2f move = new Vector2f();

            if (TackInput.KeyHeld(KeyboardKey.D)) {
                move.X += 10f;
            }

            if (TackInput.KeyHeld(KeyboardKey.A)) {
                move.X -= 10f;
            }


            Camera.MainCamera.GetParent().Position += move;
        }

        public static void Close() {

        }
    }
}