using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Objects;

namespace TackEngine.Core.Renderer.ParticleSystem {
    public class RandomSpawnPositionParticleBehaviour : ParticleBehaviour {
        private Random m_rand;
        private string m_particleSystemParentHash;

        public Vector2f BoundsTopLeft { get; set; }
        public Vector2f BoundsBottomRight { get; set; }

        public RandomSpawnPositionParticleBehaviour(string parentHash, float maxAliveTime) : base(maxAliveTime) {
            m_particleSystemParentHash = parentHash;
            m_rand = new Random();
        }

        public override void Spawn(ref Particle particle) {
            RecalculateBounds();

            Vector2f nextPosClamped = new Vector2f((float)m_rand.NextDouble(), (float)m_rand.NextDouble());

            Vector2f boundsSize = new Vector2f(
                BoundsBottomRight.X - BoundsTopLeft.X,
                BoundsTopLeft.Y - BoundsBottomRight.Y);

            particle.Position = new Vector2f(BoundsTopLeft.X + (nextPosClamped.X * boundsSize.X), BoundsBottomRight.Y + (nextPosClamped.Y * boundsSize.Y));
        }

        private void RecalculateBounds() {
            TackObject parentObj = TackObject.GetByHash(m_particleSystemParentHash);

            BoundsTopLeft = new Vector2f(parentObj.Position.X - (parentObj.Size.X / 2f), parentObj.Position.Y + (parentObj.Size.Y / 2f));
            BoundsBottomRight = new Vector2f(parentObj.Position.X + (parentObj.Size.X / 2f), parentObj.Position.Y - (parentObj.Size.Y / 2f));
        }
    }
}
