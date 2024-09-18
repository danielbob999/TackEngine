/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngine.Core.Engine;
using TackEngine.Core.Main;
using TackEngine.Core.Renderer;
using TackEngine.Core.Source.Renderer;

namespace TackEngine.Core.Objects.Components
{
    /// <summary>
    /// The component that draws the world view to the screen
    /// </summary>
    public class Camera : TackComponent
    {
        public static Camera MainCamera {
            get { return TackRenderer.Instance.Cameras[0]; }
            set { 
                TackRenderer.Instance.Cameras[0] = value;
                TackRenderer.Instance.Cameras[0].RenderTarget = GetRenderTargetForSplitScreenMode(0, TackRenderer.Instance.CurrentSplitScreenMode);
            }
        }

        public static Camera SecondCamera {
            get { return TackRenderer.Instance.Cameras[1]; }
            set { 
                TackRenderer.Instance.Cameras[1] = value;
                TackRenderer.Instance.Cameras[1].RenderTarget = GetRenderTargetForSplitScreenMode(1, TackRenderer.Instance.CurrentSplitScreenMode);
            }
        }

        public static Camera ThirdCamera {
            get { return TackRenderer.Instance.Cameras[2]; }
            set { 
                TackRenderer.Instance.Cameras[2] = value;
                TackRenderer.Instance.Cameras[2].RenderTarget = GetRenderTargetForSplitScreenMode(2, TackRenderer.Instance.CurrentSplitScreenMode);
            }
        }

        public static Camera FourthCamera {
            get { return TackRenderer.Instance.Cameras[3]; }
            set {
                TackRenderer.Instance.Cameras[3] = value;
                TackRenderer.Instance.Cameras[3].RenderTarget = GetRenderTargetForSplitScreenMode(3, TackRenderer.Instance.CurrentSplitScreenMode);
            }
        }

        private float m_zoomFactor;
        private RectangleShape m_renderTarget;

        internal Physics.AABB BoundingBoxInWorld {
            get {
                TackObject parent = GetParent();

                return new Physics.AABB(
                    new Vector2f(parent.Position.X - ((RenderTarget.Width / 2f) / ZoomFactor), parent.Position.Y - ((RenderTarget.Height / 2f) / ZoomFactor)),
                    new Vector2f(parent.Position.X + ((RenderTarget.Width / 2f) / ZoomFactor), parent.Position.Y + (RenderTarget.Height / 2f) / ZoomFactor));
            }
        }

        /// <summary>
        /// Gets/Sets the shape of the render target of this Camera.
        /// Note: Currently, the set functionality is disabled 
        /// </summary>
        public RectangleShape RenderTarget {
            get { return m_renderTarget; }
            internal set {
                if (value.X >= 0 && value.Y >= 0 && value.Width > 0 && value.Height > 0) {
                    m_renderTarget = value;
                } else {
                    TackConsole.EngineLog(TackConsole.LogType.Warning, "Cannot set Camera RenderTarget to value: {0}", value.ToString());
                }
            }
        }

        public float ZoomFactor { 
            get { return m_zoomFactor; } 
            set { m_zoomFactor = Math.TackMath.Clamp(value, 0, float.PositiveInfinity); } }

        public Camera() {
            m_renderTarget = new RectangleShape(0, 0, TackEngineInstance.Instance.Window.WindowSize.X, TackEngineInstance.Instance.Window.WindowSize.Y);
            m_zoomFactor = 1f;
        }

        public override void OnAttachedToTackObject() {
            base.OnAttachedToTackObject();

            if (MainCamera == null) {
                MainCamera = this;
            }
        }

        internal static RectangleShape GetRenderTargetForSplitScreenMode(int cameraIndex, SplitScreenMode mode) {
            switch (mode) {
                case SplitScreenMode.Single:
                    return new RectangleShape(0, 0, TackEngineInstance.Instance.Window.WindowSize.X, TackEngineInstance.Instance.Window.WindowSize.Y);
                case SplitScreenMode.DualScreen:
                    if (cameraIndex == 0) {
                        return new RectangleShape(0, 0, TackEngineInstance.Instance.Window.WindowSize.X / 2f, TackEngineInstance.Instance.Window.WindowSize.Y);
                    } else {
                        return new RectangleShape(TackEngineInstance.Instance.Window.WindowSize.X / 2f, 0, TackEngineInstance.Instance.Window.WindowSize.X / 2f, TackEngineInstance.Instance.Window.WindowSize.Y);
                    }
                case SplitScreenMode.QuadScreen:
                    float halfWidth = TackEngineInstance.Instance.Window.WindowSize.X / 2f;
                    float halfHeight = TackEngineInstance.Instance.Window.WindowSize.Y / 2f;

                    if (cameraIndex == 0) {
                        return new RectangleShape(0, 0, halfWidth, halfHeight);
                    } else if (cameraIndex == 1) {
                        return new RectangleShape(halfWidth, 0, halfWidth, halfHeight);
                    } else if (cameraIndex == 2) {
                        return new RectangleShape(0, halfHeight, halfWidth, halfHeight);
                    } else {
                        return new RectangleShape(halfWidth, halfHeight, halfWidth, halfHeight);
                    }
                default:
                    return new RectangleShape(0, 0, TackEngineInstance.Instance.Window.WindowSize.X, TackEngineInstance.Instance.Window.WindowSize.Y);
            }
        }
    }
}
