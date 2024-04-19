using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TackEngine.Core.Physics;
using TackEngine.Core.Main;
using TackEngine.Core.Math;
using tainicom.Aether.Physics2D.Common;
using TackEngine.Core.Renderer;

namespace TackEngine.Core.Objects.Components {
    public class RectanglePhysicsComponent : BasePhysicsComponent {

        public RectanglePhysicsComponent() : base() {
            Mass = 1f;
            IsStatic = false;
            IsAffectedByGravity = false;
            Friction = 1f;
            Restitution = 1f;
            FixedRotation = false;
        }

        /// <summary>
        /// Creates a new RectanglePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        public RectanglePhysicsComponent(float mass) : base() {
            Mass = mass;
            IsStatic = false;
            IsAffectedByGravity = false;
            Friction = 1f;
            Restitution = 1f;
            FixedRotation = false;
        }

        /// <summary>
        /// Creates a new RectanglePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        /// <param name="isStatic">Is this component static? If true, it cannot be moved.</param>
        public RectanglePhysicsComponent(float mass, bool isStatic) : base() {
            Mass = mass;
            IsStatic = isStatic;
            IsAffectedByGravity = false;
            Friction = 1f;
            Restitution = 1f;
            FixedRotation = false;
        }

        /// <summary>
        /// Creates a new RectanglePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        /// <param name="isStatic">Is this component static? If true, it cannot be moved.</param>
        /// <param name="isAffectedByGravity">Is this component affected by gravity?</param>
        public RectanglePhysicsComponent(float mass, bool isStatic, bool isAffectedByGravity) : base() {
            Mass = mass;
            IsStatic = isStatic;
            IsAffectedByGravity = isAffectedByGravity;
            Friction = 1f;
            Restitution = 1f;
            FixedRotation = false;
        }

        /// <summary>
        /// Creates a new RectanglePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        /// <param name="isStatic">Is this component static? If true, it cannot be moved.</param>
        /// <param name="isAffectedByGravity">Is this component affected by gravity?</param>
        /// <param name="fixedRotation">Should this component have a fixed rotation?</param>
        public RectanglePhysicsComponent(float mass, bool isStatic, bool isAffectedByGravity, bool fixedRotation) : base() {
            Mass = mass;
            IsStatic = isStatic;
            IsAffectedByGravity = isAffectedByGravity;
            Friction = 1f;
            Restitution = 1f;
            FixedRotation = fixedRotation;
        }

        /// <summary>
        /// Creates a new RectanglePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        /// <param name="isStatic">Is this component static? If true, it cannot be moved.</param>
        /// <param name="isAffectedByGravity">Is this component affected by gravity?</param>
        /// <param name="fixedRotation">Should this component have a fixed rotation?</param>
        /// <param name="friction">The friction of this component when colliding with other components</param>
        public RectanglePhysicsComponent(float mass, bool isStatic, bool isAffectedByGravity, bool fixedRotation, float friction) : base() {
            Mass = mass;
            IsStatic = isStatic;
            IsAffectedByGravity = isAffectedByGravity;
            Friction = friction;
            Restitution = 1f;
            FixedRotation = fixedRotation;
        }

        /// <summary>
        /// Creates a new RectanglePhysicsComponent
        /// </summary>
        /// <param name="mass">The mass of this component</param>
        /// <param name="isStatic">Is this component static? If true, it cannot be moved.</param>
        /// <param name="isAffectedByGravity">Is this component affected by gravity?</param>
        /// <param name="fixedRotation">Should this component have a fixed rotation?</param>
        /// <param name="friction">The friction of this component when colliding with other components</param>
        /// <param name="restitution">The restitution of this component</param>
        public RectanglePhysicsComponent(float mass, bool isStatic, bool isAffectedByGravity, bool fixedRotation, float friction, float restitution) : base() {
            Mass = mass;
            IsStatic = isStatic;
            IsAffectedByGravity = isAffectedByGravity;
            Friction = friction;
            Restitution = restitution;
            FixedRotation = fixedRotation;
        }

        public override void OnStart() {
            base.OnStart();
        }

        public override void OnUpdate() {
            base.OnUpdate();
        }

        public override void OnClose() {
            base.OnClose();
        }

        internal override void OnDebugDraw() {
            Vector2f pos = GetParent().Position;
            Vector2f halfSize = GetParent().Size / 2f;

            // top line
            DebugLineRenderer.DrawLine(new Vector2f(pos.X, pos.Y + halfSize.Y), GetParent().Rotation, TackPhysics.BoundsColour, GetParent().Size.X);

            // bottom line
            DebugLineRenderer.DrawLine(new Vector2f(pos.X, pos.Y - halfSize.Y), GetParent().Rotation, TackPhysics.BoundsColour, GetParent().Size.X);

            // left line
            Vector2f rotatedPosLeft = new Vector2f(
                (float)(System.Math.Cos(TackMath.DegToRad(GetParent().Rotation)) * ((pos.X - halfSize.X) - pos.X) - System.Math.Sin(TackMath.DegToRad(GetParent().Rotation)) * (pos.Y - pos.Y) + pos.X),
                (float)(System.Math.Sin(TackMath.DegToRad(GetParent().Rotation)) * ((pos.X - halfSize.X) - pos.X) + System.Math.Cos(TackMath.DegToRad(GetParent().Rotation)) * (pos.Y - pos.Y) + pos.Y)
                );
            DebugLineRenderer.DrawLine(rotatedPosLeft, GetParent().Rotation + 90f, TackPhysics.BoundsColour, GetParent().Size.Y);

            // right line
            Vector2f rotatedPosRight = new Vector2f(
                (float)(System.Math.Cos(TackMath.DegToRad(GetParent().Rotation)) * ((pos.X + halfSize.X) - pos.X) - System.Math.Sin(TackMath.DegToRad(GetParent().Rotation)) * (pos.Y - pos.Y) + pos.X),
                (float)(System.Math.Sin(TackMath.DegToRad(GetParent().Rotation)) * ((pos.X + halfSize.X) - pos.X) + System.Math.Cos(TackMath.DegToRad(GetParent().Rotation)) * (pos.Y - pos.Y) + pos.Y)
                );
            DebugLineRenderer.DrawLine(rotatedPosRight, GetParent().Rotation + 90f, TackPhysics.BoundsColour, GetParent().Size.Y);

            // Diagonal lines

        }
    }
}
