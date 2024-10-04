using Android.Graphics;
using Android.Hardware.Lights;
using Android.Opengl;
using Android.Util;
using Java.Lang;
using Javax.Microedition.Khronos.Opengles;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Android.Audio;
using TackEngine.Core.Engine;
using TackEngine.Core.Input;
using TackEngine.Core.Main;
using TackEngine.Core.Objects;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Physics;
using TackEngine.Core.Renderer;
using static Android.Icu.Text.ListFormatter;

namespace TackEngine.Android.Renderer
{
    public class MyGLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
    {

        public void OnSurfaceCreated(IGL10? gl, Javax.Microedition.Khronos.Egl.EGLConfig? config)
        {
            // Create your application here

            //gl.GlEnableClientState(IGL10.GlColorArray);

            //gl.GlEnableClientState(IGL10.GlVertexArray);
            //gl.GlEnableClientState(IGL10.GlTextureCoordArray);

            TackEngineActivity.Instance.m_engineTimer = new EngineTimer();
            TackEngineActivity.Instance.m_engineTimer.OnStart();

            TackEngineActivity.Instance.mTackConsole.OnStart();

            TackEngineActivity.Instance.m_spriteManager = new AndroidSpriteManager();
            TackEngineActivity.Instance.m_spriteManager.OnStart();

            //mTackConsole.OnStart();

            TackEngineActivity.Instance.mTackObjectManager = new TackObjectManager();

            TackEngineActivity.Instance.mTackLightingSystem = new TackLightingSystem();
            TackEngineActivity.Instance.mTackLightingSystem.OnStart();

            TackEngineActivity.Instance.mTackRender = new TackRenderer(new AndroidRenderingBehaviour(), new AndroidLineRenderingBehaviour(), new TackGUI(), new AndroidShaderImpl());
            TackEngineActivity.Instance.mTackRender.OnStart();

            TackEngineActivity.Instance.m_tackProfiler = new TackProfiler();
            TackEngineActivity.Instance.m_tackProfiler.OnStart();

            TackEngineActivity.Instance.mTackPhysics = new TackPhysics(60);
            TackEngineActivity.Instance.mTackPhysics.Start();

            TackEngineActivity.Instance.m_tackInput = new TackInput();
            TackEngineActivity.Instance.m_tackInput.OnStart();

            TackEngineActivity.Instance.m_audioManager = new AndroidAudioManagerImpl();
            TackEngineActivity.Instance.m_audioManager.OnStart();

            TackEngineInstance.Instance.SceneManager.LoadFirstScene();

            TackEngineActivity.Instance.mTackObjectManager.OnStart();
        }

        public void OnDrawFrame(IGL10? unused)
        {
            // Redraw background color
            //OpenTK.Graphics.ES30.GL.ClearColor(1f, 0f, 0f, 1f);
            //OpenTK.Graphics.ES30.GL.Clear(OpenTK.Graphics.ES30.ClearBufferMask.ColorBufferBit);

            // ----------
            //  updating
            // ----------

            TackEngineActivity.Instance.m_engineTimer.OnUpdate();

            // All OnUpdate here
            TackEngineActivity.Instance.m_tackProfiler.OnUpdate();
            TackEngineActivity.Instance.mTackPhysics.Update();      // If issues arise, try running this below RunTackObjectUpdateMethods()
            TackEngineActivity.Instance.mTackObjectManager.OnUpdate();
            TackEngineActivity.Instance.mTackLightingSystem.OnUpdate();
            TackEngineActivity.Instance.m_audioManager.OnUpdate();

            TackEngineActivity.Instance.mTackConsole.OnUpdate();
            TackEngineActivity.Instance.mTackRender.OnUpdate();
            TackEngineActivity.Instance.m_tackInput.OnUpdate();

            // ----------
            //  Renderering
            // ----------

            TackEngineActivity.Instance.m_engineTimer.OnRender();

            //OpenTK.Graphics.ES30.GL.ClearColor(1f, 0f, 0f, 1f);
            OpenTK.Graphics.ES30.GL.Clear(OpenTK.Graphics.ES30.ClearBufferMask.ColorBufferBit | OpenTK.Graphics.ES30.ClearBufferMask.DepthBufferBit);
            OpenTK.Graphics.ES30.GL.ClearColor(TackRenderer.Instance.BackgroundColour.R / 255f, TackRenderer.Instance.BackgroundColour.G / 255f, TackRenderer.Instance.BackgroundColour.B / 255f, TackRenderer.Instance.BackgroundColour.A / 255f);

            TackEngineActivity.Instance.mTackConsole.OnGUIRender(); // TackConsole should be rendered above everything else, including the onGUIRenderFunction

            // All OnRender here
            TackEngineActivity.Instance.mTackRender.OnRender(TackEngineActivity.Instance.m_engineTimer.LastRenderTime);

            TackEngineActivity.Instance.m_currentUpdateLoopIndex++;
            TackEngineActivity.Instance.m_currentRenderLoopIndex++;
        }

        public void OnSurfaceChanged(IGL10? unused, int width, int height)
        {
            OpenTK.Graphics.ES30.GL.Viewport(0, 0, width, height);

            TackEngineActivity.Instance.WindowSize = new Vector2f(width, height);

            Core.Objects.Components.Camera c = Core.Objects.Components.Camera.MainCamera;
            c.RenderTarget = new RectangleShape(0, 0, width, height);
        }
    }
}
