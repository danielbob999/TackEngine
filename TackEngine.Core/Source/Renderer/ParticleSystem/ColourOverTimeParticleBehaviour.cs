using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.Renderer.ParticleSystem {
    public class ColourOverTimeParticleBehaviour : ParticleBehaviour {

        public Colour4b ColourFrom { get; set; }
        public Colour4b ColourTo { get; set; }

        public ColourOverTimeParticleBehaviour(Colour4b from, Colour4b to, float maxAliveTime) : base(maxAliveTime) {
            ColourFrom = from;
            ColourTo = to;
        }

        public override void Spawn(ref Particle particle) {
            particle.Colour = ColourFrom;
        }

        public override void Update(ref Particle particle, float delta) {
            particle.Colour = Lerp(ColourFrom, ColourTo, particle.AliveTime / MaxAliveTime);
        }

        private Colour4b Lerp(Colour4b from, Colour4b to, float time) {
            float r = (to.R - from.R) * time + from.R;
            float g = (to.G - from.G) * time + from.G;
            float b = (to.B - from.B) * time + from.B;
            float a = (to.A - from.A) * time + from.A;

            return new Colour4b((byte)r, (byte)g, (byte)b, (byte)a);
        }
    }
}
