using Android.Content;
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Java.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Opengl.GLSurfaceView;

namespace TackEngine.Android {
    public class MySurfaceView : GLSurfaceView {
        private MyGLRenderer m_renderer;

        public MySurfaceView(Context context) : base(context) {
            // Create an OpenGL ES 2.0 context
            SetEGLContextClientVersion(2);

            m_renderer = new MyGLRenderer();

            // Set the Renderer for drawing on the GLSurfaceView
            SetRenderer(m_renderer);
        }

        public override bool OnTouchEvent(MotionEvent? e) {
            if (e == null) {
                return base.OnTouchEvent(e);
            }

            if (e.Action == MotionEventActions.Down) {
                TackEngineActivity.Instance.m_tackInput.TouchDownEvent(new Core.Main.Vector2i((int)e.RawX, (int)e.RawY));
            }

            if (e.Action == MotionEventActions.Up) {
                TackEngineActivity.Instance.m_tackInput.TouchUpEvent(new Core.Main.Vector2i((int)e.RawX, (int)e.RawY));
            }

            if (e.Action == MotionEventActions.Move) {
                TackEngineActivity.Instance.m_tackInput.TouchDragEvent(new Core.Main.Vector2i((int)e.RawX, (int)e.RawY));
            }

            return true;
        }
    }
}
