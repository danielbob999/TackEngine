using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Engine;
using TackEngineLib.Renderer.ParticleSystem;

namespace TackEngineLib.Objects.Components {
    public class ParticleSystemComponent : TackComponent {

        private float m_timeSinceLastSpawn = 0;
        private List<Particle> m_particles;
        private List<ParticleBehaviour> m_behaviours;

        public bool IsEmitting { get; private set; }

        /// <summary>
        /// The amount of m_particles emitted per second
        /// </summary>
        public float ParticlesPerSecond { get; set; }

        // Default Particle variables
        /// <summary>
        /// The time that this particle is active for. Once it has been active in the world for this amount of time, it is destroyed
        /// </summary>
        public float MaxParticleAliveTime { get; set; }

        /// <summary>
        /// The default texture of the emitted m_particles. This can be overridden by ParticleBehaviours
        /// </summary>
        public Sprite ParticleTexture { get; set; }

        /// <summary>
        /// The default colour of the emitted m_particles. This can be overridden by ParticleBehaviours
        /// </summary>
        public Colour4b ParticleColour { get; set; }
        
        /// <summary>
        /// The default scale of the emitted m_particles. This can be overridden by ParticleBehaviours
        /// </summary>
        public Vector2f ParticleScale { get; set; }

        /// <summary>
        /// The default rotation of the emitted m_particles. This can be overridden by ParticleBehaviours
        /// </summary>
        public float ParticleRotation { get; set; }

        public ParticleSystemComponent() {
            m_particles = new List<Particle>();
            m_behaviours = new List<ParticleBehaviour>();
            ParticlesPerSecond = 1f;

            ParticleTexture = Sprite.DefaultSprite;
            ParticleColour = Colour4b.White;
            ParticleScale = new Vector2f(10f, 10f);
        }

        public override void OnStart() {
            base.OnStart();

            IsEmitting = true;
        }

        public override void OnUpdate() {
            base.OnUpdate();

            m_timeSinceLastSpawn += (float)EngineTimer.Instance.LastUpdateTime;

            List<Particle> oldm_particles = new List<Particle>();

            // Loop throught m_particles and call update methods of Particle and all loaded Behaviours
            for (int i = 0; i < m_particles.Count; i++) {
                Particle particle = m_particles[i];

                particle.AliveTime += (float)EngineTimer.Instance.LastUpdateTime;
                m_behaviours.ForEach(behaviour => behaviour.Update(ref particle, (float)EngineTimer.Instance.LastUpdateTime));
                particle.Update();

                if (particle.AliveTime > MaxParticleAliveTime) {
                    oldm_particles.Add(particle);
                }
            }

            // Destory and remove all expired m_particles  
            oldm_particles.ForEach(particle => {
                m_particles.Remove(particle);
                particle.Destory();
            });

            if (IsEmitting) {
                float timeBetweenSpawns = 1 / ParticlesPerSecond;

                if (m_timeSinceLastSpawn > timeBetweenSpawns) {
                    // Spawn particle
                    Particle newParticle = new Particle(new Vector2f(), ParticleScale, ParticleRotation, ParticleTexture, ParticleColour);
                    m_behaviours.ForEach(behaviour => behaviour.Spawn(ref newParticle));
                    m_particles.Add(newParticle);

                    m_timeSinceLastSpawn = 0;
                }
            }
        }

        /// <summary>
        /// Starts the m_particlesystemComponents emitting m_particles
        /// </summary>
        public void StartEmitting() {
            IsEmitting = true;
        }

        /// <summary>
        /// Stops the m_particlesystemComponent from emitting m_particles
        /// </summary>
        public void StopEmitting() {
            IsEmitting = false;
        }

        /// <summary>
        /// Spawns a burst of m_particles
        /// </summary>
        /// <param name="count">The amount of m_particles to spawn</param>
        public void SpawnParticleBurst(int count) {
            for (int i = 0; i < count; i++) {
                Particle newParticle = new Particle(new Vector2f(), ParticleScale, ParticleRotation, ParticleTexture, ParticleColour);
                m_behaviours.ForEach(behaviour => behaviour.Spawn(ref newParticle));
                m_particles.Add(newParticle);
            }
        }

        /// <summary>
        /// Adds a ParticleBehaviour to this m_particlesystemComponent
        /// </summary>
        /// <param name="behaviour"></param>
        public void AddBehaviour(ParticleBehaviour behaviour) {
            Type type = behaviour.GetType();

            int behavioursOfTypeCount = m_behaviours.Count(x => x.GetType() == type);

            if (behavioursOfTypeCount == 0) {
                m_behaviours.Add(behaviour);
                //TackConsole.EngineLog(TackConsole.LogType.Message, "Added ParticleBehaviour of type '" + type.Name + "' to m_particlesystemComponent attached to TackObject with name '" + GetParent().Name + "'");
            }
        }

        /// <summary>
        /// Removes a ParticleBehaviour
        /// </summary>
        /// <param name="type"></param>
        public void RemoveBehaviour(Type type) {
            if (type.BaseType != typeof(ParticleBehaviour)) {
                return;
            }

            m_behaviours.RemoveAll(x => x.GetType() == type);
        }

        public IReadOnlyList<Particle> GetParticles() {
            return m_particles;
        }
    }
}
