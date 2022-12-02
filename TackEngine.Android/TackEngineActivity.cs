using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TackEngine.Core.Engine;
using TackEngine.Core.GUI;
using TackEngine.Core.Input;
using TackEngine.Core.Main;
using TackEngine.Core.Objects;
using TackEngine.Core.Physics;
using TackEngine.Core.Renderer;

namespace TackEngine.Android {
    [Activity(Label = "TackEngineActivity")]
    public class TackEngineActivity : Activity, IBaseTackWindow {
        public Vector2f WindowSize { get { return new Vector2f(Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.HeightPixels); } }

        public static TackEngineActivity Instance { get; private set; }

        // Modules
        internal TackConsole mTackConsole;
        internal TackPhysics mTackPhysics;
        internal TackObjectManager mTackObjectManager;
        internal TackRenderer mTackRender;
        internal TackLightingSystem mTackLightingSystem;
        internal TackEngine.Core.Main.EngineTimer m_engineTimer;
        internal TackProfiler m_tackProfiler;
        internal TackInput m_tackInput;
        internal AndroidSpriteManager m_spriteManager;

        internal TackEngine.Core.Engine.EngineDelegates.OnStart onStartFunction;
        internal TackEngine.Core.Engine.EngineDelegates.OnStart onUpdateFunction;
        internal TackEngine.Core.Engine.EngineDelegates.OnStart onCloseFunction;

        private MySurfaceView m_glView;

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            onStartFunction = OnEngineStart;
            onUpdateFunction = OnEngineUpdate;
            onCloseFunction = OnEngineClose;

            mTackConsole = new TackConsole();

            // Set implementations
            TackFont.FontLoadingImplementation = new AndroidTackFontLoadingImpl();

            Instance = this;
            AndroidContext.CurrentContext = this;
            AndroidContext.CurrentAssetManager = Assets;

            TackEngine.Core.Engine.TackEngineInstance.Initialise(this);

            m_glView = new MySurfaceView(this);

            // Set our view from the "main" layout resource
            SetContentView(m_glView);

            base.RequestedOrientation = global::Android.Content.PM.ScreenOrientation.Sensor;
            //RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            
            // Hides the app action bar
            if (this.ActionBar != null) {
                this.ActionBar.Hide();
            }

            // removes the android status bar (battery icon/notifications)
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

            System.Diagnostics.Debug.WriteLine("-------------- Started TackEngineActivity " + TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.ToString());
        }

        public virtual void OnEngineStart() {
            

        }

        public virtual void OnEngineUpdate() {

        }

        public virtual void OnEngineClose() {

        }
    }
}