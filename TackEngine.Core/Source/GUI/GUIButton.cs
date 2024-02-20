using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Input;
using TackEngine.Core.GUI.Events;
using tainicom.Aether.Physics2D.Collision.Shapes;
using TackEngine.Core.Engine;

namespace TackEngine.Core.GUI {
    public class GUIButton : GUIObject {

        /// <summary>
        /// The style of a GUIButton object
        /// </summary>
        public class GUIButtonStyle {

            public Colour4b Colour { get; set; }
            public GUIBorder Border { get; set; }
            public TackFont Font { get; set; }
            public float FontSize { get; set; }
            public Colour4b FontColour { get; set; }
            public HorizontalAlignment HorizontalAlignment { get; set; }
            public VerticalAlignment VerticalAlignment { get; set; }
            public Sprite Texture { get; set; }
            public float ScrollPosition { get; set; }
            public bool Scrollable { get; set; }

            public GUIButtonStyle() {
                Colour = Colour4b.White;
                Border = new GUIBorder(0, 0, 0, 0, Colour4b.Black);
                FontSize = 8f;
                Font = BaseTackGUI.Instance.DefaultFont;
                FontColour = Colour4b.Black;
                HorizontalAlignment = HorizontalAlignment.Middle;
                VerticalAlignment = VerticalAlignment.Middle;
                Texture = Sprite.DefaultUISprite;
                ScrollPosition = 0.0f;
                Scrollable = false;
            }

            internal GUITextArea.GUITextAreaStyle ToGUITextAreaStyle() {
                GUITextArea.GUITextAreaStyle style = new GUITextArea.GUITextAreaStyle();
                style.Border = Border;
                style.Colour = Colour;
                style.FontColour = FontColour;
                style.Font = Font;
                style.FontSize = FontSize;
                style.HorizontalAlignment = HorizontalAlignment;
                style.VerticalAlignment = VerticalAlignment;
                style.Texture = Texture;
                style.Scrollable = Scrollable;
                style.ScrollPosition = ScrollPosition;

                return style;
            }

            internal GUIBox.GUIBoxStyle ToGUIBoxStyle() {
                GUIBox.GUIBoxStyle style = new GUIBox.GUIBoxStyle();
                style.Border = Border;
                style.Colour = Colour;
                style.Texture = Texture;

                return style;
            }
        }

        private bool m_pressing;

        /// <summary>
        /// The regular style of this GUIButton
        /// </summary>
        public GUIButtonStyle NormalStyle { get; set; }

        /// <summary>
        /// The style of this GUIButton when being hovered over
        /// </summary>
        public GUIButtonStyle HoverStyle { get; set; }

        /// <summary>
        /// The style of this GUIButton when it is being pressed
        /// </summary>
        public GUIButtonStyle ClickedStyle { get; set; }

        /// <summary>
        /// The text of this GUIButton
        /// </summary>
        public string Text { get; set; }

        public event EventHandler OnClickedEvent;

        /// <summary>
        /// Intialises a new Button
        /// </summary>
        public GUIButton() {
            Text = "GUIButton";

            NormalStyle = new GUIButtonStyle();

            HoverStyle = new GUIButtonStyle();
            HoverStyle.Colour = new Colour4b(225, 225, 225, 255);

            ClickedStyle = new GUIButtonStyle();
            ClickedStyle.Colour = new Colour4b(195, 195, 195, 255);

            Position = new Vector2f(5, 5);
            Size = new Vector2f(175, 40);

            BaseTackGUI.Instance.RegisterGUIObject(this);
        }

        internal override void OnStart() {
            base.OnStart();
        }

        internal override void OnUpdate() {
            base.OnUpdate();
        }

        internal override void OnRender(GUIMaskData maskData) {
            base.OnRender(new GUIMaskData(maskData.Masks));

            GUIButtonStyle style = NormalStyle;

            if (IsMouseHovering) {
                style = HoverStyle;
            }

            if (m_pressing) {
                style = ClickedStyle;
            }

            // Background
            BaseTackGUI.Instance.InternalBox(new RectangleShape(Position, Size), style.ToGUIBoxStyle(), new GUIMaskData(maskData.Masks));

            // Render text
            BaseTackGUI.Instance.InternalTextArea(new RectangleShape(Position, Size), Text, style.ToGUITextAreaStyle(), Vector2f.Zero, new GUIMaskData(maskData.Masks), -1);

            for (int i = 0; i < ChildObjects.Count; i++) {
                if (ChildObjects[i].Active) {
                    ChildObjects[i].OnRender(new GUIMaskData(maskData.Masks));
                }
            }
        }

        internal override void OnClose() {

        }

        internal override void OnMouseEvent(GUIMouseEventArgs args) {
            base.OnMouseEvent(args);

            if (args.MouseButton == MouseButtonKey.Left && args.MouseAction == MouseButtonAction.Down) {
                m_pressing = true;
            }

            if (args.MouseButton == MouseButtonKey.Left && args.MouseAction == MouseButtonAction.Up) {
                m_pressing = false;

                RectangleShape shape = GetShapeWithMask();

                Vector2f mousePos = TackInput.Instance.MousePosition.ToVector2f();

                if (TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.Android) {
                    mousePos = TackInput.Instance.TouchPosition.ToVector2f();
                }

                if (Physics.AABB.IsPointInAABB(new Physics.AABB(new Vector2f(shape.X, shape.Y + shape.Height), new Vector2f(shape.X + shape.Width, shape.Y)), mousePos)) {
                    if (OnClickedEvent != null) {
                        if (OnClickedEvent.GetInvocationList().Length > 0) {
                            OnClickedEvent.Invoke(this, EventArgs.Empty);
                        }
                    }
                }
            }
        }

        internal override void OnFocusGained() {
            base.OnFocusGained();
        }

        internal override void OnFocusLost() {
            base.OnFocusLost();
        }
    }
}
