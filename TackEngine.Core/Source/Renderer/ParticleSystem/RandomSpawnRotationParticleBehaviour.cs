using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TackEngineLib.Renderer.ParticleSystem {
    public class RandomSpawnRotationParticleBehaviour : ParticleBehaviour {
        private Random m_rand;

        public float MinRotation { get; set; }
        public float MaxRotation { get; set; }

        public RandomSpawnRotationParticleBehaviour(float minRotation, float maxRotation, float maxAliveTime) : base(maxAliveTime) {
            m_rand = new Random();

            MinRotation = minRotation;
            MaxRotation = maxRotation;
        }

        public override void Spawn(ref Particle particle) {
            particle.Rotation = MinRotation + ((float)m_rand.NextDouble() * (MaxRotation - MinRotation));
        }
    }
}
