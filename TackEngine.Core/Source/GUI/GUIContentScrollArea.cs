using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Engine;
using TackEngine.Core.Input;

namespace TackEngine.Core.GUI {
    public class GUIContentScrollArea : GUIObject, IGUIScrollable {

        public float HorizontalScrollPosition { get; set; }
        public float VerticalScrollPosition { get; set; }
        public float ScrollSensitivity { get; set; }
        public bool CanScroll { get; set; }

        private GUIScrollBar m_verticalScrollBar;

        private Dictionary<int, Vector2f> m_childOffsets;

        public GUIContentScrollArea() {
            Position = new Vector2f(5, 5);
            Size = new Vector2f(300, 300);

            ScrollSensitivity = 1;
            m_verticalScrollBar = new GUIScrollBar(this);

            m_childOffsets = new Dictionary<int, Vector2f>();

            BaseTackGUI.Instance.RegisterGUIObject(this);
        }

        public override void OnStart() {

        }

        public override void OnUpdate() {
            base.OnUpdate();

            m_verticalScrollBar.Size = new Vector2f(m_verticalScrollBar.Size.X, Size.Y);
            m_verticalScrollBar.Position = new Vector2f(Position.X + Size.X - m_verticalScrollBar.Size.X, Position.Y);
            m_verticalScrollBar.SetData(Size.Y, GetContentSize().Y, VerticalScrollPosition);

            if (TackInput.Instance.MouseScrollWheelChange != 0) {
                if (IsMouseHovering) {
                    Vector2f contentSize = GetContentSize();

                    if (contentSize.Y > Size.Y) {
                        float diff = contentSize.Y - Size.Y;
                        VerticalScrollPosition = Math.TackMath.Clamp(VerticalScrollPosition + (-TackInput.Instance.MouseScrollWheelChange * (ScrollSensitivity * 10)), 0, diff);
                    }
                }
            }

            for (int i = 0; i < ChildObjects.Count; i++) {
                if (!m_childOffsets.ContainsKey(ChildObjects[i].Id)) {
                    m_childOffsets.Add(ChildObjects[i].Id, ChildObjects[i].LocalPosition);
                }

                ChildObjects[i].LocalPosition = new Vector2f(ChildObjects[i].LocalPosition.X, m_childOffsets[ChildObjects[i].Id].Y - VerticalScrollPosition);
            }
        }

        internal override void OnRender(GUIMaskData maskData) {
            base.OnRender(new GUIMaskData(maskData.Masks));

            BaseTackGUI.Instance.InternalBox(new RectangleShape(Position, Size), new GUIBox.GUIBoxStyle() { Colour = Colour4b.Blue }, new GUIMaskData(maskData.Masks));

            m_verticalScrollBar.OnRender(new GUIMaskData(maskData.Masks));

            maskData.AddMask(new RectangleShape(Position, Size));

            for (int i = 0; i < ChildObjects.Count; i++) {
                if (ChildObjects[i].Active) {
                    ChildObjects[i].OnRender(new GUIMaskData(maskData.Masks));
                }
            }
        }

        public override void OnClose() {
            
        }

        private Vector2f GetContentSize() {
            Vector2f largest = new Vector2f();

            foreach (KeyValuePair<int, Vector2f> pair in m_childOffsets) {
                GUIObject childObj = ChildObjects.Find(o => o.Id == pair.Key);

                if ((pair.Value.X + childObj.Size.X) > largest.X) {
                    largest.X = pair.Value.X + childObj.Size.X;
                }

                if ((pair.Value.Y + childObj.Size.Y) > largest.Y) {
                    largest.Y = pair.Value.Y + childObj.Size.Y;
                }
            }

            return largest;
        }
    }
}
