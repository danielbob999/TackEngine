using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Input;
using TackEngine.Core.GUI.Events;

namespace TackEngine.Core.GUI {
    public class GUIInputField : GUIObject {

        /// <summary>
        /// The style of a GUIInputField object
        /// </summary>
        public class GUIInputFieldStyle {

            public Colour4b Colour { get; set; }
            public GUIBorder Border { get; set; }
            public TackFont Font { get; set; }
            public float FontSize { get; set; }
            public Colour4b FontColour { get; set; }
            public Colour4b PlaceholderTextFontColour { get; set; }
            public HorizontalAlignment HorizontalAlignment { get; set; }
            public VerticalAlignment VerticalAlignment { get; set; }
            public Sprite Texture { get; set; }
            public float ScrollPosition { get; set; }
            public bool Scrollable { get; set; }

            public GUIInputFieldStyle() {
                Colour = Colour4b.White;
                Border = Border = new GUIBorder(0, 0, 0, 0, Colour4b.Black);
                FontSize = 8f;
                HorizontalAlignment = HorizontalAlignment.Left;
                VerticalAlignment = VerticalAlignment.Top;
                Font = BaseTackGUI.Instance.DefaultFont;
                FontColour = Colour4b.Black;
                PlaceholderTextFontColour = new Colour4b(175, 175, 175, 255);
                Texture = Sprite.DefaultSprite;
            }

            internal GUITextArea.GUITextAreaStyle ConvertToGUITextStyle() {
                GUITextArea.GUITextAreaStyle style = new GUITextArea.GUITextAreaStyle();
                style.Border = Border;
                style.Colour = Colour;
                style.FontColour = FontColour;
                style.Font = Font;
                style.FontSize = FontSize;
                style.HorizontalAlignment = HorizontalAlignment;
                style.VerticalAlignment = VerticalAlignment;
                style.Scrollable = Scrollable;
                style.ScrollPosition = ScrollPosition;

                return style;
            }

            internal GUIBox.GUIBoxStyle ConvertToGUIBoxStyle() {
                return new GUIBox.GUIBoxStyle() { 
                    Border = Border,
                    Colour = Colour,
                    Texture = Texture
                };
            }
        }

        private bool m_showCaret;
        private float m_timeAtLastCaretSwitch;
        private bool m_backspacePressedDown;    // Indicates whether the backspace button has been pressed down but not released yet
        private double m_backspacePressedTime;  // The time at which the backspace button was pressed down
        private bool m_holdingBackspace;    // Indicates whether the user has been holding the backspace button for more than 0.5 seconds
        private double m_backspaceHeldLastDeleteTime;   // Indicates the time at which the last character got deleted,
                                                        //          used for deleting a certain amount of characters per second when the backspace button is being held

        public string Text { get; set; }
        public string PlaceholderText { get; set; }
        public int SelectionStart { get; set; }  // The start of the selection. If the SelectionLength = 0, SelectionStart is the position of the Caret
        public int SelectionLength { get; set; }
        public GUIInputFieldStyle NormalStyle { get; set; }
        public Input.KeyboardKey SubmitKey { get; set; }

        public event EventHandler OnSubmittedEvent;
        public event EventHandler OnTextChangedEvent;

        public GUIInputField() {
            Text = "";
            PlaceholderText = "GUIInputField";
            m_showCaret = false;
            SubmitKey = KeyboardKey.Enter;

            Position = new Vector2f(5, 5);
            Size = new Vector2f(300, 300);

            NormalStyle = new GUIInputFieldStyle();

            BaseTackGUI.Instance.RegisterGUIObject(this);
        }

        internal override void OnStart() {
            base.OnStart();

            TackInput.Instance.GUIInputRequired = true;
        }

        internal override void OnUpdate() {
            base.OnUpdate();

            if (EngineTimer.Instance.TotalRunTime - m_timeAtLastCaretSwitch > 0.53f) {
                m_timeAtLastCaretSwitch = (float)EngineTimer.Instance.TotalRunTime;
                m_showCaret = !m_showCaret;
            }

            if (m_backspacePressedDown) {
                if (EngineTimer.Instance.TotalRunTime > (m_backspacePressedTime + 0.5f)) {
                    m_holdingBackspace = true;
                }
            }

            if (m_holdingBackspace) {
                if (EngineTimer.Instance.TotalRunTime > (m_backspaceHeldLastDeleteTime + (1f / 30))) {
                    string oldText = Text;

                    m_backspaceHeldLastDeleteTime = EngineTimer.Instance.TotalRunTime;
                    BackspaceCharacter();

                    if (!Text.Equals(oldText)) {
                        if (OnTextChangedEvent != null) {
                            if (OnTextChangedEvent.GetInvocationList().Length > 0) {
                                OnTextChangedEvent.Invoke(this, EventArgs.Empty);
                            }
                        }
                    }
                }
            }
        }

        internal override void OnRender(GUIMaskData maskData) {
            base.OnRender(new GUIMaskData(maskData.Masks));

            GUIInputFieldStyle style = NormalStyle;

            // The background
            BaseTackGUI.Instance.InternalBox(new RectangleShape(Position, Size), style.ConvertToGUIBoxStyle(), new GUIMaskData(maskData.Masks));

            // The text area
            if (string.IsNullOrEmpty(Text)) {
                GUITextArea.GUITextAreaStyle placeholderTextStyle = style.ConvertToGUITextStyle();
                placeholderTextStyle.FontColour = style.PlaceholderTextFontColour;

                BaseTackGUI.Instance.InternalTextArea(new RectangleShape(Position, Size), PlaceholderText, placeholderTextStyle, Vector2f.Zero, new GUIMaskData(maskData.Masks), (IsFocused ? (m_showCaret ? SelectionStart : -1) : -1));
            } else {
                BaseTackGUI.Instance.InternalTextArea(new RectangleShape(Position, Size), Text, style.ConvertToGUITextStyle(), Vector2f.Zero, new GUIMaskData(maskData.Masks), (IsFocused ? (m_showCaret ? SelectionStart : -1) : -1));
            }

            for (int i = 0; i < ChildObjects.Count; i++) {
                if (ChildObjects[i].Active) {
                    ChildObjects[i].OnRender(new GUIMaskData(maskData.Masks));
                }
            }
        }

        internal override void OnClose() {
            base.OnClose();
        }

        internal override void OnFocusGained() {
            base.OnFocusGained();

            TackInput.Instance.GUIInputRequired = true;
        }

        internal override void OnFocusLost() {
            base.OnFocusLost();

            TackInput.Instance.GUIInputRequired = false;
        }

        internal override void OnKeyboardEvent(GUIKeyboardEventArgs args) {
            base.OnKeyboardEvent(args);

            if (args.Key == KeyboardKey.Backspace && args.KeyAction == KeyboardKeyAction.Up) {
                m_holdingBackspace = false;
                m_backspacePressedDown = false;
            }

            string oldText = Text;

            if (IsFocused) {
                if (args.KeyAction == KeyboardKeyAction.Down) {
                    if (args.Key == SubmitKey) {
                        if (OnSubmittedEvent != null) {
                            if (OnSubmittedEvent.GetInvocationList().Length > 0) {
                                OnSubmittedEvent.Invoke(this, EventArgs.Empty);
                            }
                        }
                    } else if (args.Key == KeyboardKey.Left) {
                        if (SelectionStart > 0) {
                            SelectionStart -= 1;
                        }
                    } else if (args.Key == KeyboardKey.Right) {
                        if (SelectionStart < Text.Length) {
                            SelectionStart += 1;
                        }

                    } else if (args.Key == KeyboardKey.Up) {

                    } else if (args.Key == KeyboardKey.Down) {

                    } else if (args.Key == KeyboardKey.Backspace) {
                        BackspaceCharacter();

                        m_backspacePressedDown = true;
                        m_backspacePressedTime = EngineTimer.Instance.TotalRunTime;
                    } else if (args.Key == KeyboardKey.Delete) {
                        DeleteCharacter();
                    } else if (args.Key == KeyboardKey.Space) {
                        Text = Text.Insert((int)SelectionStart, " ");

                        if (SelectionStart < Text.Length) {
                            SelectionStart += 1;
                        }
                    } else if (args.Key == KeyboardKey.Period) {
                        Text = Text.Insert((int)SelectionStart, ".");

                        if (SelectionStart < Text.Length) {
                            SelectionStart += 1;
                        }
                    } else if (args.Key == KeyboardKey.Apostrophe) {
                        Text = Text.Insert((int)SelectionStart, "\"");

                        if (SelectionStart < Text.Length) {
                            SelectionStart += 1;
                        }
                    } else if (args.Key == KeyboardKey.Minus) {
                        if (TackInput.Instance.InputBufferShift) {
                            Text = Text.Insert((int)SelectionStart, "_");
                        } else {
                            Text = Text.Insert((int)SelectionStart, "-");
                        }

                        if (SelectionStart < Text.Length) {
                            SelectionStart += 1;
                        }
                    } else if (args.Key >= KeyboardKey.D0 && args.Key <= KeyboardKey.D9) {
                        Text = Text.Insert((int)SelectionStart, ((char)((int)args.Key + 0)).ToString());

                        if (SelectionStart < Text.Length) {
                            SelectionStart += 1;
                        }
                    } else if (args.Key >= KeyboardKey.A && args.Key <= KeyboardKey.Z) {
                        if (TackInput.Instance.InputBufferCapsLock || TackInput.Instance.InputBufferShift) {
                            Text = Text.Insert((int)SelectionStart, ((char)((int)args.Key + 0)).ToString());
                        } else {
                            Text = Text.Insert((int)SelectionStart, ((char)((int)args.Key + 32)).ToString());
                        }

                        if (SelectionStart < Text.Length) {
                            SelectionStart += 1;
                        }
                    }

                    m_showCaret = true;
                    m_timeAtLastCaretSwitch = (float)EngineTimer.Instance.TotalRunTime;
                }
            }

            if (!Text.Equals(oldText)) {
                if (OnTextChangedEvent != null) {
                    if (OnTextChangedEvent.GetInvocationList().Length > 0) {
                        OnTextChangedEvent.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void BackspaceCharacter() {
            if (SelectionStart > 0) {
                Text = Text.Remove((int)SelectionStart - 1, 1);
            }

            if (SelectionStart > 0) {
                SelectionStart -= 1;
            }
        }

        private void DeleteCharacter() {
            if (SelectionStart < Text.Length) {
                Text = Text.Remove((int)SelectionStart, 1);
            }
        }
    }
}
