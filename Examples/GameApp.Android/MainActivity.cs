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

        protected override void OnCreate(Bundle? savedInstanceState) {
            base.OnCreate(savedInstanceState);
        }

        public override void OnEngineStart() {
            base.OnEngineStart();
        }

        public override void OnEngineUpdate() {
            base.OnEngineStart();
        }

        public override void OnEngineClose() {
            base.OnEngineStart();
        }
    }
}