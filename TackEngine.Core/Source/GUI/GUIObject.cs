using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.GUI.Events;
using TackEngine.Core.SceneManagement;

namespace TackEngine.Core.GUI {
    public class GUIObject {
        private static int s_nextId = 0;

        private bool m_active;
        private bool[] m_mouseDownWaitingForUp;
        private int? m_parentId = null;
        private GUIMaskData m_previousMaskData;
        protected Vector2f m_position;
        protected Vector2f m_localPosition;
        protected Vector2f m_size;

        public bool Active {
            get { return m_active; }
            set { m_active = value; }
        }

        internal int Id { get; private set; }
        internal int? ParentId { get { return m_parentId; } }
        internal Type LinkedSceneType { get; set; }

        public List<GUIObject> ChildObjects { get; private set; }

        /// <summary>
        /// Gets the parent TackObject of this object
        /// </summary>
        public GUIObject Parent { get { return BaseTackGUI.Instance.GetGUIObjectById(m_parentId); } }

        /// <summary>
        /// The position of this GUIObject in world space
        /// </summary>
        public virtual Vector2f Position {
            get {
                if (m_parentId == null || BaseTackGUI.Instance.GetGUIObjectById(m_parentId) == null) {
                    return LocalPosition;
                }

                return BaseTackGUI.Instance.GetGUIObjectById(m_parentId).Position + LocalPosition;
            }

            set {
                if (Parent == null) {
                    LocalPosition = value;
                } else {
                    LocalPosition = Parent.Position - value;
                }
            }
        }

        /// <summary>
        /// The position of this GUIObject relative to it's parent
        /// </summary>
        public virtual Vector2f LocalPosition { 
            get { return m_localPosition; } 
            set { m_localPosition = value; } 
        }

        /// <summary>
        /// The size of this GUIObject
        /// </summary>
        public virtual Vector2f Size {
            get { return m_size; }
            set { m_size = value; }
        }

        /// <summary>
        /// Is the mouse hovering over this GUIObject?
        /// </summary>
        public bool IsMouseHovering { get; private set; }

        public bool IsFocused { get; private set; }

        public int RenderLayer { get; set; }

        /// <summary>
        /// The name of this GUIObject. This does not have to be unique but can be used for identification
        /// </summary>
        public string Name { get; set; }

        internal GUIObject() {
            Id = s_nextId++;

            ChildObjects = new List<GUIObject>();
            RenderLayer = 1;
            Active = true;
            m_mouseDownWaitingForUp = new bool[12];
            
            if (SceneManager.Instance.CurrentScene != null) {
                LinkedSceneType = SceneManager.Instance.CurrentScene.GetType();
            }
        }

        internal virtual void OnStart() {

        }

        internal virtual void OnUpdate() {
            RectangleShape shape = GetShapeWithMask();

            if (Physics.AABB.IsPointInAABB(new Physics.AABB(new Vector2f(shape.X, shape.Y + shape.Height), new Vector2f(shape.X + shape.Width, shape.Y)), TackEngine.Core.Input.TackInput.Instance.MousePosition.ToVector2f())) {
                IsMouseHovering = true;
            } else {
                IsMouseHovering = false;
            }
        }

        internal virtual void OnRender(GUIMaskData maskData) {
            m_previousMaskData = maskData;
        }

        internal virtual void OnClose() {

        }

        internal virtual void OnMouseEvent(GUIMouseEventArgs args) {
            if (args.MouseAction == Input.MouseButtonAction.Down) {
                m_mouseDownWaitingForUp[(int)args.MouseButton] = true;
            }
        }

        internal virtual void OnKeyboardEvent(GUIKeyboardEventArgs args) {

        }

        internal virtual void OnFocusGained() {
            IsFocused = true;
        }

        internal virtual void OnFocusLost() {
            IsFocused = false;
        }

        protected bool IsWaitingForMouseUp(Input.MouseButtonKey mouseKey) {
            return m_mouseDownWaitingForUp[(int)mouseKey];
        }

        /// <summary>
        /// Sets the parent of this GUIObject
        /// </summary>
        /// <param name="obj"></param>
        public virtual void SetParent(GUIObject obj) {
            // If this object was a child, remove it from it's old parent
            GUIObject oldParent = Parent;

            if (oldParent != null) {
                oldParent.RemoveChild(this);
            }

            if (obj == null) {
                m_parentId = null;
                return;
            }

            m_parentId = obj.Id;

            obj.AddChild(this);
        }

        internal void AddChild(GUIObject obj) {
            if (ChildObjects == null) {
                ChildObjects = new List<GUIObject>();
            }

            if (ChildObjects.Find(o => o.Id == obj.Id) == null) {
                ChildObjects.Add(obj);
            }
        }

        internal void RemoveChild(GUIObject obj) {
            if (obj == null) {
                return;
            }

            obj.ChildObjects.Remove(obj);
        }

        public virtual void Destroy() {
            BaseTackGUI.Instance.DeregisterGUIObject(this);

            if (m_parentId != null) {
                Parent.ChildObjects.Remove(this);
            }
        }

        /// <summary>
        /// Gets the rectangle of this object with the last mask taken into account
        /// </summary>
        public RectangleShape GetShapeWithMask() {
            RectangleShape shape = new RectangleShape(Position, Size);

            if (m_previousMaskData == null) {
                return shape;
            }

            foreach (RectangleShape s in m_previousMaskData.Masks) {
                // If too far left
                if (shape.X < s.X) {
                    shape.X = s.X;
                }

                // If too far right
                if ((shape.X + shape.Width) > (s.X + s.Width)) {
                    shape.Width = (s.X + s.Width) - shape.X;
                }

                // If too far up
                if (shape.Y < s.Y) {
                    shape.Y = s.Y;
                }

                // If too far down
                if ((shape.Y + shape.Height) > (s.Y + s.Height)) {
                    shape.Height = (s.Y + s.Height) - shape.Y;
                }
            }

            return shape;
        }
    }
}
