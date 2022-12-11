using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Org.Apache.Http.Cookies;
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
    [Activity(Label = "TackEngineActivity", ConfigurationChanges = (ConfigChanges.ScreenSize | ConfigChanges.Orientation))]
    public class TackEngineActivity : Activity, IBaseTackWindow, ISensorEventListener {
        public Vector2f WindowSize { get; set; }

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
        internal DebugLineRenderer m_debugLineRenderer;

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

            TackEngine.Core.Engine.TackEngineInstance.Initialise(this, TackEngineInstance.TackEnginePlatform.Android);

            m_glView = new MySurfaceView(this);

            // Set our view from the "main" layout resource
            SetContentView(m_glView);

            //base.RequestedOrientation = global::Android.Content.PM.ScreenOrientation.Sensor;
            base.RequestedOrientation = global::Android.Content.PM.ScreenOrientation.Landscape;
            //RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;

            // Register this class for sensor callbacks
            SensorManager sm = (SensorManager)GetSystemService(Context.SensorService);

            if (sm.GetSensorList(SensorType.Accelerometer).Count != 0) {
                Sensor s = sm.GetSensorList(SensorType.Accelerometer)[0];
                sm.RegisterListener(this, s, SensorDelay.Normal);
            }

            if (sm.GetSensorList(SensorType.Gyroscope).Count != 0) {
                Sensor s = sm.GetSensorList(SensorType.Gyroscope)[0];
                sm.RegisterListener(this, s, SensorDelay.Normal);
            }

            // Hides the app action bar
            if (this.ActionBar != null) {
                this.ActionBar.Hide();
            }

            // removes the android status bar (battery icon/notifications)
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

            System.Diagnostics.Debug.WriteLine("-------------- Started TackEngineActivity " + TackEngine.Core.Engine.TackEngineInstance.Instance.Window.WindowSize.ToString());
        }

        public void OnAccuracyChanged(Sensor? sensor, [GeneratedEnum] SensorStatus accuracy) {
            //throw new NotImplementedException();
        }

        public void OnSensorChanged(SensorEvent? e) {
            if (e == null) {
                return;
            }

            //System.Diagnostics.Debug.WriteLine("Sensor changed " + e.Sensor.Type);

            if (e.Sensor.Type == SensorType.Gyroscope) {
                //System.Diagnostics.Debug.WriteLine(e.Values[0].ToString() + " " + e.Values[1].ToString() + " " + e.Values[2].ToString());

                if (m_tackInput != null) {
                    m_tackInput.GyroscopeChangeEvent(new Vector3(e.Values[0], e.Values[1], e.Values[2]), e.Timestamp);
                }

            }
        }

        public virtual void OnEngineStart() {
            

        }

        public virtual void OnEngineUpdate() {

        }

        public virtual void OnEngineClose() {

        }
    }
}