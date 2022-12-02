using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Input;
using TackEngine.Core.GUI.Events;

namespace TackEngine.Core.GUI {
    public class GUIToggle : GUIObject {

        /// <summary>
        /// The style of a GUIToggle object
        /// </summary>
        public class GUIToggleStyle {

            public Colour4b Colour { get; set; }
            public GUIBorder Border { get; set; }
            public TackFont Font { get; set; }
            public float FontSize { get; set; }
            public Colour4b FontColour { get; set; }

            public GUIToggleStyle() {
                Colour = Colour4b.Black;
                Border = new GUIBorder(0, 0, 0, 0, Colour4b.Black);
                FontSize = 8f;
                Font = BaseTackGUI.Instance.DefaultFont;
                FontColour = Colour4b.Black;
            }

            internal GUIBox.GUIBoxStyle ConvertToGUIBoxStyle() {
                GUIBox.GUIBoxStyle style = new GUIBox.GUIBoxStyle();
                style.Border = Border;
                style.Colour = Colour;

                return style;
            }

            internal GUITextArea.GUITextAreaStyle ConvertToGUITextStyle() {
                GUITextArea.GUITextAreaStyle style = new GUITextArea.GUITextAreaStyle();
                style.Border = Border;
                style.Colour = new Colour4b(Colour.R, Colour.G, Colour.B, 0);
                style.FontColour = FontColour;
                style.Font = Font;
                style.FontSize = FontSize;
                style.HorizontalAlignment = HorizontalAlignment.Left;
                style.VerticalAlignment = VerticalAlignment.Middle;
                style.Scrollable = false;
                style.ScrollPosition = 0.0f;

                return style;
            }
        }

        private RectangleShape m_toggleBounds;

        public string Text { get; set; }
        public bool IsSelected { get; set; }
        public GUIToggleStyle NormalStyle { get; set; }

        /// <summary>
        /// The event that is invoked when the object is selected/unselected
        /// </summary>
        public event EventHandler OnToggledEvent;

        public GUIToggle() {
            Text = "GUIToggle";
            IsSelected = false;

            NormalStyle = new GUIToggleStyle();

            BaseTackGUI.Instance.RegisterGUIObject(this);
        }

        internal override void OnStart() {
            
        }

        internal override void OnUpdate() {
            base.OnUpdate();

            m_toggleBounds = new RectangleShape(Position.X + 1, Position.Y + 1, Size.Y - 2, Size.Y - 2);
        }

        internal override void OnRender(GUIMaskData maskData) {
            base.OnRender(new GUIMaskData(maskData.Masks));

            GUIToggleStyle style = NormalStyle;

            // The selection box
            RectangleShape optionSelectedBox = new RectangleShape(m_toggleBounds.X + 4, m_toggleBounds.Y + 4, m_toggleBounds.Width -8, m_toggleBounds.Height - 8);

            BaseTackGUI.Instance.InternalBox(m_toggleBounds, new GUIBox.GUIBoxStyle() { Colour = Colour4b.White, Border = null, Texture = Sprite.DefaultSprite }, maskData);

            if (IsSelected) {
                BaseTackGUI.Instance.InternalBox(optionSelectedBox, new GUIBox.GUIBoxStyle() { Colour = style.Colour, Border = null, Texture = Sprite.DefaultSprite }, maskData);
            }

            BaseTackGUI.Instance.InternalTextArea(new RectangleShape(Position.X + m_toggleBounds.Width, Position.Y, Size.X, Size.Y), Text, style.ConvertToGUITextStyle(), Vector2f.Zero, maskData, -1);
        }

        internal override void OnClose() {
            
        }

        internal override void OnMouseEvent(GUIMouseEventArgs args) {
            base.OnMouseEvent(args);

            if (args.MouseButton == MouseButtonKey.Left && args.MouseAction == MouseButtonAction.Up) {
                if (TackEngine.Core.Physics.AABB.IsPointInAABB(new Physics.AABB(m_toggleBounds), TackEngine.Core.Input.TackInput.Instance.MousePosition.ToVector2f())) {
                    if (IsMouseHovering) {
                        IsSelected = !IsSelected;

                        InvokeOnToggledEvent();
                    }
                }
            }
        }

        internal override void OnKeyboardEvent(GUIKeyboardEventArgs args) {
            base.OnKeyboardEvent(args);

            if ((args.Key == KeyboardKey.Space || args.Key == KeyboardKey.Enter) && args.KeyAction == KeyboardKeyAction.Up){
                if (IsFocused) {
                    IsSelected = !IsSelected;

                    InvokeOnToggledEvent();
                }
            }
        }

        private void InvokeOnToggledEvent() {
            if (OnToggledEvent != null) {
                if (OnToggledEvent.GetInvocationList().Length > 0) {
                    OnToggledEvent.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
