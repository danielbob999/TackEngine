using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Math;
using TackEngine.Core.Physics;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using tainicom.Aether.Physics2D.Dynamics;

namespace TackEngine.Core.Objects.Components {
    public class WheelPhysicsComponent : BasePhysicsComponent {

        private WheelJoint m_wheelJoint;

        protected override void GenerateBody() {
            // Destroy the body before we regenerate it
            DestroyBody();

            m_physicsBody = TackPhysics.Instance.GetWorld().CreateBody(new Vector2(GetParent().Position.X / 100f, GetParent().Position.Y / 100f), TackMath.DegToRad(GetParent().Rotation), GetBodyType());
            m_physicsBody.FixedRotation = false;
            m_physicsBody.SleepingAllowed = false;
            //m_physicsBody.AngularDamping = 0f;
            //m_physicsBody.LinearDamping = 0;
            m_physicsBody.IgnoreGravity = !IsAffectedByGravity;
            m_physicsBody.Tag = GetParent().Hash;
            m_fixture = m_physicsBody.CreateCircle((GetParent().Scale.X / 2f) / 100f, 10);
            m_fixture.Restitution = Restitution;
            m_fixture.Friction = Friction;

            m_physicsBody.OnCollision += InternalOnCollision;

            // Create wheel joint
            Vector2 axis = new Vector2(0.0f, 1.0f);
            m_wheelJoint = new WheelJoint((GetParent().GetComponent<BasePhysicsComponent>()).PhysicsBody, m_physicsBody, new Vector2(GetParent().LocalPosition.X / 100f, GetParent().LocalPosition.Y / 100f), axis, true);
            m_wheelJoint.MotorSpeed = 0.0f;
            m_wheelJoint.MaxMotorTorque = 20.0f;
            m_wheelJoint.MotorEnabled = true;
            m_wheelJoint.Frequency = 4f;
            m_wheelJoint.DampingRatio = 0.7f;

            TackPhysics.Instance.GetWorld().Add(m_wheelJoint);
        }
    }
}
