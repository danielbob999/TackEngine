using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Objects;
using TackEngineLib.Objects.Components;

namespace TackEngineLib.Physics {
    public class CollisionData {
        public BasePhysicsComponent OtherComponent { get; private set; }
        internal CollisionData(BasePhysicsComponent otherComp) {
            OtherComponent = otherComp;
        }
    }
}
