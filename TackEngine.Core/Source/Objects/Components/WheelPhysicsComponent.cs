using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Math;
using TackEngine.Core.Physics;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics.Joints;
using tainicom.Aether.Physics2D.Dynamics;
using TackEngine.Core.Main;
using TackEngine.Core.Renderer;

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

        protected override void DestroyBody() {
            base.DestroyBody();

            if (m_wheelJoint == null) {
                return;
            }

            TackPhysics.Instance.GetWorld().Remove(m_wheelJoint);
        }

        protected override void GenerateBody() {
            // Destroy the body before we regenerate it
            DestroyBody();

            m_physicsBody = TackPhysics.Instance.GetWorld().CreateBody(new Vector2(GetParent().Position.X / 100f, GetParent().Position.Y / 100f), TackMath.DegToRad(GetParent().Rotation), GetBodyType());
            m_physicsBody.FixedRotation = false;
            m_physicsBody.SleepingAllowed = true;
            m_physicsBody.IgnoreGravity = !IsAffectedByGravity;
            m_physicsBody.Tag = GetParent().Hash;
            
            Fixture fixture = m_physicsBody.CreateCircle((GetParent().Size.X / 2f) / 100f, 1);
            fixture.Restitution = 0f;
            fixture.Friction = Friction;

            m_fixtures.Add(fixture);

            m_physicsBody.OnCollision += InternalOnCollision;
            m_physicsBody.OnSeparation += InternalOnSeparation;

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

        internal override void OnDebugDraw() {
            int numOfSegments = 10;
            int segAngle = 360 / numOfSegments;

            Vector2f prevPoint = new Vector2f();
            for (int i = 0; i <= numOfSegments; i++) {
                if (i == 0) {
                    prevPoint = new Vector2f(
                            GetParent().Position.X + (GetParent().Size.X / 2f) * (float)System.Math.Cos(TackMath.DegToRad(i * segAngle)),
                            GetParent().Position.Y + (GetParent().Size.X / 2f) * (float)System.Math.Sin(TackMath.DegToRad(i * segAngle)));

                    continue;
                }

                Vector2f posOnCircle = new Vector2f(
                    GetParent().Position.X + (GetParent().Size.X / 2f) * (float)System.Math.Cos(TackMath.DegToRad(i * segAngle)),
                    GetParent().Position.Y + (GetParent().Size.X / 2f) * (float)System.Math.Sin(TackMath.DegToRad(i * segAngle)));

                DebugLineRenderer.DrawLine(prevPoint, posOnCircle, TackPhysics.WheelColour);

                prevPoint = posOnCircle;

            }

            Vector2f posOnCircle1 = new Vector2f(
                    GetParent().Position.X + (GetParent().Size.X / 2f) * (float)System.Math.Cos(TackMath.DegToRad(GetParent().Rotation)),
                    GetParent().Position.Y + (GetParent().Size.X / 2f) * (float)System.Math.Sin(TackMath.DegToRad(GetParent().Rotation)));

            DebugLineRenderer.DrawLine(GetParent().Position, posOnCircle1, TackPhysics.WheelColour);
        }
    }
}
