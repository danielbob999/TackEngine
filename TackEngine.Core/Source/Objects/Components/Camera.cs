/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TackEngineLib.Engine;
using TackEngineLib.Main;

namespace TackEngineLib.Objects.Components
{
    /// <summary>
    /// The component that draws the world view to the screen
    /// </summary>
    public class Camera : TackComponent
    {
        private static Camera s_mainCamera = null;
        public static Camera MainCamera {
            get {
                if (s_mainCamera == null) {
                    // Create an object
                    TackObject camObject = TackObject.Create("Camera", new Vector2f(0, 0), new Vector2f(0, 0), 0);

                    Camera newCamera = new Camera();

                    camObject.AddComponent(newCamera);
                    s_mainCamera = newCamera;
                }

                return s_mainCamera;
            }

            set {
                if (value != null) {
                    s_mainCamera = value;
                }
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
                }
            }
        }

        public float ZoomFactor { 
            get { return m_zoomFactor; } 
            set { m_zoomFactor = Math.TackMath.Clamp(value, 0, float.PositiveInfinity); } }

        public Camera() {
            RenderTarget = new RectangleShape(0, 0, TackEngine.Instance.Window.WindowSize.X, TackEngine.Instance.Window.WindowSize.Y);
            m_zoomFactor = 1f;
        }
    }
}
