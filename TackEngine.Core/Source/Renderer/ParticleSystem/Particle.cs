using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Main;
using TackEngine.Core.Objects;
using TackEngine.Core.Objects.Components;

namespace TackEngine.Core.Renderer.ParticleSystem {
    public class Particle {
        private static int s_nextId = 0;

        public int Id { get; set; }
        public Vector2f Position { get; set; }
        public Vector2f Scale { get; set; }
        public float Rotation { get; set; }
        public Main.Sprite Sprite { get; set; }
        public Colour4b Colour { get; set; }
        public float AliveTime { get; internal set; }

        internal TackObject ConnectedObject { get; }
        internal SpriteRendererComponent RendererComponent { get; }

        internal Particle(Vector2f pos, Vector2f scale, float rotation, Main.Sprite sprite, Colour4b colour) {
            Id = s_nextId++;

            Position = pos;
            Scale = scale;
            Rotation = rotation;
            Sprite = sprite;
            Colour = colour;
            AliveTime = 0;
            Scale = new Vector2f(10, 10);

            // Generate a TackObject
            ConnectedObject = TackObject.Create("particle", Position, Scale, Rotation);

            // Add a SpriteRenderer
            RendererComponent = new SpriteRendererComponent() {
                Active = true,
                Colour = Colour,
                RenderLayer = 10,
                Sprite = Sprite,
            };

            ConnectedObject.AddComponent(RendererComponent);
        }

        internal void Update() {
            RendererComponent.Colour = Colour;
            RendererComponent.Sprite = Sprite;

            ConnectedObject.Position = Position;
            ConnectedObject.Scale = Scale;
            ConnectedObject.Rotation = Rotation;
        }

        internal void Destory() {
            ConnectedObject.Destroy();
        }
    }
}
