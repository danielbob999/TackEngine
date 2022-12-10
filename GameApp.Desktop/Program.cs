using System;
using TackEngine.Core.GUI;
using TackEngine.Core.Input;
using TackEngine.Core.Main;
using TackEngine.Core.Math;
using TackEngine.Core.Objects;
using TackEngine.Core.Objects.Components;

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
            TackObject obj4 = TackObject.Create("Floor", new Vector2f(100, -100));
            obj4.Scale = new Vector2f(1000, 50);
            obj4.Rotation = 0f;
            //obj4.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green, Sprite = s });
            obj4.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green });
            obj4.AddComponent(new RectanglePhysicsComponent(1f, true, false, true, 1, 0f));

            Sprite circleSprite = Sprite.LoadFromFile("resources/circle.png");
            circleSprite.Create();

            // Car box
            TackObject body = TackObject.Create("Body", new Vector2f(0, 75));
            body.Scale = new Vector2f(100, 50);
            body.Rotation = 0f;
            body.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green });
            body.AddComponent(new RectanglePhysicsComponent(1f, false, false, false, 1, 0f));

            // Wheel 1
            TackObject wheel1 = TackObject.Create("BackWheel", new Vector2f(-70, 0));
            wheel1.Scale = new Vector2f(35, 35);
            wheel1.Rotation = 0f;
            //wheel1.SetParent(body);
            wheel1.LocalPosition = new Vector2f(-30, 50);
            wheel1.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Blue, Sprite = circleSprite });
            wheel1.AddComponent(new WheelPhysicsComponent(body.GetComponent<RectanglePhysicsComponent>()));

            // Wheel 2
            TackObject wheel2 = TackObject.Create("FrontWheel", new Vector2f(70, 0));
            wheel2.Scale = new Vector2f(35, 35);
            wheel2.Rotation = 0f;
            //wheel2.SetParent(body);
            wheel2.LocalPosition = new Vector2f(30, 50);
            wheel2.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Red, Sprite = circleSprite });
            wheel2.AddComponent(new WheelPhysicsComponent(body.GetComponent<RectanglePhysicsComponent>()));
        }

        public static void Update() {
            //TackObject.Get("TackObject1").Rotation = TackObject.Get("TackObject2").Rotation = TackObject.Get("TackObject3").Rotation += ((float)EngineTimer.Instance.LastUpdateTime * 25f);

            if (TackInput.KeyHeld(KeyboardKey.A)) {
                m_motorSpeed = TackMath.Clamp(m_motorSpeed + (3f * (float)EngineTimer.Instance.LastUpdateTime), -2f, 2f);
            } else if (TackInput.KeyHeld(KeyboardKey.D)) {
                m_motorSpeed = TackMath.Clamp(m_motorSpeed - (3f * (float)EngineTimer.Instance.LastUpdateTime), -2f, 2f);
            } else {
                if (m_motorSpeed > 0) {
                    m_motorSpeed = TackMath.Clamp(m_motorSpeed - (0.7f * (float)EngineTimer.Instance.LastUpdateTime), 0f, 2f);
                } else {
                    m_motorSpeed = TackMath.Clamp(m_motorSpeed + (0.7f * (float)EngineTimer.Instance.LastUpdateTime), -2f, 0f);
                }
            }

            TackObject.Get("BackWheel").GetComponent<WheelPhysicsComponent>().MotorSpeed = m_motorSpeed;
            TackObject.Get("FrontWheel").GetComponent<WheelPhysicsComponent>().MotorSpeed = m_motorSpeed;
        }

        public static void Close() {

        }
    }
}