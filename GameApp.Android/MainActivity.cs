using System.Security.AccessControl;
using TackEngine.Core.Main;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Objects;
using Android.OS;
using TackEngine.Android;
using TackEngine.Core.GUI;
using TackEngine.Core.Input;
using Android.Content.PM;

namespace GameApp.Android {
    [Activity(Label = "@string/app_name", MainLauncher = true, ConfigurationChanges = (ConfigChanges.ScreenSize | ConfigChanges.Orientation))]
    public class MainActivity : TackEngine.Android.TackEngineActivity {
        TackObject obj4;
        private Vector2f m_dragStart;
        private bool m_isDragging = false;

        protected override void OnCreate(Bundle? savedInstanceState) {
            base.OnCreate(savedInstanceState);
        }

        public override void OnEngineStart() {
            base.OnEngineStart();

            Camera.MainCamera.ZoomFactor *= 2.5f;

            Sprite s = Sprite.LoadFromFile("resources/texture1.png");
            s.Create();

            /*
            TackObject obj1 = TackObject.Create("TackObject1", new Vector2f(0, 50));
            obj1.Scale = new Vector2f(35, 35);
            obj1.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Blue});

            TackObject obj2 = TackObject.Create("TackObject2", new Vector2f(-100, -50));
            obj2.Scale = new Vector2f(35, 35);
            obj2.AddComponent(new SpriteRendererComponent() { Colour = new Colour4b(245, 245, 0, 255) });

            TackObject obj3 = TackObject.Create("TackObject3", new Vector2f(100, -50));
            obj3.Scale = new Vector2f(35, 35);
            obj3.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green });
            */

            
            obj4 = TackObject.Create("Floor", new Vector2f(100, -100));
            obj4.Scale = new Vector2f(1000, 50);
            obj4.Rotation = 35f;
            //obj4.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green, Sprite = s });
            obj4.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green });
            obj4.AddComponent(new RectanglePhysicsComponent(1f, true, false, true, 1, 0f));

            TackObject floor2 = TackObject.Create("Box1", new Vector2f(-100, -100));
            floor2.Scale = new Vector2f(1000, 50);
            floor2.Rotation = -35f;
            floor2.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green });
            floor2.AddComponent(new RectanglePhysicsComponent(1f, true, false, true, 1, 0f));

            /*
            TackObject box = TackObject.Create("Box1", new Vector2f(0, 0));
            box.Scale = new Vector2f(35, 35);
            box.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Red });
            box.AddComponent(new RectanglePhysicsComponent(1f, false, true, false, 1f, 0f));
            */
            Sprite circleSprite = Sprite.LoadFromFile("resources/circle.png");
            circleSprite.Create();

            TackObject circle = TackObject.Create("Circle1", new Vector2f(-60, 0));
            circle.Scale = new Vector2f(35, 35);
            circle.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Blue, Sprite = circleSprite });
            circle.AddComponent(new CirclePhysicsComponent(1f, false, true, false, 1f, 0f));


            /*
            GUIBox ta = new GUIBox();
            ta.Size = new Vector2f(100, 100f);
            ta.Position = new Vector2f(200, 200);

            GUITextArea ta1 = new GUITextArea();
            ta1.Size = new Vector2f(500f, 500f);
            ta1.Text = "This is some text\nOn a new line!";
            ta1.Position = new Vector2f(400, 200);
            //ta1.NormalStyle.Colour = new Colour4b(0, 0, 0, 0);
            ta1.NormalStyle.FontColour = new Colour4b(255, 0, 0, 255);

            GUIButton but = new GUIButton();
            but.Size = new Vector2f(400, 100);
            but.Position = new Vector2f(400, 800);
            but.OnClickedEvent += OnButtonPress;*/
        }

        public override void OnEngineUpdate() {
            base.OnEngineStart();

            Vector2f camMoveAmnt = new Vector2f();

            // touch drag logic
            if (TackInput.TouchDown()) {
                //System.Diagnostics.Debug.WriteLine("Touch down");
                m_isDragging = true;
                m_dragStart = TackInput.Instance.TouchPosition.ToVector2f();
            }

            if (m_isDragging) {
                Vector2f mouseDragAmount = TackInput.Instance.TouchPosition.ToVector2f() - m_dragStart;
                camMoveAmnt += (mouseDragAmount * new Vector2f(-0.5f, 0.5f));
                m_dragStart = TackInput.Instance.TouchPosition.ToVector2f();
            }

            if (TackInput.TouchUp()) {
                m_isDragging = false;
                //System.Diagnostics.Debug.WriteLine("Touch down");
            }

            Camera.MainCamera.GetParent().Position += camMoveAmnt;

            //TackObject.Get("TackObject1").Rotation = TackObject.Get("TackObject2").Rotation = TackObject.Get("TackObject3").Rotation += ((float)EngineTimer.Instance.LastUpdateTime * 25f);
        }

        public override void OnEngineClose() {
            base.OnEngineStart();
        }

        public void OnButtonPress(object sender, EventArgs e) {
            System.Diagnostics.Debug.WriteLine("Button pressed");
        }
    }
}