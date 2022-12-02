using System.Security.AccessControl;
using TackEngine.Core.Main;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Objects;
using Android.OS;
using TackEngine.Android;
using TackEngine.Core.GUI;

namespace GameApp.Android {
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : TackEngine.Android.TackEngineActivity {
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

            
            TackObject obj4 = TackObject.Create("TackObject3", new Vector2f(0, 0));
            obj4.Scale = new Vector2f(100, 100);
            obj4.AddComponent(new SpriteRendererComponent() { Colour = Colour4b.Green, Sprite = s });
            
            


            //GUIBox ta = new GUIBox();
            //ta.Size = new Vector2f(100, 100f);
            //ta.Position = new Vector2f(5, 100);
        }

        public override void OnEngineUpdate() {
            base.OnEngineStart();

            //TackObject.Get("TackObject1").Rotation = TackObject.Get("TackObject2").Rotation = TackObject.Get("TackObject3").Rotation += ((float)EngineTimer.Instance.LastUpdateTime * 25f);
        }

        public override void OnEngineClose() {
            base.OnEngineStart();
        }
    }
}