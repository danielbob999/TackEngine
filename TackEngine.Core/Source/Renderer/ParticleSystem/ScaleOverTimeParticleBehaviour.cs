using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;

namespace TackEngine.Core.Renderer.ParticleSystem {
    public class ScaleOverTimeParticleBehaviour : ParticleBehaviour {

        public Vector2f ScaleFrom { get; set; }
        public Vector2f ScaleTo { get; set; }

        public ScaleOverTimeParticleBehaviour(Vector2f from, Vector2f to, float maxAliveTime) : base(maxAliveTime) {
            ScaleFrom = from;
            ScaleTo = to;
        }

        public override void Spawn(ref Particle particle) {
            particle.Scale = ScaleFrom;
        }

        public override void Update(ref Particle particle, float delta) {
            particle.Scale = ScaleLerp(ScaleFrom, ScaleTo, particle.AliveTime / MaxAliveTime);
        }

        private Vector2f ScaleLerp(Vector2f from, Vector2f to, float time) {
            float x = (to.X - from.X) * time + from.X;
            float y = (to.Y - from.Y) * time + from.Y;

            return new Vector2f(x, y);
        }
    }
}
