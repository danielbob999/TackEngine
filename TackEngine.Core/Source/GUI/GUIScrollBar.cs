using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.GUI;
using TackEngine.Core.GUI.Events;
using TackEngine.Core.Input;
using TackEngine.Core.Math;
using TackEngine.Core.Engine;

namespace TackEngine.Core.GUI {
    internal class GUIScrollBar : GUIObject {
        public enum GUIScrollBarOrientation {
            Horizontal,
            Vertical
        }

        public enum GUIScrollBarDisplayMode {
            Always, // The scroll bar is always displayed
            Auto,   // The scroll bar is only shown if the content is larger than the display area
            Never   // The scroll bar is never displayed
        }

        /// <summary>
        /// The style of a GUIScrollBar
        /// </summary>
        public class GUIScrollBarStyle {
            public Colour4b Colour { get; set; }
            public Colour4b HandleColour { get; set; }

            public GUIScrollBarStyle() {
                HandleColour = new Colour4b(205, 205, 205, 255);
                Colour = new Colour4b(240, 240, 240, 255);
            }
        }

        private RectangleShape m_handleBounds;
        private Vector2i m_mouseDownOffset;
        private bool m_shouldRender = false;
        private float m_parentContentSize;
        private float m_parentBaseSize;
        private IGUIScrollable m_parentScrollable;

        public GUIScrollBarStyle NormalStyle { get; set; }
        public GUIScrollBarOrientation Orientation { get; set; }
        public GUIScrollBarDisplayMode DisplayMode { get; set; }
        public bool IsDragging { get; private set; }

        public GUIScrollBar(IGUIScrollable parentScrollable) {
            m_parentScrollable = parentScrollable;

            NormalStyle = new GUIScrollBarStyle();
            Orientation = GUIScrollBarOrientation.Vertical;
            LocalPosition = new Vector2f(5, 5);
            DisplayMode = GUIScrollBarDisplayMode.Auto;

            if (Orientation == GUIScrollBarOrientation.Vertical) {
                Size = new Vector2f(10, 200);
            }

            m_handleBounds = new RectangleShape(0, 0, 10, 30);

            BaseTackGUI.Instance.RegisterGUIObject(this);
        }

        internal override void OnUpdate() {
            base.OnUpdate();

            if (IsDragging) {
                Vector2f mousePos = TackInput.Instance.MousePosition.ToVector2f();

                if (TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.Android) {
                    mousePos = TackInput.Instance.TouchPosition.ToVector2f();
                }

                float delta = m_mouseDownOffset.Y - mousePos.Y;
                float diff = m_parentScrollable.GetContentSize().Y - Size.Y;

                m_parentScrollable.VerticalScrollPosition = Math.TackMath.Clamp(m_parentScrollable.VerticalScrollPosition - (delta * (m_parentScrollable.ScrollSensitivity * 1)), 0, diff);
                m_mouseDownOffset = new Vector2i((int)mousePos.X, (int)mousePos.Y);
            }
        }

        internal override void OnRender(GUIMaskData maskData) {
            if (m_shouldRender) {
                // Scrollbar background
                BaseTackGUI.Instance.InternalBox(new RectangleShape(Position, Size), new GUIBox.GUIBoxStyle() { Colour = NormalStyle.Colour, Texture = Sprite.DefaultSprite }, maskData);

                // Scrollbar handle
                BaseTackGUI.Instance.InternalBox(new RectangleShape(Position.X, m_handleBounds.Y + Position.Y, m_handleBounds.Width, m_handleBounds.Height), new GUIBox.GUIBoxStyle() { Colour = NormalStyle.HandleColour, Texture = Sprite.DefaultSprite }, maskData);
            }
        }

        internal override void OnMouseEvent(GUIMouseEventArgs args) {
            base.OnMouseEvent(args);

            if (args.MouseAction == MouseButtonAction.Down) {
                Vector2f mousePos = TackInput.Instance.MousePosition.ToVector2f();

                if (TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.Android ||
                    TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.iOS) {
                    mousePos = TackInput.Instance.TouchPosition.ToVector2f();
                }

                RectangleShape rect = new RectangleShape(Position.X + m_handleBounds.X, Position.Y + m_handleBounds.Y, m_handleBounds.Width, m_handleBounds.Height);

                if (Physics.AABB.IsPointInAABB(new Physics.AABB(rect), mousePos)) {
                    IsDragging = true;
                    m_mouseDownOffset = args.MousePosition;
                }
            }

            if (args.MouseAction == MouseButtonAction.Up) {
                IsDragging = false;
            }
        }

        internal void SetData(float baseAreaSize, float contentSize, float scrollPos) {
            m_parentBaseSize = baseAreaSize;
            m_parentContentSize = contentSize;

            if (baseAreaSize < contentSize) {
                float mult = contentSize / baseAreaSize;
                float scrollPosClamped = scrollPos / (contentSize - baseAreaSize);

                if (Orientation == GUIScrollBarOrientation.Vertical) {
                    m_handleBounds.Height = baseAreaSize / mult;
                    m_handleBounds.Y = (baseAreaSize - m_handleBounds.Height) * scrollPosClamped;
                }

                m_shouldRender = (DisplayMode == GUIScrollBarDisplayMode.Never ? false : true);
            } else {
                m_handleBounds.Height = baseAreaSize;

                m_shouldRender = (DisplayMode == GUIScrollBarDisplayMode.Always ? true : false);
            }
        }
    }
}
