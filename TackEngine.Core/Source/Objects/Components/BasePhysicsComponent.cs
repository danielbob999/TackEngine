﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;

using TackEngine.Core.Main;
using TackEngine.Core.Physics;
using TackEngine.Core.Renderer;

namespace TackEngine.Core.Objects.Components {
    public abstract class BasePhysicsComponent : TackComponent {

        private float m_mass;
        private float m_friction;
        private float m_restitution;
        protected Body m_physicsBody = null;
        protected List<Fixture> m_fixtures = null;

        private bool m_affectedByGravity;
        private bool m_isStatic;

        public override bool Active {
            get { return base.Active; }
            set {
                base.Active = value;

                if (m_physicsBody != null) {
                    m_physicsBody.Enabled = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets the mass of the physics body. If setting the mass, the value must be a number larger than 0 and smaller than Infinity
        /// </summary>
        public float Mass {
            get { return m_mass; }
            set {
                if (value <= 0) {
                    throw new Exception("The mass of a physics body must be larger than 0");
                }

                if (value == float.PositiveInfinity || value == float.NegativeInfinity) {
                    throw new Exception("The mass of a physics body cannot be equal to Inifity");
                }

                m_mass = value;
            }
        }

        /// <summary>
        /// Gets/Sets whether the physics body is static. A static physics body cannot be moved with a force or collisions
        /// </summary>
        public bool IsStatic {
            get { return m_isStatic; }
            set { m_isStatic = value; }
        }

        /// <summary>
        /// Gets/Sets whether the physics body is affected by gravity
        /// </summary>
        public bool IsAffectedByGravity {
            get { return m_affectedByGravity; }
            set { 
                m_affectedByGravity = value;
            }
        }

        /// <summary>
        /// Gets/Sets the drag of the physics body
        /// </summary>
        public float Friction {
            get { return m_friction; }
            set { 
                m_friction = value;
            }
        }

        /// <summary>
        /// Gets/Sets the restitution (or bounciness) of the physics body
        /// </summary>
        public float Restitution {
            get { return m_restitution; }
            set { 
                m_restitution = value;
            }
        }

        /// <summary>
        /// Gets/Sets the linear velocity of this physics body
        /// </summary>
        public Vector2f Velocity {
            get { return new Vector2f(m_physicsBody.LinearVelocity.X, m_physicsBody.LinearVelocity.Y); }
            set { m_physicsBody.LinearVelocity = new Vector2(value.X, value.Y); }
        }

        /// <summary>
        /// Gets/Sets the angular velocity of this physics body
        /// </summary>
        public float AngularVelocity {
            get { return m_physicsBody.AngularVelocity; }
            set { m_physicsBody.AngularVelocity = value; }
        }

        /// <summary>
        /// Gets/Sets whether this physics component has a fixed rotation
        /// </summary>
        public bool FixedRotation { get; set; }

        /// <summary>
        /// Gets/Sets whether this physics component is a trigger. 
        /// A trigger component will still return the OnCollision callback methods but will not
        ///     physically collide with other objects
        /// </summary>
        public bool IsTrigger { get; set; } = false;

        // Physics Events
        public delegate void OnCollisionMethodPrototype(CollisionData data);

        /// <summary>
        /// A event that is called when this component collides with another.
        /// - If either of the components is a trigger component (IsTrigger = true), this will be called every frame until they separate
        /// - If neither components are a trigger, this will only be called one time (for the initial collision)
        /// </summary>
        public event OnCollisionMethodPrototype OnCollision;

        /// <summary>
        /// A event that is called when this component separates from another.
        /// - If either of the components is a trigger component (IsTrigger = true), this will not be called
        /// - If neither components are a trigger, this will only be called one time (for the initial separation)
        /// </summary>
        public event OnCollisionMethodPrototype OnSeparation;

        internal Body PhysicsBody { get { return m_physicsBody; } }
        internal List<Fixture> PhysicsFixture { get { return m_fixtures; } }

        protected BasePhysicsComponent() {
            m_fixtures = new List<Fixture>();
        }

        public override void OnStart() {
            base.OnStart();
        }

        public override void OnUpdate() {
            base.OnUpdate();

            if (m_physicsBody != null) {
                GetParent().ChangePosition(new Vector2f(m_physicsBody.Position.X * 100f, m_physicsBody.Position.Y * 100f));
                GetParent().ChangeRotation(Math.TackMath.RadToDeg(m_physicsBody.Rotation));

                m_physicsBody.Mass = Mass;
                m_physicsBody.BodyType = GetBodyType();
                m_physicsBody.FixedRotation = FixedRotation;

                m_physicsBody.Enabled = Active;
            }

            if (m_fixtures != null) {
                foreach (Fixture f in m_fixtures) {
                    f.Friction = Friction;
                    f.Restitution = Restitution;
                    f.IsSensor = IsTrigger;
                }
            }
        }

        public override void OnClose() {
            base.OnClose();

            TackPhysics.Instance.DeregisterPhysicsComponent(this);
        }

        public override void OnAttachedToTackObject() {
            base.OnAttachedToTackObject();

            GenerateBody();
        }

        internal bool InternalOnCollision(Fixture sender, Fixture other, tainicom.Aether.Physics2D.Dynamics.Contacts.Contact contact) {
            if (!Active) {
                return false;
            }

            TackObject collidedObject = TackObjectManager.Instance.GetByHash((string)other.Body.Tag);

            CallOnCollision(new CollisionData((BasePhysicsComponent)Utilities.FirstNotNull(
                collidedObject.GetComponent<RectanglePhysicsComponent>(),
                collidedObject.GetComponent<CirclePhysicsComponent>())));

            return true;
        }

        internal void InternalOnSeparation(Fixture sender, Fixture other, tainicom.Aether.Physics2D.Dynamics.Contacts.Contact contact) {
            if (!Active) {
                return;
            }

            TackObject collidedObject = TackObjectManager.Instance.GetByHash((string)other.Body.Tag);

            CallOnSeparation(new CollisionData((BasePhysicsComponent)Utilities.FirstNotNull(
                collidedObject.GetComponent<RectanglePhysicsComponent>(),
                collidedObject.GetComponent<CirclePhysicsComponent>())));
        }

        public override void OnDetachedFromTackObject() {
            base.OnDetachedFromTackObject();

            TackPhysics.Instance.DeregisterPhysicsComponent(this);
        }

        public override void OnPositionChanged() {
            base.OnPositionChanged();

            // Change the body position. This will only get called if the user manually changes the position
            m_physicsBody.Position = new Vector2(GetParent().Position.X / 100f, GetParent().Position.Y / 100f);
        }

        public override void OnSizeChanged() {
            base.OnSizeChanged();

            // Generate because we cannot change the fixture shape dynamically
            GenerateBody();
        }

        public override void OnRotationChanged() {
            base.OnRotationChanged();

            // Change the body rotation. This will only get called if the user manually changes the rotation
            m_physicsBody.Rotation = Math.TackMath.DegToRad(GetParent().Rotation);
        }

        public virtual void AddForce(Vector2f force) {
            if (m_physicsBody != null) {
                m_physicsBody.ApplyForce(new Vector2(force.X, force.Y));
            }
        }

        internal void CallOnCollision(CollisionData data) {
            if (OnCollision != null) {
                if (OnCollision.GetInvocationList().Length > 0) {
                    OnCollision.Invoke(data);
                }
            }
        }

        internal void CallOnSeparation(CollisionData data) {
            if (OnSeparation != null) {
                if (OnSeparation.GetInvocationList().Length > 0) {
                    OnSeparation.Invoke(data);
                }
            }
        }

        internal BodyType GetBodyType() {
            if (IsStatic) {
                return BodyType.Static;
            }

            return BodyType.Dynamic;
        }

        protected virtual void DestroyBody() {
            if (m_physicsBody == null) {
                return;
            }

            if (m_fixtures == null) {
                return;
            }

            foreach (Fixture f in m_fixtures) {
                m_physicsBody.Remove(f);
            }

            TackPhysics.Instance.GetWorld().Remove(m_physicsBody);

            m_physicsBody = null;
        }

        /// <summary>
        /// Generates a new Body. If not overriden, will generate a rectangular body
        /// </summary>
        protected virtual void GenerateBody() {
            // Destroy the body before we regenerate it
            DestroyBody();

            m_physicsBody = TackPhysics.Instance.GetWorld().CreateBody(new Vector2((GetParent().Position.X / 100f), (GetParent().Position.Y / 100f)), Math.TackMath.DegToRad(GetParent().Rotation), GetBodyType());
            m_physicsBody.FixedRotation = false;
            m_physicsBody.SleepingAllowed = true;
            m_physicsBody.IgnoreGravity = !IsAffectedByGravity;
            m_physicsBody.Tag = GetParent().Hash;

            Fixture fixture = m_physicsBody.CreateRectangle((GetParent().Size.X / 100f), (GetParent().Size.Y / 100f), 1, new Vector2(0, 0));
            fixture.Restitution = Restitution;
            fixture.Friction = Friction;
            fixture.IsSensor = IsTrigger;

            m_fixtures.Add(fixture);

            m_physicsBody.OnCollision += InternalOnCollision;
            m_physicsBody.OnSeparation += InternalOnSeparation;
        }

        internal virtual void OnDebugDraw() {
        }
    }
}
