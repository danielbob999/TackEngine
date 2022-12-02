using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using TackEngine.Core.Main;
using TackEngine.Core.Input;

namespace TackEngine.Core.GUI {
    public class GUITextArea : GUIObject, IGUIScrollable {

        /// <summary>
        /// The style of a GUITextArea object
        /// </summary>
        public class GUITextAreaStyle {

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

            public GUITextAreaStyle() {
                Colour = Colour4b.White;
                Border = new GUIBorder(0, 0, 0, 0, Colour4b.Black);
                FontSize = 8f;
                FontColour = Colour4b.Black;
                HorizontalAlignment = HorizontalAlignment.Left;
                VerticalAlignment = VerticalAlignment.Top;
                Texture = Sprite.DefaultUISprite;
                ScrollPosition = 0.0f;
                Scrollable = false;
                Font = BaseTackGUI.Instance.DefaultFont;
            }

            public GUIBox.GUIBoxStyle ToBoxStyle() {
                return new GUIBox.GUIBoxStyle() {
                    Border = this.Border,
                    Colour = this.Colour,
                    Texture = this.Texture
                };
            }
        }

        private bool m_hovering = false;

        public string Text { get; set; }
        public GUITextAreaStyle NormalStyle { get; set; }
        public float HorizontalScrollPosition { get; set; }
        public float VerticalScrollPosition { get; set; }
        public float ScrollSensitivity { get; set; }
        public bool CanScroll { get; set; }

        public GUITextArea() {
            Text = "GUITextArea";
            NormalStyle = new GUITextAreaStyle();
            VerticalScrollPosition = 0;
            ScrollSensitivity = 1;
            CanScroll = false;

            BaseTackGUI.Instance.RegisterGUIObject(this);
        }

        internal override void OnStart() {
            base.OnStart();
        }

        internal override void OnUpdate() {
            base.OnUpdate();

            if (CanScroll) {
                if (TackInput.Instance.MouseScrollWheelChange != 0) {
                    if (IsMouseHovering) {
                        Vector2f sizeOfString = BaseTackGUI.MeasureStringSize(Text, BaseTackGUI.Instance.DefaultFont, NormalStyle.FontSize, new RectangleShape(Position, Size));

                        if (sizeOfString.Y > Size.Y) {
                            float diff = sizeOfString.Y - Size.Y;
                            VerticalScrollPosition = Math.TackMath.Clamp(VerticalScrollPosition + TackInput.Instance.MouseScrollWheelChange * (ScrollSensitivity * 10), -diff - 10, 0);
                        }   
                    }
                }
            }
        }

        internal override void OnRender(GUIMaskData maskData) {
            base.OnRender(new GUIMaskData(maskData.Masks));

            BaseTackGUI.Instance.InternalBox(new RectangleShape(Position, Size), NormalStyle.ToBoxStyle(), new GUIMaskData(maskData.Masks));

            BaseTackGUI.Instance.InternalTextArea(new RectangleShape(Position, Size), Text, NormalStyle, new Vector2f(0, VerticalScrollPosition), new GUIMaskData(maskData.Masks), -1);

            for (int i = 0; i < ChildObjects.Count; i++) {
                if (ChildObjects[i].Active) {
                    ChildObjects[i].OnRender(new GUIMaskData(maskData.Masks));
                }
            }
        }

        internal override void OnClose() {
            base.OnClose();
        }
    }
}
