using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Engine;

namespace TackEngine.Core.GUI {
    public class GUIBox : GUIObject {

        /// <summary>
        /// The style of a GUIBox object
        /// </summary>
        public class GUIBoxStyle {
            public Colour4b Colour { get; set; }
            public Sprite Texture { get; set; }
            public GUIBorder Border { get; set; }

            public GUIBoxStyle() {
                Colour = Colour4b.White;
                Texture = Sprite.DefaultUISprite;
                Border = new GUIBorder(0, 0, 0, 0, Colour4b.Black);
            }
        }

        public GUIBoxStyle NormalStyle { get; set; }
        public GUIBoxStyle HoverStyle { get; set; }

        public GUIBox() {
            Position = new Vector2f(5, 5);
            Size = new Vector2f(300, 300);

            NormalStyle = new GUIBoxStyle();
            HoverStyle = new GUIBoxStyle();

            BaseTackGUI.Instance.RegisterGUIObject(this);
        }

        public override void OnStart() {

        }

        public override void OnUpdate() {
            base.OnUpdate();
        }

        internal override void OnRender(GUIMaskData maskData) {
            base.OnRender(new GUIMaskData(maskData.Masks));

            GUIBoxStyle style = NormalStyle;

            if (IsMouseHovering) {
                style = HoverStyle;
            }

            BaseTackGUI.Instance.InternalBox(new RectangleShape(Position, Size), style, new GUIMaskData(maskData.Masks));

            for (int i = 0; i < ChildObjects.Count; i++) {
                if (ChildObjects[i].Active) {
                    ChildObjects[i].OnRender(new GUIMaskData(maskData.Masks));
                }
            }
        }

        public override void OnClose() {
            
        }
    }
}
