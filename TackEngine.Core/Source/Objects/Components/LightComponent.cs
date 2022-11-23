/* Copyright (c) 2019 Daniel Phillip Robinson */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngineLib.Main;
using TackEngineLib.Engine;
using TackEngineLib.Renderer;

namespace TackEngineLib.Objects.Components
{
    public class LightComponent : TackComponent {

        private float m_intensity;
        private float m_radius;

        public Colour4b Colour { get; set; }

        public float Intensity { 
            get { return m_intensity; } 
            set { m_intensity = Math.TackMath.Clamp(value, 0, float.PositiveInfinity); }
        }
        public float Radius {
            get { return m_radius; }
            set { m_radius = Math.TackMath.Clamp(value, 0, float.PositiveInfinity); }
        }

        public LightComponent() : base() {
            Colour = Colour4b.White;
            Intensity = 1f;
            Radius = 100f;
        }

        public LightComponent(Colour4b colour, float intensity, float radius) : base() {
            Colour = colour;
            Intensity = intensity;
            Radius = radius;
        }

        public override void OnStart() {
            base.OnStart();
        }

        public override void OnUpdate() {
            base.OnUpdate();
        }

        public override void OnRender() {
            base.OnRender();
        }

        public override void OnAttachedToTackObject() {
            base.OnAttachedToTackObject();

            TackLightingSystem.Instance.RegisterLightComponent(this);
        }

        public override void OnDetachedFromTackObject() {
            base.OnDetachedFromTackObject();

            TackLightingSystem.Instance.DeregisterLightComponent(this);
        }
    }
}
