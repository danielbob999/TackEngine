using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.Renderer.ParticleSystem {
    public class RandomSpawnScaleParticleBehaviour : ParticleBehaviour {
        private Random m_rand;

        public Vector2f MinScale { get; set; }
        public Vector2f MaxScale { get; set; }

        public RandomSpawnScaleParticleBehaviour(Vector2f minScale, Vector2f maxScale, float maxAliveTime) : base(maxAliveTime) {
            m_rand = new Random();

            MinScale = minScale;
            MaxScale = maxScale;
        }

        public override void Spawn(ref Particle particle) {
            particle.Scale = ScaleLerp(MinScale, MaxScale, (float)m_rand.NextDouble());

        }

        private Vector2f ScaleLerp(Vector2f from, Vector2f to, float time) {
            float x = (to.X - from.X) * time + from.X;
            float y = (to.Y - from.Y) * time + from.Y;

            return new Vector2f(x, y);
        }
    }
}
