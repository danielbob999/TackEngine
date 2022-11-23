using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.GUI;
using TackEngineLib.GUI.Events;
using TackEngineLib.Input;
using TackEngineLib.Math;

namespace TackEngineLib.GUI {
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
        private bool m_isDragging = false;
        private Vector2i m_mouseDownOffset;
        private bool m_shouldRender = false;
        private float m_parentContentSize;
        private float m_parentBaseSize;
        private IGUIScrollable m_parentScrollable;

        public GUIScrollBarStyle NormalStyle { get; set; }
        public GUIScrollBarOrientation Orientation { get; set; }
        public GUIScrollBarDisplayMode DisplayMode { get; set; }

        public GUIScrollBar(IGUIScrollable parentScrollable) {
            m_parentScrollable = parentScrollable;

            NormalStyle = new GUIScrollBarStyle();
            Orientation = GUIScrollBarOrientation.Vertical;
            LocalPosition = new Vector2f(5, 5);
            DisplayMode = GUIScrollBarDisplayMode.Auto;

            if (Orientation == GUIScrollBarOrientation.Vertical) {
                Size = new Vector2f(30, 200);
            }

            m_handleBounds = new RectangleShape(0, 0, 30, 30);

            BaseTackGUI.Instance.RegisterGUIObject(this);
        }

        internal override void OnUpdate() {
            base.OnUpdate();

            if (IsWaitingForMouseUp(MouseButtonKey.Left)) {
                if (TackInput.MouseButtonUp(MouseButtonKey.Left)) {
                    m_isDragging = false;
                }
            }
        }

        internal override void OnRender(GUIMaskData maskData) {
            if (m_shouldRender) {
                BaseTackGUI.Instance.InternalBox(new RectangleShape(Position, Size), new GUIBox.GUIBoxStyle() { Colour = Colour4b.Red }, maskData);
                BaseTackGUI.Instance.InternalBox(new RectangleShape(Position.X, m_handleBounds.Y + Position.Y, m_handleBounds.Width, m_handleBounds.Height), new GUIBox.GUIBoxStyle() { Colour = Colour4b.Green }, maskData);
            }
        }

        internal override void OnMouseEvent(GUIMouseEventArgs args) {
            base.OnMouseEvent(args);

            if (Physics.AABB.IsPointInAABB(new Physics.AABB(m_handleBounds), TackInput.Instance.MousePosition.ToVector2f())) {
                m_isDragging = true;
                m_mouseDownOffset = args.MousePosition - new Vector2i((int)m_handleBounds.X, (int)m_handleBounds.Y);
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
