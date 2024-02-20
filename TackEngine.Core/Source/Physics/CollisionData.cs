using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Objects;
using TackEngine.Core.Objects.Components;

namespace TackEngine.Core.Physics {
    public class CollisionData {
        public BasePhysicsComponent OtherComponent { get; private set; }
        internal CollisionData(BasePhysicsComponent otherComp) {
            OtherComponent = otherComp;
        }
    }
}
