using Android.Content;
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Opengl.GLSurfaceView;

namespace TackEngine.Android {
    public class MySurfaceView : GLSurfaceView {
        private MyGLRenderer renderer;

        public MySurfaceView(Context context) : base(context) {
            // Create an OpenGL ES 2.0 context
            SetEGLContextClientVersion(2);

            renderer = new MyGLRenderer();

            // Set the Renderer for drawing on the GLSurfaceView
            SetRenderer(renderer);
        }
    }
}
