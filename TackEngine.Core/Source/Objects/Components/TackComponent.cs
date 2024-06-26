﻿/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngine.Core.Objects.Components
{
    public class TackComponent {
        private static int s_nextId = 0;

        protected bool m_active = true;
        private string m_parentObjectHash;
        private int m_componentId;

        /// <summary>
        /// Is the component active on the TackObject
        /// </summary>
        public virtual bool Active { 
            get { return m_active;  } 
            set { m_active = value; } 
        }

        /// <summary>
        /// The ID of this TackComponent
        /// </summary>
        public int Id {
            get { return m_componentId; }
        }

        protected TackComponent() {
            m_componentId = s_nextId;
            s_nextId++;

            Active = true;
        }

        /// <summary>
        /// Called at the start of the TackObject lifecycle.
        /// This can either be during the next frame from when the TackObject is created OR
        /// at the start of the game engine
        /// </summary>
        public virtual void OnStart() {

        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public virtual void OnUpdate() {

        }

        public virtual void OnClose() {

        }

        /// <summary>
        /// And override-able method that occurs when the component gets added to a TackObject
        /// </summary>
        public virtual void OnAttachedToTackObject() {

        }

        /// <summary>
        /// And override-able method that occurs when the component gets removed from the parent TackObject
        /// Its is also called when the parent TackObject is destroyed
        /// </summary>
        public virtual void OnDetachedFromTackObject() {

        }

        /// <summary>
        /// An override-able method that occurs when the user clicks the parent TackObject
        /// </summary>
        public virtual void OnMouseClick() {

        }
        
        /// <summary>
        /// An override-able method that occurs when a user hovers over the parent TackObject
        /// </summary>
        public virtual void OnMouseOver() {

        }

        /// <summary>
        /// An override-able method that occurs when the mouse enters the bounds of the parent TackObject
        /// </summary>
        public virtual void OnMouseEnter() {

        }

        /// <summary>
        /// An override-able method that occurs when the mouse exits the bounds of the parent TackObject
        /// </summary>
        public virtual void OnMouseExit() {

        }

        /// <summary>
        /// An override-able method that occurs when a user manually changes the position of the connected TackObject
        /// This method is NOT called by external factors such as physics movement
        /// </summary>
        public virtual void OnPositionChanged() {

        }

        /// <summary>
        /// An override-able method that occurs when a user manually changes the size of the connected TackObject
        /// </summary>
        public virtual void OnSizeChanged() {

        }

        /// <summary>
        /// An override-able method that occurs when a user manually changes the rotation of the connected TackObject
        /// This method is NOT called by external factors such as physics movement
        /// </summary>
        public virtual void OnRotationChanged() {

        }

        public TackObject GetParent() {
            return TackObjectManager.Instance.GetByHash(m_parentObjectHash);
        }

        internal void SetParent(string hash) {
            m_parentObjectHash = hash;
        }

        public bool Equals(TackComponent comp) {
            if (comp.m_componentId == this.m_componentId) {
                return true;
            }

            return false;
        }
    }
}
