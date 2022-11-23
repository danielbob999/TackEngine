using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Physics;
using TackEngineLib.Main;
using TackEngineLib.Math;
using tainicom.Aether.Physics2D.Common;

namespace TackEngineLib.Objects.Components {
    public class RectanglePhysicsComponent : BasePhysicsComponent {

        public RectanglePhysicsComponent() : base() {
            Mass = 1f;
            IsStatic = false;
            IsAffectedByGravity = false;
            Friction = 1f;
            Restitution = 1f;
            FixedRotation = false;
        }

        /// <summary>
        /// Creates a new RectanglePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        public RectanglePhysicsComponent(float mass) : base() {
            Mass = mass;
            IsStatic = false;
            IsAffectedByGravity = false;
            Friction = 1f;
            Restitution = 1f;
            FixedRotation = false;
        }

        /// <summary>
        /// Creates a new RectanglePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        /// <param name="isStatic">Is this component static? If true, it cannot be moved.</param>
        public RectanglePhysicsComponent(float mass, bool isStatic) : base() {
            Mass = mass;
            IsStatic = isStatic;
            IsAffectedByGravity = false;
            Friction = 1f;
            Restitution = 1f;
            FixedRotation = false;
        }

        /// <summary>
        /// Creates a new RectanglePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        /// <param name="isStatic">Is this component static? If true, it cannot be moved.</param>
        /// <param name="isAffectedByGravity">Is this component affected by gravity?</param>
        public RectanglePhysicsComponent(float mass, bool isStatic, bool isAffectedByGravity) : base() {
            Mass = mass;
            IsStatic = isStatic;
            IsAffectedByGravity = isAffectedByGravity;
            Friction = 1f;
            Restitution = 1f;
            FixedRotation = false;
        }

        /// <summary>
        /// Creates a new RectanglePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        /// <param name="isStatic">Is this component static? If true, it cannot be moved.</param>
        /// <param name="isAffectedByGravity">Is this component affected by gravity?</param>
        /// <param name="fixedRotation">Should this component have a fixed rotation?</param>
        public RectanglePhysicsComponent(float mass, bool isStatic, bool isAffectedByGravity, bool fixedRotation) : base() {
            Mass = mass;
            IsStatic = isStatic;
            IsAffectedByGravity = isAffectedByGravity;
            Friction = 1f;
            Restitution = 1f;
            FixedRotation = fixedRotation;
        }

        /// <summary>
        /// Creates a new RectanglePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        /// <param name="isStatic">Is this component static? If true, it cannot be moved.</param>
        /// <param name="isAffectedByGravity">Is this component affected by gravity?</param>
        /// <param name="fixedRotation">Should this component have a fixed rotation?</param>
        /// <param name="friction">The friction of this component when colliding with other components</param>
        public RectanglePhysicsComponent(float mass, bool isStatic, bool isAffectedByGravity, bool fixedRotation, float friction) : base() {
            Mass = mass;
            IsStatic = isStatic;
            IsAffectedByGravity = isAffectedByGravity;
            Friction = friction;
            Restitution = 1f;
            FixedRotation = fixedRotation;
        }

        /// <summary>
        /// Creates a new RectanglePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        /// <param name="isStatic">Is this component static? If true, it cannot be moved.</param>
        /// <param name="isAffectedByGravity">Is this component affected by gravity?</param>
        /// <param name="fixedRotation">Should this component have a fixed rotation?</param>
        /// <param name="friction">The friction of this component when colliding with other components</param>
        /// <param name="restitution">The restitution of this component</param>
        public RectanglePhysicsComponent(float mass, bool isStatic, bool isAffectedByGravity, bool fixedRotation, float friction, float restitution) : base() {
            Mass = mass;
            IsStatic = isStatic;
            IsAffectedByGravity = isAffectedByGravity;
            Friction = friction;
            Restitution = restitution;
            FixedRotation = fixedRotation;
        }

        public override void OnStart() {
            base.OnStart();
        }

        public override void OnUpdate() {
            base.OnUpdate();
        }

        public override void OnGUIRender() {
            base.OnGUIRender();
        }

        public override void OnClose() {
            base.OnClose();
        }
    }
}
