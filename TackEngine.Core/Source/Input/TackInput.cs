/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Engine;
using TackEngine.Core.Objects.Components;
using System.Diagnostics;
using TackEngine.Core.Math;

namespace TackEngine.Core.Input {
    public class TackInput {
        public static TackInput Instance { get; private set; } = null;

        // Keyboard keys
        private bool[] mKeysHeld;
        private bool[] mKeysDownPerFrame;
        private bool[] mKeysUpPerFrame;
        private bool[] mLastFramesKeys;

        // Mouse button keys
        private bool[] mMouseKeysHeld;
        private bool[] mMouseKeysDownPerFrame;
        private bool[] mMouseKeysUpPerFrame;
        private bool[] mLastFrameMouseKeys;

        // Touch Input
        private bool mTouchUpPerFrame;
        private bool mTouchDownPerFrame;
        private bool mTouchDownLock;
        private bool mTouchHeld;

        // Mobile sensor data
        private float mGyroscopeMinChange;
        private Vector3 mGyroscopeRotation;
        private long mGyroscopeTimestamp;

        // Gamepads
        //private List<GamepadData> m_connectedGamepads;

        // Key Down/Up Lockers
        private bool[] locker_mKeysDownPerFrame;

        private bool[] locker_mMouseKeysDownPerFrame;

        private List<KeyboardKey> mInputBuffer = new List<KeyboardKey>();

        private bool mInputBufferCapsLock = false;
        private bool mInputBufferShift = false;

        // Tells TackInput that there is a TackGUI.InputField active and needs input
        private bool mGUIInputRequired = false;

        /// <summary>
        /// A bool that tells TackInput that a TackGUI.InputField is active and requires input
        /// </summary>
        internal bool GUIInputRequired {
            get { return mGUIInputRequired; }
            set {
                mGUIInputRequired = value;
                //TackConsole.EngineLog(TackConsole.LogType.Message, string.Format("{0} GUI input", mGUIInputRequired ? "Enabled" : "Disabled"));
            }
        }

        internal bool InputBufferCapsLock {
            get { return mInputBufferCapsLock; }
        }

        internal bool InputBufferShift {
            get { return mInputBufferShift; }
        }

        public Vector2i MousePosition { get; internal set; }

        public Vector2f MousePositionInWorld {
            get {
                Vector2f clampedMousePos = new Vector2f(
                (MousePosition.X / (TackEngineInstance.Instance.Window.WindowSize.X / 2.0f)) - 1f,
                ((MousePosition.Y / (TackEngineInstance.Instance.Window.WindowSize.Y / 2.0f)) - 1f) * -1f);

                Vector2f cameraPosition = Camera.MainCamera.GetParent().Position;

                return new Vector2f(
                    (clampedMousePos.X * (TackEngineInstance.Instance.Window.WindowSize.X / (2.0f * Camera.MainCamera.ZoomFactor))) + cameraPosition.X,
                    (clampedMousePos.Y * (TackEngineInstance.Instance.Window.WindowSize.Y / (2.0f * Camera.MainCamera.ZoomFactor))) + cameraPosition.Y);
            }
        }

        public Vector2i TouchPosition { get; internal set; }

        public Vector2f TouchPositionInWorld {
            get {
                Vector2f clampedTouchPos = new Vector2f(
                (TouchPosition.X / (TackEngineInstance.Instance.Window.WindowSize.X / 2.0f)) - 1f,
                ((TouchPosition.Y / (TackEngineInstance.Instance.Window.WindowSize.Y / 2.0f)) - 1f) * -1f);

                Vector2f cameraPosition = Camera.MainCamera.GetParent().Position;

                return new Vector2f(
                    (clampedTouchPos.X * (TackEngineInstance.Instance.Window.WindowSize.X / (2.0f * Camera.MainCamera.ZoomFactor))) + cameraPosition.X,
                    (clampedTouchPos.Y * (TackEngineInstance.Instance.Window.WindowSize.Y / (2.0f * Camera.MainCamera.ZoomFactor))) + cameraPosition.Y);
            }
        }

        public Vector3 GyroscopeRotation { get { return mGyroscopeRotation; } }

        public int MouseScrollWheelChange { get; private set; }

        internal TackInput() {
            Instance = this;
        }

        internal void OnStart() {
            /*
            if (Console.CapsLock) {
                mInputBufferCapsLock = true;
            }
            */

            //m_connectedGamepads = new List<GamepadData>();

            // Keyboard keys
            mKeysHeld = new bool[1024];
            mLastFramesKeys = new bool[1024];
            mKeysDownPerFrame = new bool[1024];
            mKeysUpPerFrame = new bool[1024];

            locker_mKeysDownPerFrame = new bool[1024];

            // Mouse keys
            mMouseKeysHeld = new bool[1024];
            mLastFrameMouseKeys = new bool[1024];
            mMouseKeysDownPerFrame = new bool[1024];
            mMouseKeysUpPerFrame = new bool[1024];

            locker_mMouseKeysDownPerFrame = new bool[1024];

            mGyroscopeMinChange = 0.05f;
            mGyroscopeRotation = new Vector3(0, 0, 0);

            GUIInputRequired = false;
        }

        internal void OnUpdate() {
            mLastFramesKeys = mKeysHeld;
            mLastFrameMouseKeys = mMouseKeysHeld;

            for (int i = 0; i < 1024; i++) {
                mKeysDownPerFrame[i] = false;
                mKeysUpPerFrame[i] = false;

                mMouseKeysDownPerFrame[i] = false;
                mMouseKeysUpPerFrame[i] = false;
            }

            MouseScrollWheelChange = 0;

            mTouchDownLock = false;
            mTouchDownPerFrame = false;
            mTouchUpPerFrame = false;
        }

        internal void KeyDownEvent(KeyboardKey _key) {
            /*
            // Add character to the input buffer.
            // NOTE: Don't register the key as being pressed
            if (mGUIInputRequired)
            {
                if (FindCharacterFromKeyCode(_key) == 8)
                {
                    if (mInputBuffer.Count > 0)
                        mInputBuffer.RemoveAt(mInputBuffer.Count - 1);
                    return;
                }

                if (FindCharacterFromKeyCode(_key) != 0)
                    mInputBuffer.Add((char)FindCharacterFromKeyCode(_key));

                return;
            }*/

            /*
            if (mGUIInputRequired)
            {
                mInputBuffer.Add(_key);
                //Console.WriteLine("Register key down: [{0}]", _key.ToString());

                if (_key == KeyboardKey.CapsLock)
                    mInputBufferCapsLock = !mInputBufferCapsLock;

                if (_key == KeyboardKey.ShiftLeft || _key == KeyboardKey.ShiftRight) {
                    mInputBufferShift = true;
                }
            }*/

            if ((int)_key < 0 || (int)_key >= mKeysDownPerFrame.Length) {
                return;
            }

            if (mGUIInputRequired) {
                if (_key == KeyboardKey.CapsLock) {
                    mInputBufferCapsLock = !mInputBufferCapsLock;
                }

                if (_key == KeyboardKey.LeftShift || _key == KeyboardKey.RightShift) {
                    mInputBufferShift = true;
                }
            }

            if (!locker_mKeysDownPerFrame[(int)_key]) // if the down key isn't locked
            {
                mKeysDownPerFrame[(int)_key] = true;
                locker_mKeysDownPerFrame[(int)_key] = true;

                if (mGUIInputRequired || (TackEngine.Core.GUI.BaseTackGUI.Instance.FocusedGUIObject != null)) {
                    GUI.BaseTackGUI.Instance.RegisterKeyboardEvent(new GUI.Events.GUIKeyboardEvent(_key, KeyboardKeyAction.Down, mInputBufferShift, mInputBufferCapsLock));
                }
            }

            mKeysHeld[(int)_key] = true;
        }

        internal void KeyUpEvent(KeyboardKey _key) {
            if ((int)_key < 0 || (int)_key >= mKeysDownPerFrame.Length) {
                return;
            }

            if (mGUIInputRequired || (TackEngine.Core.GUI.BaseTackGUI.Instance.FocusedGUIObject != null)) {
                if (_key == KeyboardKey.LeftShift || _key == KeyboardKey.RightShift) {
                    mInputBufferShift = false;
                }

                GUI.BaseTackGUI.Instance.RegisterKeyboardEvent(new GUI.Events.GUIKeyboardEvent(_key, KeyboardKeyAction.Up, mInputBufferShift, mInputBufferCapsLock));
            }

            mKeysUpPerFrame[(int)_key] = true;
            mKeysHeld[(int)_key] = false;

            locker_mKeysDownPerFrame[(int)_key] = false; // Unlock the down key
        }

        internal void MouseMoveEvent(int _x, int _y) {
            MousePosition = new Vector2i(_x, _y);
        }

        internal void MouseScrollEvent(int moveAmount) {
            MouseScrollWheelChange = moveAmount;
        }

        internal void MouseDownEvent(MouseButtonKey _key) {
            if ((int)_key < 0 || (int)_key >= mMouseKeysDownPerFrame.Length) {
                return;
            }

            if (!locker_mMouseKeysDownPerFrame[(int)_key]) // if the down key isn't locked
            {
                mMouseKeysDownPerFrame[(int)_key] = true;
                locker_mMouseKeysDownPerFrame[(int)_key] = true;

                GUI.BaseTackGUI.Instance.RegisterMouseEvent(new GUI.Events.GUIMouseEvent(MousePosition, _key, MouseButtonAction.Down));
            }

            mMouseKeysHeld[(int)_key] = true;

        }

        internal void MouseUpEvent(MouseButtonKey _key) {
            if ((int)_key < 0 || (int)_key >= mMouseKeysDownPerFrame.Length) {
                return;
            }

            mMouseKeysUpPerFrame[(int)_key] = true;
            mMouseKeysHeld[(int)_key] = false;

            locker_mMouseKeysDownPerFrame[(int)_key] = false; // Unlock the down key

            GUI.BaseTackGUI.Instance.RegisterMouseEvent(new GUI.Events.GUIMouseEvent(MousePosition, _key, MouseButtonAction.Up));
        }

        internal void TouchDownEvent(Vector2i touchPosition) {
            TouchPosition = touchPosition;

            if (!mTouchDownLock) { // if the touch down var isn't locked
                mTouchDownPerFrame = true;
                mTouchDownLock = true;

                GUI.BaseTackGUI.Instance.RegisterMouseEvent(new GUI.Events.GUIMouseEvent(TouchPosition, MouseButtonKey.Left, MouseButtonAction.Down));
            }

            mTouchHeld = true;
        }

        internal void TouchUpEvent(Vector2i touchPosition) {
            TouchPosition = touchPosition;

            mTouchUpPerFrame = true;
            mTouchHeld = false;

            mTouchDownLock = false; // Unlock the touch var

            GUI.BaseTackGUI.Instance.RegisterMouseEvent(new GUI.Events.GUIMouseEvent(TouchPosition, MouseButtonKey.Left, MouseButtonAction.Up));
        }

        internal void TouchDragEvent(Vector2i touchPosition) {
            TouchPosition = touchPosition;
        }

        internal void GyroscopeChangeEvent(Vector3 change, long timestamp) {
            if (TackMath.Abs(change.X) > mGyroscopeMinChange) {
                mGyroscopeRotation.X += change.X;
            }

            if (TackMath.Abs(change.Y) > mGyroscopeMinChange) {
                mGyroscopeRotation.Y += change.Y;
            }

            if (TackMath.Abs(change.Z) > mGyroscopeMinChange) {
                mGyroscopeRotation.Z += change.Z;
            }
        }

        public static bool KeyDown(KeyboardKey _keyCode) {
            if (Instance == null) {
                return false;
            }

            if (Instance.GUIInputRequired)
                return false;

            return Instance.mKeysDownPerFrame[(int)_keyCode];
        }

        /// <summary>
        /// Returns true if the specified KeyboardKey was depressed during the last frame.
        ///     Should only be used when the a GUI InputField is getting input
        /// </summary>
        /// <param name="a_keyCode"></param>
        /// <returns></returns>
        internal bool InputActiveKeyDown(KeyboardKey a_keyCode) {
            return mKeysDownPerFrame[(int)a_keyCode];
        }

        public static bool KeyHeld(KeyboardKey _keyCode) {
            if (Instance == null) {
                return false;
            }

            if (Instance.GUIInputRequired) {
                return false;
            }

            return Instance.mKeysHeld[(int)_keyCode];
        }

        /// <summary>
        /// Returns true if the specified KeyboardKey was held during the last frame.
        ///     Should only be used when the a GUI InputField is getting input
        /// </summary>
        /// <param name="a_keyCode"></param>
        /// <returns></returns>
        internal bool InputActiveKeyHeld(KeyboardKey a_keyCode) {
            return mKeysHeld[(int)a_keyCode];
        }

        public static bool KeyUp(KeyboardKey _keyCode) {
            if (Instance == null) {
                return false;
            }

            if (Instance.GUIInputRequired) {
                return false;
            }

            return Instance.mKeysUpPerFrame[(int)_keyCode];
        }

        /// <summary>
        /// Returns true if the specified KeyboardKey was lifted during the last frame.
        ///     Should only be used when the a GUI InputField is getting input
        /// </summary>
        /// <param name="a_keyCode"></param>
        /// <returns></returns>
        internal bool InputActiveKeyUp(KeyboardKey a_keyCode) {
            return mKeysUpPerFrame[(int)a_keyCode];
        }

        public static bool MouseButtonDown(MouseButtonKey _key) {
            if (Instance == null) {
                return false;
            }

            return Instance.mMouseKeysDownPerFrame[(int)_key];
        }

        public static bool MouseButtonHeld(MouseButtonKey _key) {
            if (Instance == null) {
                return false;
            }

            return Instance.mMouseKeysHeld[(int)_key];
        }

        public static bool MouseButtonUp(MouseButtonKey _key) {
            if (Instance == null) {
                return false;
            }

            return Instance.mMouseKeysUpPerFrame[(int)_key];
        }

        public static bool TouchUp() {
            return Instance.mTouchUpPerFrame;
        }

        public static bool TouchDown() {
            return Instance.mTouchDownPerFrame;
        }

        public static bool TouchHeld() {
            return Instance.mTouchHeld;
        }

        internal string GetInputBuffer() {
            string returnStr = "";

            foreach (char c in mInputBuffer) {
                returnStr += c;
            }

            return returnStr;
        }

        internal KeyboardKey[] GetInputBufferArray() {
            return mInputBuffer.ToArray();
        }

        /// <summary>
        /// Gets a character based on a KeyboardKey code.
        /// </summary>
        /// <param name="_key"></param>
        /// <returns>Returns an ASCII keycode that represents the letter than has been pressed.
        /// - Returns 8 if backspace has been pressed
        /// - Returns 0 if useless button was pressed (Caps lock, shift, ect)
        /// </returns>
        internal int FindCharacterFromKeyCode(KeyboardKey _key) {
            if (_key == KeyboardKey.CapsLock) {
                mInputBufferCapsLock = !mInputBufferCapsLock;
                return 0;
            }

            if (_key == KeyboardKey.Backspace)
                return 8;

            if (_key == KeyboardKey.Space)
                return 32;

            if (_key == KeyboardKey.Period)
                return 46;

            if (_key >= KeyboardKey.D0 && _key <= KeyboardKey.D9)
                return ((int)_key - 61);

            if (_key >= KeyboardKey.A && _key <= KeyboardKey.Z) {
                if (mInputBufferCapsLock)
                    return ((int)_key - 18);
                else
                    return ((int)_key + 14);
            }

            return 0;
        }

        /// <summary>
        /// Clears the GUI input buffer
        /// </summary>
        public void ClearInputBuffer() {
            mInputBuffer.Clear();
        }

        /// <summary>
        /// Gets the first element in the input buffer, then removes it
        /// </summary>
        /// <param name="a_outKey"></param>
        /// <returns></returns>
        public bool GetKeyFromInputBuffer(out KeyboardKey a_outKey) {
            if (mInputBuffer.Count == 0) {
                a_outKey = KeyboardKey.A;
                return false;
            }

            a_outKey = mInputBuffer[0];
            mInputBuffer.RemoveAt(0);
            return true;
        }
    }
}
