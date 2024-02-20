using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngine.Core.Renderer.ParticleSystem {
    public abstract class ParticleBehaviour {
        protected float MaxAliveTime { get; private set; }

        public ParticleBehaviour(float maxAliveTime) {
            MaxAliveTime = maxAliveTime;
        }

        public virtual void Spawn(ref Particle particle) {
            
        }

        public virtual void Update(ref Particle particle, float delta) {

        }
    }
}
