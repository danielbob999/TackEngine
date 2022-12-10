using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Math;
using TackEngine.Core.Physics;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using tainicom.Aether.Physics2D.Dynamics;
using TackEngine.Core.Main;

namespace TackEngine.Core.Objects.Components {
    public class WheelPhysicsComponent : BasePhysicsComponent {

        private WheelJoint m_wheelJoint;
        private BasePhysicsComponent m_physicsParent;

        public float MaxTorque { get; set; }
        public float MotorSpeed { get; set; }
        public float Frequency { get; set; }
        public float Damping { get; set; }
        public bool MotorEnabled { get; set; }

        public WheelPhysicsComponent(BasePhysicsComponent parent) : base() {
            Mass = 1f;
            IsStatic = false;
            IsAffectedByGravity = true;
            Friction = 1f;
            Restitution = 0f;
            FixedRotation = false;
            MotorEnabled = true;

            MaxTorque = 20f;
            MotorSpeed = 0;
            Frequency = 4;
            Damping = 0.7f;

            m_physicsParent = parent;
        }

        /// <summary>
        /// Creates a new WheelPhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        public WheelPhysicsComponent(float friction, float maxTorque, float frequency, float damping) : base() {
            Mass = 1f;
            IsStatic = false;
            IsAffectedByGravity = true;
            Friction = friction;
            Restitution = 1f;
            FixedRotation = false;

            MaxTorque = maxTorque;
            MotorSpeed = 0f;
            Frequency = frequency;
            Damping = damping;
        }

        public override void OnUpdate() {
            base.OnUpdate();

            if (m_wheelJoint != null) {
                m_wheelJoint.MaxMotorTorque = MaxTorque;
                m_wheelJoint.MotorSpeed = MotorSpeed;
                m_wheelJoint.Frequency = Frequency;
                m_wheelJoint.DampingRatio = Damping;
                m_wheelJoint.MotorEnabled = MotorEnabled;
            }
        }

        protected override void GenerateBody() {
            // Destroy the body before we regenerate it
            DestroyBody();

            m_physicsBody = TackPhysics.Instance.GetWorld().CreateBody(new Vector2(GetParent().Position.X / 100f, GetParent().Position.Y / 100f), TackMath.DegToRad(GetParent().Rotation), GetBodyType());
            m_physicsBody.FixedRotation = false;
            m_physicsBody.SleepingAllowed = false;
            m_physicsBody.IgnoreGravity = !IsAffectedByGravity;
            m_physicsBody.Tag = GetParent().Hash;
            m_fixture = m_physicsBody.CreateCircle((GetParent().Scale.X / 2f) / 100f, 1);
            m_fixture.Restitution = 0f;
            m_fixture.Friction = Friction;

            m_physicsBody.OnCollision += InternalOnCollision;

            // Create wheel joint
            Vector2 axis = new Vector2(0.0f, 1f);

            m_wheelJoint = new WheelJoint(m_physicsParent.PhysicsBody, m_physicsBody, new Vector2(GetParent().Position.X / 100f, GetParent().Position.Y / 100f), axis, true);
            m_wheelJoint.MotorSpeed = MotorSpeed;
            m_wheelJoint.MaxMotorTorque = MaxTorque;
            m_wheelJoint.MotorEnabled = MotorEnabled;
            m_wheelJoint.Frequency = Frequency;
            m_wheelJoint.DampingRatio = Damping;
            m_wheelJoint.Enabled = true;

            TackPhysics.Instance.GetWorld().Add(m_wheelJoint);
        }
    }
}
