using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;

namespace TackEngineLib.Renderer.ParticleSystem {
    public class ConstantPositionChangeParticleBehaviour : ParticleBehaviour {
        public Vector2f PositionChange { get; set; }
        public Vector2f SpawnPosition { get; set; }

        public ConstantPositionChangeParticleBehaviour(Vector2f change, Vector2f spawnLocation, float maxAliveTime) : base(maxAliveTime) {
            PositionChange = change;
            SpawnPosition = spawnLocation;
        }

        public override void Spawn(ref Particle particle) {
            particle.Position = SpawnPosition;
        }

        public override void Update(ref Particle particle, float delta) {
            particle.Position += PositionChange * delta;
        }
    }
}
