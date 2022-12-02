using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Objects.Components;

namespace TackEngine.Core.Physics {
    internal class PhysicsCollision {
        private BasePhysicsComponent m_body1;
        private BasePhysicsComponent m_body2;
        private Vector2f m_normal;
        private float m_penetration;

        public BasePhysicsComponent Body1 {
            get { return m_body1; }
        }

        public BasePhysicsComponent Body2 {
            get { return m_body2; }
        }

        public Vector2f Normal {
            get { return m_normal; }
        }

        public float Penetration {
            get { return m_penetration; }
        }

        public PhysicsCollision(BasePhysicsComponent b1, BasePhysicsComponent b2, Vector2f normal, float penetration) {
            m_body1 = b1;
            m_body2 = b2;
            m_normal = normal;
            m_penetration = penetration;
        }
    }
}
