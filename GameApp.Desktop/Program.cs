using System;
using TackEngine.Core.GUI;
using TackEngine.Core.Input;
using TackEngine.Core.Main;
using TackEngine.Core.Objects;
using TackEngine.Core.Objects.Components;

namespace GameApp.Desktop {
    internal class Program {
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
            TackObject obj4 = TackObject.Create("Floor", new Vector2f(100, -100));
            obj4.Scale = new Vector2f(1000, 50);
            obj4.Rotation = 0f;
            //obj4.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green, Sprite = s });
            obj4.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green });
            obj4.AddComponent(new RectanglePhysicsComponent(1f, true, false, true, 1, 0f));

            /*
            TackObject floor2 = TackObject.Create("Box1", new Vector2f(-100, -100));
            floor2.Scale = new Vector2f(1000, 50);
            floor2.Rotation = -35f;
            floor2.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green });
            floor2.AddComponent(new RectanglePhysicsComponent(1f, true, false, true, 1, 0f));
            */

            /*
            TackObject box = TackObject.Create("Box1", new Vector2f(0, 0));
            box.Scale = new Vector2f(35, 35);
            box.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Red });
            box.AddComponent(new RectanglePhysicsComponent(1f, false, true, false, 1f, 0f));
            */
            Sprite circleSprite = Sprite.LoadFromFile("resources/circle.png");
            circleSprite.Create();

            TackObject circle = TackObject.Create("Circle1", new Vector2f(-60, 200));
            circle.Scale = new Vector2f(35, 35);
            circle.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Blue, Sprite = circleSprite });
            circle.AddComponent(new CirclePhysicsComponent(1f, false, true, false, 1f, 0f));
        }

        public static void Update() {
            //TackObject.Get("TackObject1").Rotation = TackObject.Get("TackObject2").Rotation = TackObject.Get("TackObject3").Rotation += ((float)EngineTimer.Instance.LastUpdateTime * 25f);
        }

        public static void Close() {

        }
    }
}