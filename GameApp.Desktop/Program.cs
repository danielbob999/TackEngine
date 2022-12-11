using System;
using TackEngine.Core.GUI;
using TackEngine.Core.Input;
using TackEngine.Core.Main;
using TackEngine.Core.Math;
using TackEngine.Core.Objects;
using TackEngine.Core.Objects.Components;
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
            Camera.MainCamera.ZoomFactor = 0.5f;

            float yOffset = 10f;
            // floor stuff
            {
                TackObject obj4 = TackObject.Create("Floor", new Vector2f(300, -100));
                obj4.Scale = new Vector2f(1200, 50);
                obj4.Rotation = 0f;
                //obj4.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green, Sprite = s });
                obj4.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green });
                obj4.AddComponent(new RectanglePhysicsComponent(1f, true, false, true, 1, 0f));

                TackObject floor1 = TackObject.Create("Floor1", new Vector2f(-200, -100));
                floor1.Scale = new Vector2f(1200, 50);
                floor1.Rotation = -7f;
                //obj4.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green, Sprite = s });
                floor1.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green });
                floor1.AddComponent(new RectanglePhysicsComponent(1f, true, false, true, 1, 0f));

                TackObject floor2 = TackObject.Create("Floor2", new Vector2f(600, -100));
                floor2.Scale = new Vector2f(1200, 50);
                floor2.Rotation = 7f;
                //obj4.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green, Sprite = s });
                floor2.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green });
                floor2.AddComponent(new RectanglePhysicsComponent(1f, true, false, true, 1, 0f));
            }

            /*
            Sprite wheelSprite = Sprite.LoadFromFile("resources/tyre.png");
            wheelSprite.Create();

            // Car
            {
                Sprite circleSprite = Sprite.LoadFromFile("resources/circle.png");
                circleSprite.Create();

                // Car box
                Sprite carSprite = Sprite.LoadFromFile("resources/car.png");
                carSprite.Create();

                float heightRatio = 176f / 564f;
                TackObject body = TackObject.Create("Body", new Vector2f(0, 75 + yOffset));

                float width = 250f;
                body.Scale = new Vector2f(width, width * heightRatio);
                body.Rotation = 0f;
                body.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.White, Sprite = carSprite });
                body.AddComponent(new RectanglePhysicsComponent(1f, false, false, false, 1, 0f));

                // Wheel 1
                TackObject wheel1 = TackObject.Create("BackWheel", new Vector2f(0, 0));
                wheel1.Scale = new Vector2f(38, 38);
                wheel1.Rotation = 0f;
                //wheel1.SetParent(body);
                wheel1.LocalPosition = new Vector2f(-78, 48 + yOffset);
                wheel1.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.White, Sprite = wheelSprite });
                wheel1.AddComponent(new WheelPhysicsComponent(body.GetComponent<RectanglePhysicsComponent>()));

                // Wheel 2
                TackObject wheel2 = TackObject.Create("FrontWheel", new Vector2f(0, 0));
                wheel2.Scale = new Vector2f(38, 38);
                wheel2.Rotation = 0f;
                //wheel2.SetParent(body);
                wheel2.LocalPosition = new Vector2f(82, 48 + yOffset);
                wheel2.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.White, Sprite = wheelSprite });
                wheel2.AddComponent(new WheelPhysicsComponent(body.GetComponent<RectanglePhysicsComponent>()));
            }

            // Trailer
            {
                float heightRatio = 176f / 564f;
                TackObject trailerBody = TackObject.Create("TrailerBody", new Vector2f(300, 75 + yOffset));

                float width = 250f;
                trailerBody.Scale = new Vector2f(width, width * heightRatio);
                trailerBody.Rotation = 0f;
                trailerBody.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.White });
                trailerBody.AddComponent(new RectanglePhysicsComponent(1f, false, false, false, 0f, 0f));

                // Wheel 1
                TackObject wheel1 = TackObject.Create("BackWheel", new Vector2f(0, 0));
                wheel1.Scale = new Vector2f(38, 38);
                wheel1.Rotation = 0f;
                //wheel1.SetParent(body);
                wheel1.LocalPosition = new Vector2f(400, 48 + yOffset);
                wheel1.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Red, Sprite = wheelSprite });
                wheel1.AddComponent(new WheelPhysicsComponent(trailerBody.GetComponent<RectanglePhysicsComponent>()) { MotorEnabled = false });
            }

            // joint
            TackObject.Get("Body").AddComponent(new RevoluteJointComponent() { BodyB = TackObject.Get("TrailerBody").GetComponent<RectanglePhysicsComponent>(), LimitEnabled = true, LowerLimit = -2f, UpperLimit = 2f, Anchor = (TackObject.Get("Body").Position + new Vector2f(100f, 0f)) });
            */

            FunFactory.CreateCar(new Vector2f(0, 75));

        }

        public static void Update() {
            //DebugLineRenderer.DrawLine(new Vector2f(0, 0), new Vector2f(100, -100), Colour4b.Blue);

            //TackObject.Get("TackObject1").Rotation = TackObject.Get("TackObject2").Rotation = TackObject.Get("TackObject3").Rotation += ((float)EngineTimer.Instance.LastUpdateTime * 25f);

            float motorSpeed = 5f;

            if (TackInput.KeyHeld(KeyboardKey.A)) {
                m_motorSpeed = TackMath.Clamp(m_motorSpeed + (3f * (float)EngineTimer.Instance.LastUpdateTime), -motorSpeed, motorSpeed);
            } else if (TackInput.KeyHeld(KeyboardKey.D)) {
                m_motorSpeed = TackMath.Clamp(m_motorSpeed - (3f * (float)EngineTimer.Instance.LastUpdateTime), -motorSpeed, motorSpeed);
            } else {
                if (m_motorSpeed > 0) {
                    m_motorSpeed = TackMath.Clamp(m_motorSpeed - (0.7f * (float)EngineTimer.Instance.LastUpdateTime), 0f, motorSpeed);
                } else {
                    m_motorSpeed = TackMath.Clamp(m_motorSpeed + (0.7f * (float)EngineTimer.Instance.LastUpdateTime), -motorSpeed, 0f);
                }
            }

            TackObject.Get("CarBackWheel").GetComponent<WheelPhysicsComponent>().MotorSpeed = m_motorSpeed;
            TackObject.Get("CarFrontWheel").GetComponent<WheelPhysicsComponent>().MotorEnabled = false;

            TackObject.Get("CarBackWheel").GetComponent<WheelPhysicsComponent>().MaxTorque = 50f;
            TackObject.Get("CarFrontWheel").GetComponent<WheelPhysicsComponent>().MaxTorque = 50f;
        }

        public static void Close() {

        }
    }
}