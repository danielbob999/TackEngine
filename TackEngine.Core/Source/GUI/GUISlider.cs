using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Input;
using TackEngine.Core.GUI.Events;
using TackEngine.Core.Engine;

namespace TackEngine.Core.GUI {
    public class GUISlider : GUIObject {

        /// <summary>
        /// The style of a GUISlider object
        /// </summary>
        public class GUISliderStyle {

            public Colour4b BackgroundColour { get; set; }
            public Sprite BackgroundSprite { get; set; }
            public Colour4b HandleColour { get; set; }
            public Sprite HandleSprite { get; set; }
            public Colour4b FillColour { get; set; }
            public Sprite FillSprite { get; set; }

            // Static default styles

            public GUISliderStyle() {
                BackgroundColour = new Colour4b(150, 150, 150, 255);
                HandleColour = Colour4b.White;
                FillColour = Colour4b.Green;
                BackgroundSprite = Sprite.DefaultSprite;
                FillSprite = Sprite.DefaultSprite;
                HandleSprite = Sprite.DefaultSprite;
            }

            internal GUIBox.GUIBoxStyle ConvertHandleToGUIBoxStyle() {
                GUIBox.GUIBoxStyle style = new GUIBox.GUIBoxStyle();
                style.Border = null;
                style.Texture = HandleSprite;
                style.Colour = HandleColour;

                return style;
            }

            internal GUIBox.GUIBoxStyle ConvertBackgroundToGUIBoxStyle() {
                GUIBox.GUIBoxStyle style = new GUIBox.GUIBoxStyle();
                style.Border = null;
                style.Texture = BackgroundSprite;
                style.Colour = BackgroundColour;
                return style;
            }

            internal GUIBox.GUIBoxStyle ConvertFillToGUIBoxStyle() {
                GUIBox.GUIBoxStyle style = new GUIBox.GUIBoxStyle();
                style.Border = null;
                style.Texture = FillSprite;
                style.Colour = FillColour;

                return style;
            }

        }

        private float m_value;
        private RectangleShape m_backgroundBounds;
        private RectangleShape m_handleBounds;
        private RectangleShape m_fillBounds;
        private bool m_isDragging = false;
        private Vector2i m_mouseDownOffset;

        public float Value { 
            get { return m_value; }
            set { 
                if (value < MinValue) {
                    m_value = MinValue;
                    CallOnValueChangedEvent();
                    return;
                }
                
                if (value > MaxValue) {
                    m_value = MaxValue;
                    CallOnValueChangedEvent();
                    return;
                }

                m_value = value;
                CallOnValueChangedEvent();
            }
        }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public float DefaultIncrease { get; set; }
        public float DefaultDecrease { get; set; }
        public GUISliderStyle NormalStyle { get; set; }

        public float BackgroundFillHeight { get; set; }
        public Vector2f HandleSize { get; set; }

        /// <summary>
        /// The event that is invoked when the value is changed
        /// </summary>
        public event EventHandler OnValueChangedEvent;

        public GUISlider() {
            // Instantiate the bounds shapes
            m_handleBounds = new RectangleShape();
            m_backgroundBounds = new RectangleShape();
            m_fillBounds = new RectangleShape();

            NormalStyle = new GUISliderStyle() {
                BackgroundColour = new Colour4b(200, 200, 200, 255),
                HandleColour = Colour4b.White,
                FillColour = new Colour4b(100, 100, 100, 255),
                BackgroundSprite = Sprite.DefaultSprite,
                FillSprite = Sprite.DefaultSprite,
                HandleSprite = Sprite.DefaultSprite
            };

            MinValue = 0;
            MaxValue = 1;
            Value = 0.5f;

            HandleSize = new Vector2f(13, 35);
            BackgroundFillHeight = 17f;

            Position = new Vector2f(10, 20);
            Size = new Vector2f(250, 40);

            BaseTackGUI.Instance.RegisterGUIObject(this);
        }

        internal override void OnStart() {

        }

        internal override void OnUpdate() {
            base.OnUpdate();

            if (IsWaitingForMouseUp(MouseButtonKey.Left)) {
                if (TackInput.MouseButtonUp(MouseButtonKey.Left)) {
                    m_isDragging = false;
                }
            }

            // Constantly set handle size
            m_handleBounds.Width = HandleSize.X;
            m_handleBounds.Height = HandleSize.Y;

            // Constantly set background bounds
            m_backgroundBounds = new RectangleShape(Position.X, (Position.Y + (Size.Y / 2f)) - (BackgroundFillHeight / 2f), Size.X, BackgroundFillHeight);

            m_fillBounds = new RectangleShape() {
                X = m_backgroundBounds.X,
                Y = m_backgroundBounds.Y,
                Width = ((m_value / (MaxValue - MinValue)) * m_backgroundBounds.Width) - ((MinValue / (MaxValue - MinValue)) * m_backgroundBounds.Width),
                Height = m_backgroundBounds.Height
            };

            m_handleBounds.Y = m_backgroundBounds.Y - (m_handleBounds.Height / 2f) + (m_backgroundBounds.Height / 2f);

            if (m_isDragging) {
                Vector2f mousePos = TackInput.Instance.MousePosition.ToVector2f();

                if (TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.Android) {
                    mousePos = TackInput.Instance.TouchPosition.ToVector2f();
                }

                m_handleBounds.X = Math.TackMath.Clamp(m_mouseDownOffset.X + mousePos.X, m_backgroundBounds.X, (m_backgroundBounds.X + m_backgroundBounds.Width) - m_handleBounds.Width);

                float valueIncreasePerPixel = (MaxValue - MinValue) / (m_backgroundBounds.Width - m_handleBounds.Width);

                Value = (float)System.Math.Round(MinValue + ((m_handleBounds.X - m_backgroundBounds.X) * valueIncreasePerPixel), 6);
            } else {
                float valueNormalised = (m_value - MinValue) / (MaxValue - MinValue);
                m_handleBounds.X = m_backgroundBounds.X + (valueNormalised * (m_backgroundBounds.Width - m_handleBounds.Width));
            }
        }

        internal override void OnRender(GUIMaskData maskData) {
            base.OnRender(new GUIMaskData(maskData.Masks));

            GUISliderStyle style = NormalStyle;

            // The background
            BaseTackGUI.Instance.InternalBox(m_backgroundBounds, style.ConvertBackgroundToGUIBoxStyle(), new GUIMaskData(maskData.Masks));

            // The fill
            BaseTackGUI.Instance.InternalBox(m_fillBounds, style.ConvertFillToGUIBoxStyle(), new GUIMaskData(maskData.Masks));

            // The handle
            BaseTackGUI.Instance.InternalBox(m_handleBounds, style.ConvertHandleToGUIBoxStyle(), new GUIMaskData(maskData.Masks));

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
                Vector2f mousePos = TackInput.Instance.MousePosition.ToVector2f();

                if (TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.Android) {
                    mousePos = TackInput.Instance.TouchPosition.ToVector2f();
                }

                if (TackEngine.Core.Physics.AABB.IsPointInAABB(new Physics.AABB(m_handleBounds), mousePos)) {
                    m_isDragging = true;
                    m_mouseDownOffset = args.MousePosition - new Vector2i((int)m_handleBounds.X, (int)m_handleBounds.Y);
                }
            }

            if (args.MouseButton == MouseButtonKey.Left && args.MouseAction == MouseButtonAction.Up) {
                if (m_isDragging) {
                    m_isDragging = false;
                } else {
                    // Check the left side of the handle of click
                    Vector2f mousePos = TackInput.Instance.MousePosition.ToVector2f();

                    if (TackEngineInstance.Instance.Platform == TackEngineInstance.TackEnginePlatform.Android) {
                        mousePos = TackInput.Instance.TouchPosition.ToVector2f();
                    }

                    if (Physics.AABB.IsPointInAABB(new Physics.AABB(new RectangleShape(m_backgroundBounds.X, m_backgroundBounds.Y, (m_handleBounds.X - m_backgroundBounds.X), m_backgroundBounds.Height)), mousePos)){
                        Value -= DefaultDecrease;
                    } else if (Physics.AABB.IsPointInAABB(new Physics.AABB(new RectangleShape((m_handleBounds.X + m_handleBounds.Width), m_backgroundBounds.Y, (m_backgroundBounds.X + m_backgroundBounds.Width) - (m_handleBounds.X + m_handleBounds.Width), m_backgroundBounds.Height)), mousePos)) {
                        Value += DefaultIncrease;
                        return;
                    }
                }
            }
        }

        internal override void OnKeyboardEvent(GUIKeyboardEventArgs args) {
            base.OnKeyboardEvent(args);

            if (args.Key == KeyboardKey.Left && args.KeyAction == KeyboardKeyAction.Up) {
                if (IsFocused) {
                    Value -= DefaultDecrease;
                }
            }

            if (args.Key == KeyboardKey.Right && args.KeyAction == KeyboardKeyAction.Up) {
                if (IsFocused) {
                    Value += DefaultIncrease;
                }
            }
        }

        private void CallOnValueChangedEvent() {
            if (OnValueChangedEvent != null) {
                if (OnValueChangedEvent.GetInvocationList().Length > 0) {
                    OnValueChangedEvent.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
