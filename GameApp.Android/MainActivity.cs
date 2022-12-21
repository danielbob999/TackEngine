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
        private Vector2f m_dragStart;
        private bool m_isDragging = false;

        protected override void OnCreate(Bundle? savedInstanceState) {
            base.OnCreate(savedInstanceState);
        }

        public override void OnEngineStart() {
            base.OnEngineStart();
        }

        public override void OnEngineUpdate() {
            base.OnEngineStart();

            Vector2f camMoveAmnt = new Vector2f();

            // touch drag logic
            if (TackInput.TouchDown()) {
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
            }

            Camera.MainCamera.GetParent().Position += camMoveAmnt;
        }

        public override void OnEngineClose() {
            base.OnEngineStart();
        }
    }
}