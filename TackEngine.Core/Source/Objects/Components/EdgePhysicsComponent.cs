using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TackEngine.Core.Main;
using TackEngine.Core.Math;
using TackEngine.Core.Objects.Components;
using TackEngine.Core.Physics;
using TackEngine.Core.Renderer;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;

namespace TackEngine.Core.Objects.Components {
    public class EdgePhysicsComponent : BasePhysicsComponent {

        public List<Vector2f> Points { get; set; }
        public List<Fixture> Fixtures { get; set; }

        public EdgePhysicsComponent() : base() {
            Points = new List<Vector2f>();
            Fixtures = new List<Fixture>();
        }

        public override void OnStart() {
            base.OnStart();
        }

        public override void OnUpdate() {
            base.OnUpdate();
        }

        protected override void GenerateBody() {
            // Destroy the body before we regenerate it
            DestroyBody();

            /*
            m_physicsBody = TackPhysics.Instance.GetWorld().CreateBody(new Vector2(GetParent().Position.X / 100f, GetParent().Position.Y / 100f), TackMath.DegToRad(GetParent().Rotation), GetBodyType());
            m_physicsBody.FixedRotation = false;
            m_physicsBody.SleepingAllowed = false;
            //m_physicsBody.AngularDamping = 0f;
            //m_physicsBody.LinearDamping = 0;
            m_physicsBody.IgnoreGravity = !IsAffectedByGravity;
            m_physicsBody.Tag = GetParent().Hash;

            Vertices verts = new Vertices();
            
            for (int i = 0; i < Points.Count; i++) {
                verts.Add(new Vector2(Points[i].X / 100f, Points[i].Y / 100f));
            }

            ChainShape shape = new ChainShape(verts, false);

            m_fixture = m_physicsBody.CreateChainShape(verts);
            m_fixture.Restitution = Restitution;
            m_fixture.Friction = Friction;

            m_physicsBody.OnCollision += InternalOnCollision;*/


            // new
            base.IsStatic = true;
            base.IsAffectedByGravity = false;
            base.FixedRotation = false;

            m_physicsBody = TackPhysics.Instance.GetWorld().CreateBody(new Vector2(GetParent().Position.X / 100f, GetParent().Position.Y / 100f), TackMath.DegToRad(GetParent().Rotation), GetBodyType());
            m_physicsBody.FixedRotation = false;
            m_physicsBody.SleepingAllowed = false;
            //m_physicsBody.AngularDamping = 0f;
            //m_physicsBody.LinearDamping = 0;
            m_physicsBody.IgnoreGravity = true;
            m_physicsBody.Tag = GetParent().Hash;

            for (int i = 1; i < Points.Count; i ++) {
                Fixtures.Add(m_physicsBody.CreateEdge(new Vector2((Points[i - 1].X * GetParent().Scale.X) / 100f, (Points[i - 1].Y * GetParent().Scale.Y) / 100f), new Vector2((Points[i].X * GetParent().Scale.X) / 100f, (Points[i].Y * GetParent().Scale.Y) / 100f)));
            }

            m_physicsBody.OnCollision += InternalOnCollision;
            m_physicsBody.OnSeparation += InternalOnSeparation;
        }

        internal override void OnDebugDraw() {
            for (int i = 0; i < Fixtures.Count; i++) {
                //DebugLineRenderer.DrawLine(new Vector2f(verts[i - 1].X * 100f, verts[i - 1].Y * 100f), new Vector2f(verts[i].X * 100f, verts[i].Y * 100f), TackPhysics.BoundsColour);
                EdgeShape shape = (EdgeShape)Fixtures[i].Shape;

                DebugLineRenderer.DrawLine(
                    new Vector2f(GetParent().Position.X + (shape.Vertex1.X * 100f), GetParent().Position.Y + (shape.Vertex1.Y * 100f)),
                    new Vector2f(GetParent().Position.X + (shape.Vertex2.X * 100f), GetParent().Position.Y + (shape.Vertex2.Y * 100f)),
                    TackPhysics.BoundsColour);
            }
        }

        public static EdgePhysicsComponent GenerateFromSprite(Sprite s, int quality, byte transThreshold, bool flipVertically, bool centre) {
            EdgePhysicsComponent epc = new EdgePhysicsComponent();

            List<Vector2f> points = new List<Vector2f>();

            byte[] data = s.Data;

            float clampedQual = (TackMath.Clamp(quality, 1, 100) / 100f);

            int columnCount = (int)(s.Width * clampedQual);
            int columnSkip = s.Width / columnCount;

            float centreV = 0f;

            if (centre) {
                centreV = 0.5f;
            }

            for (int x = 0; x < s.Width; x += columnSkip) {
                for (int y = 0; y < s.Height; y++) {
                    byte alphaValue = data[3 + 4 * (x + y * s.Width)];

                    if (alphaValue > transThreshold) {
                        if (flipVertically) {
                            points.Add(new Vector2f((x / (float)s.Width) - centreV, (1 - (y / (float)s.Height)) - centreV));
                        } else {
                            points.Add(new Vector2f((x / (float)s.Width) - centreV, (y / (float)s.Height) - centreV));
                        }

                        break;
                    }
                }
            }

            epc.Points = points;

            return epc;
        }
    }
}
