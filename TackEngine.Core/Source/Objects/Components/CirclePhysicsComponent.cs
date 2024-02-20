using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Physics;
using TackEngine.Core.Main;
using TackEngine.Core.Math;
using tainicom.Aether.Physics2D.Common;
using TackEngine.Core.Renderer;
using tainicom.Aether.Physics2D.Dynamics;

namespace TackEngine.Core.Objects.Components {
    public class CirclePhysicsComponent : BasePhysicsComponent {

        /// <summary>
        /// Creates a new CirclePhysicsComponent
        /// </summary>
        public CirclePhysicsComponent() {
            Mass = 1f;
            IsStatic = false;
            IsAffectedByGravity = false;
            Friction = 1f;
            Restitution = 1f;
            FixedRotation = false;
        }

        /// <summary>
        /// Creates a new CirclePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        public CirclePhysicsComponent(float mass) : base() {
            Mass = mass;
            IsStatic = false;
            IsAffectedByGravity = false;
            Friction = 1f;
            Restitution = 1f;
            FixedRotation = false;
        }

        /// <summary>
        /// Creates a new CirclePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        /// <param name="isStatic">Is this component static? If true, it cannot be moved.</param>
        public CirclePhysicsComponent(float mass, bool isStatic) : base() {
            Mass = mass;
            IsStatic = isStatic;
            IsAffectedByGravity = false;
            Friction = 1f;
            Restitution = 1f;
            FixedRotation = false;
        }

        /// <summary>
        /// Creates a new CirclePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        /// <param name="isStatic">Is this component static? If true, it cannot be moved.</param>
        /// <param name="isAffectedByGravity">Is this component affected by gravity?</param>
        public CirclePhysicsComponent(float mass, bool isStatic, bool isAffectedByGravity) : base() {
            Mass = mass;
            IsStatic = isStatic;
            IsAffectedByGravity = isAffectedByGravity;
            Friction = 1f;
            Restitution = 1f;
            FixedRotation = false;
        }

        /// <summary>
        /// Creates a new CirclePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        /// <param name="isStatic">Is this component static? If true, it cannot be moved.</param>
        /// <param name="isAffectedByGravity">Is this component affected by gravity?</param>
        /// <param name="fixedRotation">Should this component have a fixed rotation?</param>
        public CirclePhysicsComponent(float mass, bool isStatic, bool isAffectedByGravity, bool fixedRotation) : base() {
            Mass = mass;
            IsStatic = isStatic;
            IsAffectedByGravity = isAffectedByGravity;
            Friction = 1f;
            Restitution = 1f;
            FixedRotation = fixedRotation;
        }

        /// <summary>
        /// Creates a new CirclePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        /// <param name="isStatic">Is this component static? If true, it cannot be moved.</param>
        /// <param name="isAffectedByGravity">Is this component affected by gravity?</param>
        /// <param name="fixedRotation">Should this component have a fixed rotation?</param>
        /// <param name="friction">The friction of this component when colliding with other components</param>
        public CirclePhysicsComponent(float mass, bool isStatic, bool isAffectedByGravity, bool fixedRotation, float friction) : base() {
            Mass = mass;
            IsStatic = isStatic;
            IsAffectedByGravity = isAffectedByGravity;
            Friction = friction;
            Restitution = 1f;
            FixedRotation = fixedRotation;
        }

        /// <summary>
        /// Creates a new CirclePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        /// <param name="isStatic">Is this component static? If true, it cannot be moved.</param>
        /// <param name="isAffectedByGravity">Is this component affected by gravity?</param>
        /// <param name="fixedRotation">Should this component have a fixed rotation?</param>
        /// <param name="friction">The friction of this component when colliding with other components</param>
        /// <param name="restitution">The restitution of this component</param>
        public CirclePhysicsComponent(float mass, bool isStatic, bool isAffectedByGravity, bool fixedRotation, float friction, float restitution) : base() {
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

        protected override void GenerateBody() {
            // Destroy the body before we regenerate it
            DestroyBody();

            m_physicsBody = TackPhysics.Instance.GetWorld().CreateBody(new Vector2(GetParent().Position.X / 100f, GetParent().Position.Y / 100f), TackMath.DegToRad(GetParent().Rotation), GetBodyType());
            m_physicsBody.FixedRotation = false;
            m_physicsBody.SleepingAllowed = true;
            //m_physicsBody.AngularDamping = 0f;
            //m_physicsBody.LinearDamping = 0;
            m_physicsBody.IgnoreGravity = !IsAffectedByGravity;
            m_physicsBody.Tag = GetParent().Hash;

            Fixture fixture = m_physicsBody.CreateCircle((GetParent().Scale.X / 2f) / 100f, 10);
            fixture.Restitution = Restitution;
            fixture.Friction = Friction;

            m_fixtures.Add(fixture);

            m_physicsBody.OnCollision += InternalOnCollision;
            m_physicsBody.OnSeparation += InternalOnSeparation;
        }

        internal override void OnDebugDraw() {
            int numOfSegments = 10;
            int segAngle = 360 / numOfSegments;

            Vector2f prevPoint = new Vector2f();
            for (int i = 0; i <= numOfSegments; i++) {
                if (i == 0) {
                    prevPoint = new Vector2f(
                            GetParent().Position.X + (GetParent().Scale.X / 2f) * (float)System.Math.Cos(TackMath.DegToRad(i * segAngle)),
                            GetParent().Position.Y + (GetParent().Scale.X / 2f) * (float)System.Math.Sin(TackMath.DegToRad(i * segAngle)));

                    continue;
                }

                Vector2f posOnCircle = new Vector2f(
                    GetParent().Position.X + (GetParent().Scale.X / 2f) * (float)System.Math.Cos(TackMath.DegToRad(i * segAngle)),
                    GetParent().Position.Y + (GetParent().Scale.X / 2f) * (float)System.Math.Sin(TackMath.DegToRad(i * segAngle)));

                DebugLineRenderer.DrawLine(prevPoint, posOnCircle, TackPhysics.BoundsColour);

                prevPoint = posOnCircle;

            }
        }
    }
}
