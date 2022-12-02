using System;
using TackEngine.Core.GUI;
using TackEngine.Core.Main;
using TackEngine.Core.Objects;
using TackEngine.Core.Objects.Components;

namespace GameApp.Desktop {
    internal class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello, World!");

            TackEngine.Desktop.TackGameWindow window = new TackEngine.Desktop.TackGameWindow(new TackEngine.Core.Engine.TackEngineInstance.InitalisationSettings(),
                Start, Update, Close);
            window.Run();
        }

        public static void Start() {
            Sprite s = Sprite.LoadFromFile("resources/texture1.png");
            s.Create();
            s.Filter = Sprite.SpriteFilter.Linear;
            /*
            TackObject obj1 = TackObject.Create("TackObject1", new Vector2f(0, 50));
            obj1.Scale = new Vector2f(35, 35f);
            obj1.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Blue, Sprite = s });

            TackObject obj2 = TackObject.Create("TackObject2", new Vector2f(-100, -50));
            obj2.Scale = new Vector2f(35, 35f);
            obj2.AddComponent(new SpriteRendererComponent() { Colour = new Colour4b(245, 245, 0, 255), Sprite = s });

            TackObject obj3 = TackObject.Create("TackObject3", new Vector2f(100, -50));
            obj3.Scale = new Vector2f(35, 35f);
            obj3.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green, Sprite = s });
            */

            TackObject obj4 = TackObject.Create("TackObject4", new Vector2f(100, -50));
            obj4.Scale = new Vector2f(100, 100);
            obj4.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green, Sprite = s });

            GUITextArea ta = new GUITextArea();
            ta.Text = "This is some text\ntesting out a ta!";
            ta.Size = new Vector2f(100, 100f);
            ta.Position = new Vector2f(5, 100);
        }

        public static void Update() {
            //TackObject.Get("TackObject1").Rotation = TackObject.Get("TackObject2").Rotation = TackObject.Get("TackObject3").Rotation += ((float)EngineTimer.Instance.LastUpdateTime * 25f);
        }

        public static void Close() {

        }
    }
}