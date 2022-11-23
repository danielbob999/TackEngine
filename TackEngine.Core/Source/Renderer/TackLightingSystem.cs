using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Objects;
using TackEngineLib.Objects.Components;

namespace TackEngineLib.Renderer {
    public class TackLightingSystem {
        public static TackLightingSystem Instance = null;

        private List<LightComponent> m_lightComponents;

        public Colour4b AmbientLightColour { get; set; }
        public float AmbientLightIntensity { get; set; }
        public bool Enabled { get; set; }
        public int MaxLights { get; private set; }

        internal TackLightingSystem() {
            if (Instance != null) {
                return;
            }

            Instance = this;
            Enabled = true;
            AmbientLightColour = Colour4b.White;
            AmbientLightIntensity = 1f;
            MaxLights = 30;

            m_lightComponents = new List<LightComponent>();
        }

        internal void OnStart() {

        }

        internal void OnUpdate() {

        }

        internal void OnClose() {

        }

        internal void RegisterLightComponent(LightComponent comp) {
            if (!m_lightComponents.Contains(comp)) {
                m_lightComponents.Add(comp);
            }
        }

        internal void DeregisterLightComponent(LightComponent comp) {
            if (m_lightComponents.Contains(comp)) {
                m_lightComponents.Remove(comp);
            }
        }

        internal LightComponent[] GetLightComponents() {
            return m_lightComponents.ToArray();
        }
    }
}
