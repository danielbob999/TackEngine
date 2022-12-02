using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;

namespace TackEngine.Core.Renderer.ParticleSystem {
    public class PositionOverTimeParticleBehaviour : ParticleBehaviour {

        public Vector2f PositionFrom { get; set; }
        public Vector2f PositionTo { get; set; }

        public PositionOverTimeParticleBehaviour(Vector2f from, Vector2f to, float maxAliveTime) : base(maxAliveTime) {
            PositionFrom = from;
            PositionTo = to;
        }

        public override void Spawn(ref Particle particle) {
            particle.Position = PositionFrom;
        }

        public override void Update(ref Particle particle, float delta) {
            particle.Position = PositionLerp(PositionFrom, PositionTo, particle.AliveTime / MaxAliveTime);
        }

        private Vector2f PositionLerp(Vector2f from, Vector2f to, float time) {
            float x = (to.X - from.X) * time + from.X;
            float y = (to.Y - from.Y) * time + from.Y;

            return new Vector2f(x, y);
        }
    }
}
