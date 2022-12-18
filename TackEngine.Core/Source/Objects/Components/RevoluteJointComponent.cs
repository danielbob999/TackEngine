using System;
using System.Collections.Generic;
using System.Text;
using TackEngine.Core.Main;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Physics;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Joints;

namespace TackEngine.Core.Objects.Components {
    public class RevoluteJointComponent : TackComponent {

        private RevoluteJoint m_joint;

        public BasePhysicsComponent BodyB { get; set; }
        public Vector2f Anchor { get; set; }
        public float LowerLimit { get; set; }
        public float UpperLimit { get; set; }
        public bool LimitEnabled { get; set; }

        public RevoluteJointComponent() {

        }

        public override void OnUpdate() {
            base.OnUpdate();

            if (m_joint != null) {
                //Console.WriteLine(m_physicsBody.Rotation);

                m_joint.LowerLimit = LowerLimit;
                m_joint.UpperLimit = UpperLimit;
                m_joint.LimitEnabled = LimitEnabled;
            }
        }

        public override void OnAttachedToTackObject() {
            base.OnAttachedToTackObject();

            GenerateJoint();
        }

        protected void DestroyJoint() {
            if (m_joint == null) {
                return;
            }

            TackPhysics.Instance.GetWorld().Remove(m_joint);

            m_joint = null;
        }

        /// <summary>
        /// Generates a new Body. If not overriden, will generate a rectangular body
        /// </summary>
        private void GenerateJoint() {
            // Destroy the body before we regenerate it
            DestroyJoint();

            if (BodyB == null) {
                // Do we log error message? Throw exception?
                return;
            }

            m_joint = new RevoluteJoint(GetParent().GetComponent<RectanglePhysicsComponent>().PhysicsBody, BodyB.PhysicsBody, new tainicom.Aether.Physics2D.Common.Vector2(Anchor.X / 100f, Anchor.Y / 100f), true);
            m_joint.Enabled = true;

            TackPhysics.Instance.GetWorld().Add(m_joint);
        }
    }
}
