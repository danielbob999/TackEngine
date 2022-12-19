/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Engine;
using TackEngine.Core.Math;

namespace TackEngine.Core.Objects {
    /// <summary>
    /// The main class used by the TackEngine.Core to represent and object
    /// </summary>
    public class TackObject {
        private Vector2f m_scale;
        private float m_rotation;

        private Vector2f m_localPosition;
        private string m_parentHash = null;

        private List<object> m_components = new List<object>();

        internal string Hash { get; private set; }
        internal bool MouseHoverLastFrame { get; set; }

        internal Physics.AABB BoundingBox { 
            get { 
                return new Physics.AABB(
                    new Vector2f(Position.X - (m_scale.X / 2f), Position.Y - (m_scale.Y / 2f)),
                    new Vector2f(Position.X + (m_scale.X / 2f), Position.Y + (m_scale.Y / 2f)));
         } 
        }

        /// <summary>
        /// The name of this TackObject
        /// </summary>
        /// <datatype>string</datatype>
        public string Name { get; set; }

        /// <summary>
        /// The position of this TackObject in world space
        /// </summary>
        /// <datatype>Vector2f</datatype>
        public Vector2f Position {
            get {
                if (m_parentHash == null || TackObjectManager.Instance.GetByHash(m_parentHash) == null) {
                    return m_localPosition;
                }

                return TackObjectManager.Instance.GetByHash(m_parentHash).Position + m_localPosition;
            }

            set {
                if (Parent == null) {
                    m_localPosition = value;
                } else {
                    m_localPosition = Parent.Position - value;
                }

                m_components.ForEach(component => {
                    if (((TackComponent)component).Active) {
                        ((TackComponent)component).OnPositionChanged();
                    }
                });
            }
        }

        /// <summary>
        /// The position of this relative to it's parent TackObject
        /// </summary>
        public Vector2f LocalPosition {
            get { return m_localPosition; }
            set { m_localPosition = value; }
        }

        /// <summary>
        /// The scale of this TackObject
        /// </summary>
        /// <datatype>Vector2f</datatype>
        public Vector2f Scale {
            get { return m_scale; }
            set { 
                m_scale = value;

                m_components.ForEach(component => {
                    if (((TackComponent)component).Active) {
                        ((TackComponent)component).OnScaleChanged();
                    }
                });
            }
        }

        /// <summary>
        /// The rotation value of this TackObject
        /// </summary>
        /// <datatype>float</datatype>
        public float Rotation {
            get { return m_rotation; }
            set { 
                m_rotation = value;

                m_components.ForEach(component => {
                    if (((TackComponent)component).Active) {
                        ((TackComponent)component).OnRotationChanged();
                    }
                });
            }
        }

        /// <summary>
        /// Is this TackObject active in the scene?
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// The vector of moving the TackObject directly forward based on rotation
        /// </summary>
        public Vector2f Up {
            get {
                return new Vector2f(1.0f * (float)System.Math.Sin(TackMath.DegToRad(m_rotation)), 1.0f * (float)System.Math.Cos(TackMath.DegToRad(m_rotation)));
            }
        }

        /// <summary>
        /// The vector of moving the TackObject directly right based on rotation
        /// </summary>
        public Vector2f Right {
            get {
                return new Vector2f(1.0f * (float)System.Math.Sin(TackMath.DegToRad(m_rotation + 90)), 1.0f * (float)System.Math.Cos(TackMath.DegToRad(m_rotation + 90)));
            }
        }

        /// <summary>
        /// Gets the parent TackObject of this object
        /// </summary>
        public TackObject Parent { get { return TackObjectManager.Instance.GetByHash(m_parentHash); } }

        // CONSTRUCTORS

        internal TackObject(string name, Vector2f position, Vector2f scale, float rotation) {
            Active = true;
            Hash = null;
            Hash = TackObjectManager.Instance.GenerateNewHash();
            Name = name;
            Position = position;
            m_scale = scale;
            m_rotation = rotation;

            TackObjectManager.Instance.RegisterTackObject(this);
        }

        public void AddComponent(object _component) {
            if (!_component.GetType().IsSubclassOf(typeof(TackComponent))) {
                TackConsole.EngineLog(TackConsole.LogType.Debug, string.Format("'{0}' cannot be added to TackObject with name '{1}' because it does not inherit from '{2}'", _component.GetType().Name, Name, typeof(TackComponent).Name));
                return;
            }

            if (_component != null) {
                ((TackComponent)_component).SetParent(Hash);
                ((TackComponent)_component).OnAttachedToTackObject();
                m_components.Add(_component);

                TackConsole.EngineLog(TackConsole.LogType.Debug, string.Format("Added a '{0}' component to TackObject with name '{1}'", _component.GetType(), Name));
            }
        }

        public T GetComponent<T>() where T : TackComponent {
            foreach (object comp in m_components) {
                if (comp.GetType() == typeof(T) || comp.GetType().IsSubclassOf(typeof(T)))
                    return (T)comp;
            }

            return null;
        }

        public T GetComponent<T>(int componentId) where T : TackComponent {
            foreach (object comp in m_components) {
                if ((comp.GetType() == typeof(T) || comp.GetType().IsSubclassOf(typeof(T))) && ((TackComponent)comp).Id == componentId)
                    return (T)comp;
            }

            return null;
        }

        /// <summary>
        /// Removes the TackComponent from this TackObject and returns it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T RemoveComponent<T>() where T : TackComponent {
            foreach (object comp in m_components) {
                if (comp.GetType() == typeof(T)) {
                    TackComponent tcomp = (T)comp;

                    tcomp.OnDetachedFromTackObject();

                    return (T)comp;
                }
            }

            return null;
        }

        internal void RemoveComponent(object comp) {
            if (m_components.Contains(comp)) {
                ((TackComponent)comp).OnDetachedFromTackObject();
                m_components.Remove(comp);
            }
        }

        public void SetParent(TackObject obj) {
            if (obj == null) {
                m_parentHash = null;
                return;
            }

            m_parentHash = obj.Hash;
        }

        /// <summary>
        /// Returns true if this TackObject has a renderer component attached. This only works with the new IRendererComponent classes listed here
        /// <list type="bullet">
        /// <item>SpriteRendererComponent</item>
        /// <item>SpriteSheetRendererComponent</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        internal bool HasRendererComponent() {
            for (int i = 0; i < m_components.Count; i++) {
                if (m_components[i].GetType() == typeof(SpriteRendererComponent) || m_components[i].GetType() == typeof(SpriteSheetRendererComponent)) {
                    return true;
                }
            }

            return false;
        }

        internal IRendererComponent GetRendererComponent() {
            if (!HasRendererComponent()) {
                return null;
            }

            IRendererComponent rc = GetComponent<SpriteRendererComponent>();

            if (rc != null) {
                return rc;
            }

            rc = GetComponent<SpriteSheetRendererComponent>();

            if (rc != null) {
                return rc;
            }

            return null;
        }

        public TackComponent[] GetAllComponents() {
            TackComponent[] components = new TackComponent[m_components.Count];

            for (int i = 0; i < components.Length; i++) {
                components[i] = (TackComponent)m_components[i];
            }

            return components;
        }

        public TackComponent[] GetActiveComponents() {
            List<TackComponent> components = new List<TackComponent>();

            for (int i = 0; i < m_components.Count; i++) {
                if (((TackComponent)m_components[i]).Active) {
                    components.Add((TackComponent)m_components[i]);
                }
            }

            return components.ToArray();
        }

        public bool IsPointInArea(Vector2f _point) {
            Vector2f xConstraints = new Vector2f(Position.X - (m_scale.X / 2), Position.X + (m_scale.X / 2));
            Vector2f yConstraints = new Vector2f(Position.Y - (m_scale.Y / 2), Position.Y + (m_scale.Y / 2));

            if ((_point.X > xConstraints.X) && (_point.X < xConstraints.Y)) {

                if ((_point.Y > yConstraints.X) && (_point.Y < yConstraints.Y)) {
                    return true;
                }
            }

            return false;
        }

        internal void Move(Vector2f _vec) {
            Position += _vec;
        }

        /// <summary>
        /// Internal method that sets the position of the TackObject
        /// Use this when wanting to set the position of an TackObject but don't want to call the TackComponent.OnPositionChanged method on all TackComponents
        /// </summary>
        /// <param name="val"></param>
        internal void ChangePosition(Vector2f val) {
            Position = val;
        }

        /// <summary>
        /// Internal method that sets the scale of the TackObject
        /// Use this when wanting to set the scale of an TackObject but don't want to call the TackComponent.OnScaleChanged method on all TackComponents
        /// </summary>
        /// <param name="val"></param>
        internal void ChangeScale(Vector2f val) {
            m_scale = val;
        }

        /// <summary>
        /// Internal method that sets the rotation of the TackObject
        /// Use this when wanting to set the rotation of an TackObject but don't want to call the TackComponent.OnRotationChanged method on all TackComponents
        /// </summary>
        /// <param name="val"></param>
        internal void ChangeRotation(float val) {
            m_rotation = val;
        }

        public string GetHash() {
            return Hash;
        }

        /// <summary>
        /// Destroys the current TackObject. All components attached to this object will also be destroyed
        /// </summary>
        public void Destroy() {
            TackObjectManager.Instance.DeregisterTackObject(this);
        }

        public static TackObject Get(string _name) {
            return GetByName(_name);
        }

        public static TackObject GetByName(string name) {
            return TackObjectManager.Instance.GetByName(name);
        }

        internal static TackObject GetByHash(string hash) {
            return TackObjectManager.Instance.GetByHash(hash);
        }

        public static TackObject[] Get() {
            return TackObjectManager.Instance.GetAllTackObjects();
        }

        public static TackObject Create() {
            return new TackObject("New TackObject", new Vector2f(0, 0), new Vector2f(50, 50), 0);
        }

        public static TackObject Create(string name, Vector2f position) {
            return new TackObject(name, position, new Vector2f(50, 50), 0);
        }

        public static TackObject Create(string name, Vector2f position, Vector2f scale) {
            return new TackObject(name, position, scale, 0);
        }

        public static TackObject Create(string name, Vector2f position, Vector2f scale, float rotation) {
            return new TackObject(name, position, scale, rotation);
        }

        /// <summary>
        /// Determines whether a TackObject is active in world (taking into account parent active status)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static bool IsActiveInHierarchy(TackObject obj) {
            TackObject nextObject = obj;

            while (nextObject != null) {
                if (!nextObject.Active) {
                    return false;
                }

                nextObject = nextObject.Parent;
            }

            return true;
        }

        /// <summary>
        /// Determines whether a TackComponent is active in world (taking into account parent active status)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static bool IsComponentActiveInHierarchy(TackComponent comp) {
            if (!comp.Active) {
                return false;
            }

            TackObject nextObject = comp.GetParent();

            while (nextObject != null) {
                if (!nextObject.Active) {
                    return false;
                }

                nextObject = nextObject.Parent;
            }

            return true;
        }
    }
}
